using GemBox.Document;
using GemBox.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using WaterPositive.Models;
using WaterPositive.Tools;
using WaterPositive.Web.Data;
using WaterPositive.Web.Helpers;
using ZXing.QrCode;

namespace WaterPositive.Web.Data
{
    public class ReportService
    {
        WaterPositiveDB db;
        CultureInfo ci;
        
        public event StatusChangedEventHandler StatusChanged;
        public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

        public class StatusChangedEventArgs : EventArgs
        {
            public int Progress { get; set; }
            public string Message { get; set; }
        }
        public ReportService()
        {
            ci = new CultureInfo("id-ID");
            if (db == null) db = new WaterPositiveDB();
           
        }

        string GetMonthName(int month)
        {
            if (month < 1 || month > 12) return "";
            var monthName = ci.DateTimeFormat.GetMonthName(month);
            return monthName;
        }

       
        public async Task<byte[]> CetakKartu(UserProfile item, bool IsLinux = false)
        {
            try
            {

                List<byte[]> DocParts = new List<byte[]>();
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 1, Message = "Mulai bikin dokumen..." });
                await Task.Delay(1);
                #region cover
                //1. sertifikat saham

                var bytes = ReadDocAsBytes(AppConstants.ReportKartu);
                // Load Word document from file's path.
                var document = DocumentModel.Load(new MemoryStream(bytes));
                var param = item;
                // Get Word document's plain text.

                document.Content.Replace("[Nama]", param.FullName);
                document.Content.Replace("[Alamat]", param.Alamat);
                document.Content.Replace("[UID]", param.UID);
                
                #region add image qr
                var width = 120;
                var height = 120;
                var QRString = param.UID;
                var img = new MemoryStream(GenerateQR(QRString, width, height));
                Picture qrPic = new Picture(document, img, width, height, GemBox.Document.LengthUnit.Pixel);

                var elements = document.GetChildElements(true, ElementType.TextBox);
                TextBox textBox = (TextBox)elements.Where(x=>x.Content.ToString().Contains("QR")).First();

                // If needed you can adjust the TextBox element's inner margin to your requirement.
                textBox.TextBoxFormat.InternalMargin = new Padding(0);

                // If needed you can remove any existing content from TextBox element.
                textBox.Blocks.Clear();

                // Get TextBox element's size.
                var textBoxSize = textBox.Layout.Size;

                // Create and add Picture element.
                textBox.Blocks.Add(
                    new Paragraph(document,
                         qrPic));
                /*
                var section = document.Sections.FirstOrDefault(); //new Section(document);
                //document.Sections.Add(section);
                var paragraph = new Paragraph(document);
                section.Blocks.Add(paragraph);
                FloatingLayout layout2 = new FloatingLayout(
           new HorizontalPosition(50, GemBox.Document.LengthUnit.Pixel, HorizontalPositionAnchor.Page),
           new VerticalPosition(60, GemBox.Document.LengthUnit.Pixel, VerticalPositionAnchor.Page),
           qrPic.Layout.Size);
                layout2.WrappingStyle = TextWrappingStyle.InFrontOfText;
               
                qrPic.Layout = layout2;
                paragraph.Inlines.Add(qrPic);
                */
                #endregion

                var dataBytes = AddDocToList(document);
                DocParts.Add(dataBytes);
                #endregion
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 90, Message = "Cetak selesai..." });
                await Task.Delay(1);

                //concatenate doc part..
                dataBytes = PdfHelper.MergePdf(DocParts);

                return await Task.FromResult(dataBytes);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke(this, new StatusChangedEventArgs() { Progress = 0, Message = "Terjadi kesalahan:" + ex.Message });
                Console.WriteLine(ex);
                return null;
            }
            void ChangeCellValue(DocumentModel document, GemBox.Document.Tables.Table table, int row, int col, object valueCell)
            {
                try
                {
                    var curRow = table.Rows[row];
                    var cell = curRow.Cells[col];
                    cell.Blocks.Clear();
                    // Create a paragraph and add it to cell.
                    var paragraph = new Paragraph(document, valueCell.ToString());
                    //paragraph.ParagraphFormat.Style.CharacterFormat = new CharacterFormat() { Bold = true, Size = 9 };
                    cell.Blocks.Add(paragraph);
                }
                catch { }
            }
            void AddRowCell(DocumentModel document, GemBox.Document.Tables.Table table, object[] Data)
            {
                var row = new GemBox.Document.Tables.TableRow(document);
                table.Rows.Add(row);
                for (int c = 0; c < Data.Length; c++)
                {
                    // Create a cell and add it to row.
                    var cell = new GemBox.Document.Tables.TableCell(document);
                    //cell.CellFormat.FitText = true;
                    row.Cells.Add(cell);

                    // Create a paragraph and add it to cell.
                    var paragraph = new Paragraph(document, Data[c].ToString());
                    paragraph.ParagraphFormat.Style.CharacterFormat = new CharacterFormat() { Bold = true, Size = 9 };
                    cell.Blocks.Add(paragraph);
                }
            }

            byte[] AddDocToList(DocumentModel document)
            {
                var pdfSaveOptions = new GemBox.Document.PdfSaveOptions() { ImageDpi = 220 };
                byte[] dataBytes;
                using (var pdfStream = new MemoryStream())
                {
                    document.Save(pdfStream, pdfSaveOptions);
                    dataBytes = pdfStream.ToArray();
                }
                return dataBytes;
            }
            byte[] AddXlsToList(ExcelFile document)
            {
                foreach (var ws in document.Worksheets)
                {
                    ws.PrintOptions.FitWorksheetWidthToPages = 1;
                    ws.PrintOptions.FitWorksheetHeightToPages = 1;
                }
                var pdfSaveOptions = new GemBox.Spreadsheet.PdfSaveOptions() { ImageDpi = 220 };
                byte[] dataBytes;
                using (var pdfStream = new MemoryStream())
                {
                    document.Save(pdfStream, pdfSaveOptions);
                    dataBytes = pdfStream.ToArray();
                }
                return dataBytes;
            }
            byte[] ReadDocAsBytes(string FilePath)
            {
                if (!System.IO.File.Exists(FilePath)) throw new Exception($"Template {Path.GetFileNameWithoutExtension(FilePath)} is not found");
                var _temp = string.Empty;
                if (IsLinux)
                {
                    _temp = FilePath.Replace("\\", "/");
                }
                else
                {
                    _temp = FilePath;
                }
                var bytes = System.IO.File.ReadAllBytes(_temp);
                return bytes;
            }
        }

        byte[] GenerateQR(string QRData, int width = 100, int height = 100)
        {


            var qr = new ZXing.BarcodeWriter();
            var options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = width,
                Height = height,
            };
            qr.Options = options;
            qr.Format = ZXing.BarcodeFormat.QR_CODE;

            var FilePath = Path.GetTempPath() + "/" + Guid.NewGuid().ToString().Replace("-", "_") + ".jpg";

            var result = new Bitmap(qr.Write(QRData));
            result.Save(FilePath, ImageFormat.Jpeg);

            var temp = File.ReadAllBytes(FilePath);
            return temp;
            //string base64String = Convert.ToBase64String(temp, 0, temp.Length);
            //QRImage = "data:image/png;base64," + base64String;
        }
    }
}
