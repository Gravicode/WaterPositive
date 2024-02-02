using ESC_POS_USB_NET.Enums;
using ESC_POS_USB_NET.Printer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalDotNet;
using WaterPositive.Models;

namespace WaterPositive.Kiosk.Data
{
    public class PosPrinterService
    {
        Printer printer { set; get; }
        public PosPrinterService()
        {
            Setup();
        }

        void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            printer = new Printer(AppConstants.PrinterName);
        }

        public void TestPrint()
        {
            Printer printer = new Printer(AppConstants.AppName);
            printer.TestPrinter();
            printer.FullPaperCut();
            printer.PrintDocument();
        }

        void PrintImage()
        {
            Printer printer = new Printer("Printer Name");
            Bitmap image = new Bitmap(Image.FromFile("Icon.bmp"));
            printer.Image(image);
            printer.FullPaperCut();
            printer.PrintDocument();
        }

        void PrintBarcode()
        {
            Printer printer = new Printer("Printer Name");
            printer.Append("Code 128");
            printer.Code128("123456789");
            printer.Separator();
            printer.Append("Code39");
            printer.Code39("123456789");
            printer.Separator();
            printer.Append("Ean13");
            printer.Ean13("1234567891231");
            printer.FullPaperCut();
            printer.PrintDocument();
        }
        public void CetakReceipt(WaterUsage usage)
        {
            try
            {
                var ci = new CultureInfo("id-ID");
                Dictionary<string, double> ItemList = new Dictionary<string, double>(100);
                printer.SetLineHeight(0);
                printer.AlignCenter();
                printer.UnderlineMode("AWS WATER POSITIVE");
                printer.Append("-- Tanda Terima Pemakaian Air --");
                printer.Append($"Depot: {usage.WaterDepot?.Nama}");
                printer.Append($"Tanggal/Waktu: {usage.UpdatedDate.ToString("dddd, dd-MMM-yyyy HH:mm",ci)}");
                printer.Append($"Nama: {usage.User?.FullName} - {usage.User?.KTP}");
                printer.Separator();

                ItemList.Add("Volume (Liter)", usage.Volume);
                ItemList.Add("Harga (Rp)", usage.TotalHarga);

                //int total = 0;
                foreach (KeyValuePair<string, double> item in ItemList)
                {
                    CashRegister(item.Key, item.Value);
                    //total += item.Value;
                }

                printer.Separator();

                //double dTotal = Convert.ToDouble(total) / 100;
                //double VAT = 10.0;

                //printer.Append(string.Format("{0:0.00}", usage.TotalHarga).PadLeft(32));

                //printer.WriteLine("VAT 10,0%" + String.Format("{0:0.00}", (dTotal * VAT / 100)).PadLeft(23));

                //printer.WriteLine(String.Format("$ {0:0.00}", dTotal * VAT / 100 + dTotal).PadLeft(16),
                //    ThermalPrinter.PrintingStyle.DoubleWidth);

                //printer.LineFeed();
                //printer.WriteLine("CASH" + String.Format("{0:0.00}", (double)total / 100).PadLeft(28));
                printer.NewLine();
                printer.NewLine();
                printer.AlignCenter();
                printer.BoldMode("Terima kasih, gunakan air dengan hemat.");
                printer.NewLine();
                printer.FullPaperCut();
                printer.PrintDocument();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Printing error: {ex}");
            }

        }

        void CashRegister(string item, double numValue)
        {
            printer.InitializePrint();
            printer.AlignLeft();
            printer.Append(item.ToUpper()+" : "+string.Format("{0:0.00}", numValue));
            printer.NewLine();
        }
        void TypoGraphyTest()
        {
            Printer printer = new Printer("Printer Name");
            printer.Append("NORMAL - 48 COLUMNS");
            printer.Append("1...5...10...15...20...25...30...35...40...45.48");
            printer.Separator();
            printer.Append("Text Normal");
            printer.BoldMode("Bold Text");
            printer.UnderlineMode("Underlined text");
            printer.Separator();
            printer.ExpandedMode(PrinterModeState.On);
            printer.Append("Expanded - 23 COLUMNS");
            printer.Append("1...5...10...15...20..23");
            printer.ExpandedMode(PrinterModeState.Off);
            printer.Separator();
            printer.CondensedMode(PrinterModeState.On);
            printer.Append("Condensed - 64 COLUMNS");
            printer.Append("1...5...10...15...20...25...30...35...40...45...50...55...60..64");
            printer.CondensedMode(PrinterModeState.Off);
            printer.Separator();
            printer.DoubleWidth2();
            printer.Append("Font Width 2");
            printer.DoubleWidth3();
            printer.Append("Font Width 3");
            printer.NormalWidth();
            printer.Append("Normal width");
            printer.Separator();
            printer.AlignRight();
            printer.Append("Right aligned text");
            printer.AlignCenter();
            printer.Append("Center-aligned text");
            printer.AlignLeft();
            printer.Append("Left aligned text");
            printer.Separator();
            printer.Font("Font A", Fonts.FontA);
            printer.Font("Font B", Fonts.FontB);
            printer.Font("Font C", Fonts.FontC);
            printer.Font("Font D", Fonts.FontD);
            printer.Font("Font E", Fonts.FontE);
            printer.Font("Font Special A", Fonts.SpecialFontA);
            printer.Font("Font Special B", Fonts.SpecialFontB);
            printer.Separator();
            printer.InitializePrint();
            printer.SetLineHeight(24);
            printer.Append("This is first line with line height of 30 dots");
            printer.SetLineHeight(40);
            printer.Append("This is second line with line height of 24 dots");
            printer.Append("This is third line with line height of 40 dots");
            printer.NewLines(3);
            printer.Append("End of Test :)");
            printer.Separator();
            printer.FullPaperCut();
            printer.PrintDocument();
        }
    }
}
