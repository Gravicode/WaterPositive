using ESCPOS_NET;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System.Drawing;
using System.IO.Ports;
using ThermalDotNet;

namespace TestPrinterThermal
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            //RunThermalDotNet();
            Print();
            //var pos = new PosPrinter();
            //pos.TestPrint();
            Console.ReadLine();
        }

        static void Print()
        {
            // USB, Bluetooth, or Serial
            var printer = new SerialPrinter(portName: "COM9", baudRate: 9600);
            // Define a callback method.

            // In your program, register event handler to call the method when printer status changes:
            printer.StatusChanged += StatusChanged;

            var e = new EPSON();

            
            printer.Write( // or, if using and immediate printer, use await printer.WriteAsync
              ByteSplicer.Combine(
                e.CenterAlign(),
                e.PrintImage(File.ReadAllBytes("pd-logo-300.png"), true),
                e.PrintLine("----------------------"),
                e.SetBarcodeHeightInDots(360),
                e.SetBarWidth(BarWidth.Default),
                e.SetBarLabelPosition(BarLabelPrintPosition.None),
                e.PrintBarcode(BarcodeType.ITF, "0123456789"),
                e.PrintLine("----------------------"),
                e.PrintLine("B&H PHOTO & VIDEO"),
                e.PrintLine("420 NINTH AVE."),
                e.PrintLine("NEW YORK, NY 10001"),
                e.PrintLine("(212) 502-6380 - (800)947-9975"),
                e.SetStyles(PrintStyle.Underline),
                e.PrintLine("www.bhphotovideo.com"),
                e.SetStyles(PrintStyle.None),
                e.PrintLine("----------------------"),
                e.LeftAlign(),
                e.PrintLine("Order: 123456789        Date: 02/01/19"),
                e.PrintLine("----------------------"),
                e.PrintLine("----------------------"),
                e.SetStyles(PrintStyle.FontB),
                e.PrintLine("1   TRITON LOW-NOISE IN-LINE MICROPHONE PREAMP"),
                e.PrintLine("    TRFETHEAD/FETHEAD                        89.95         89.95"),
                e.PrintLine("----------------------------------------------------------------"),
                e.RightAlign(),
                e.PrintLine("SUBTOTAL         89.95"),
                e.PrintLine("Total Order:         89.95"),
                e.PrintLine("Total Payment:         89.95"),
                e.PrintLine("----------------------"),
                e.LeftAlign(),
                e.SetStyles(PrintStyle.Bold | PrintStyle.FontB),
                e.PrintLine("SOLD TO:                        SHIP TO:"),
                e.SetStyles(PrintStyle.FontB),
                e.PrintLine("  FIRSTN LASTNAME                 FIRSTN LASTNAME"),
                e.PrintLine("  123 FAKE ST.                    123 FAKE ST."),
                e.PrintLine("  DECATUR, IL 12345               DECATUR, IL 12345"),
                e.PrintLine("  (123)456-7890                   (123)456-7890"),
                e.PrintLine("  CUST: 87654321"),
                e.PrintLine("----------------------")
              ));

        }
        static void StatusChanged(object sender, EventArgs ps)
        {
            var status = (PrinterStatusEventArgs)ps;
            Console.WriteLine($"Status: {status.IsPrinterOnline}");
            Console.WriteLine($"Has Paper? {status.IsPaperOut}");
            Console.WriteLine($"Paper Running Low? {status.IsPaperLow}");
            Console.WriteLine($"Cash Drawer Open? {status.IsCashDrawerOpen}");
            Console.WriteLine($"Cover Open? {status.IsCoverOpen}");
        }

        #region thermal dotnet
        static void TestReceipt(ThermalPrinter printer)
        {
            Dictionary<string, int> ItemList = new Dictionary<string, int>(100);
            printer.SetLineSpacing(0);
            printer.SetAlignCenter();
            printer.WriteLine("MY SHOP",
                (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
            printer.WriteLine("My address, CITY");
            printer.LineFeed();
            printer.LineFeed();

            ItemList.Add("Item #1", 8990);
            ItemList.Add("Item #2 goes here", 2000);
            ItemList.Add("Item #3", 1490);
            ItemList.Add("Item number four", 490);
            ItemList.Add("Item #5 is cheap", 245);
            ItemList.Add("Item #6", 2990);
            ItemList.Add("The seventh item", 790);

            int total = 0;
            foreach (KeyValuePair<string, int> item in ItemList)
            {
                CashRegister(printer, item.Key, item.Value);
                total += item.Value;
            }

            printer.HorizontalLine(32);

            double dTotal = Convert.ToDouble(total) / 100;
            double VAT = 10.0;

            printer.WriteLine(String.Format("{0:0.00}", (dTotal)).PadLeft(32));

            printer.WriteLine("VAT 10,0%" + String.Format("{0:0.00}", (dTotal * VAT / 100)).PadLeft(23));

            printer.WriteLine(String.Format("$ {0:0.00}", dTotal * VAT / 100 + dTotal).PadLeft(16),
                ThermalPrinter.PrintingStyle.DoubleWidth);

            printer.LineFeed();

            printer.WriteLine("CASH" + String.Format("{0:0.00}", (double)total / 100).PadLeft(28));
            printer.LineFeed();
            printer.LineFeed();
            printer.SetAlignCenter();
            printer.WriteLine("Have a good day.", ThermalPrinter.PrintingStyle.Bold);

            printer.LineFeed();
            printer.SetAlignLeft();
            printer.WriteLine("Seller : Bob");
            printer.WriteLine("09-28-2011 10:53 02331 509");
            printer.LineFeed();
            printer.LineFeed();
            printer.LineFeed();
        }

        static void TestBarcode(ThermalPrinter printer)
        {
            ThermalPrinter.BarcodeType myType = ThermalPrinter.BarcodeType.ean13;
            string myData = "3350030103392";
            printer.WriteLine(myType.ToString() + ", data: " + myData);
            printer.SetLargeBarcode(true);
            printer.LineFeed();
            printer.PrintBarcode(myType, myData);
            printer.SetLargeBarcode(false);
            printer.LineFeed();
            printer.PrintBarcode(myType, myData);
        }

        static void TestImage(ThermalPrinter printer)
        {
            printer.WriteLine("Test image:");
            Bitmap img = new Bitmap("../../../mono-logo.png");
            printer.LineFeed();
            printer.PrintImage(img);
            printer.LineFeed();
            printer.WriteLine("Image OK");
        }

        public static void RunThermalDotNet()
        {
            string printerPortName = "COM9";

            //Serial port init
            SerialPort printerPort = new SerialPort(printerPortName, 9600);

            if (printerPort != null)
            {
                Console.WriteLine("Port ok");
                if (printerPort.IsOpen)
                {
                    printerPort.Close();
                }
            }

            Console.WriteLine("Opening port");

            try
            {
                printerPort.Open();
            }
            catch
            {
                Console.WriteLine("I/O error");
                Environment.Exit(0);
            }

            //Printer init
            ThermalPrinter printer = new ThermalPrinter(printerPort, 2, 180, 2);
            printer.WakeUp();
            Console.WriteLine(printer.ToString());

            //TestReceipt(printer);

            //System.Threading.Thread.Sleep(5000);
            //printer.SetBarcodeLeftSpace(25);
            //TestBarcode(printer);

            //System.Threading.Thread.Sleep(5000);
            //TestImage(printer);

            //System.Threading.Thread.Sleep(5000);

            printer.WriteLineSleepTimeMs = 200;
            printer.WriteLine("Default style");
            printer.WriteLine("PrintingStyle.Bold", ThermalPrinter.PrintingStyle.Bold);
            printer.WriteLine("PrintingStyle.DeleteLine", ThermalPrinter.PrintingStyle.DeleteLine);
            printer.WriteLine("PrintingStyle.DoubleHeight", ThermalPrinter.PrintingStyle.DoubleHeight);
            printer.WriteLine("PrintingStyle.DoubleWidth", ThermalPrinter.PrintingStyle.DoubleWidth);
            printer.WriteLine("PrintingStyle.Reverse", ThermalPrinter.PrintingStyle.Reverse);
            printer.WriteLine("PrintingStyle.Underline", ThermalPrinter.PrintingStyle.Underline);
            printer.WriteLine("PrintingStyle.Updown", ThermalPrinter.PrintingStyle.Updown);
            printer.WriteLine("PrintingStyle.ThickUnderline", ThermalPrinter.PrintingStyle.ThickUnderline);
            printer.SetAlignCenter();
            printer.WriteLine("BIG TEXT!", ((byte)ThermalPrinter.PrintingStyle.Bold +
                (byte)ThermalPrinter.PrintingStyle.DoubleHeight +
                (byte)ThermalPrinter.PrintingStyle.DoubleWidth));
            printer.SetAlignLeft();
            printer.WriteLine("Default style again");

            printer.LineFeed(3);
            printer.Sleep();
            Console.WriteLine("Printer is now offline.");
            printerPort.Close();
        }

        static void CashRegister(ThermalPrinter printer, string item, int price)
        {
            printer.Reset();
            printer.Indent(0);

            if (item.Length > 24)
            {
                item = item.Substring(0, 23) + ".";
            }

            printer.WriteToBuffer(item.ToUpper());
            printer.Indent(25);
            string sPrice = String.Format("{0:0.00}", (double)price / 100);

            sPrice = sPrice.PadLeft(7);

            printer.WriteLine(sPrice);
            printer.Reset();
        }
        #endregion
    }
}
