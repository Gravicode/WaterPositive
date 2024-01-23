using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermalDotNet;
using WaterPositive.Models;

namespace WaterPositive.Kiosk.Data
{
    public class PrinterService:IDisposable
    {
        public bool IsPrinterReady { get; set; }
        SerialPort printerPort  { set; get; }
        ThermalPrinter printer { set; get; }
        public PrinterService()
        {
            Setup(AppConstants.PrinterPort);
        }

        public void Dispose()
        {   
            printerPort.Close();
        }

        void Setup(string PrinterPort, int BaudRate=9600)
        {
            string printerPortName = PrinterPort;

            //Serial port init
            SerialPort printerPort = new SerialPort(printerPortName, BaudRate);

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
                IsPrinterReady = true;
            }
            catch
            {
                Console.WriteLine("I/O error");
                IsPrinterReady = false;
                //Environment.Exit(0);
            }

            if (IsPrinterReady)
            {
                //Printer init
                printer = new ThermalPrinter(printerPort);//, 7, 80, 2);
                printer.Reset();
                printer.WakeUp();
                printer.BoldOn();
                Console.WriteLine(printer.ToString());
            }

        }

        public void CetakReceipt(WaterUsage usage)
        {
            try
            {
                Dictionary<string, double> ItemList = new Dictionary<string, double>(100);
                printer.SetLineSpacing(0);
                printer.SetAlignCenter();
                printer.WriteLine("AWS WATER POSITIVE",
                    (byte)ThermalPrinter.PrintingStyle.DoubleHeight
                    + (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
                printer.WriteLine("-- Tanda Terima Pemakaian Air --");
                printer.WriteLine($"Tanggal/Waktu: {usage.UpdatedDate.ToString("dddd, dd-MMM-yyyy HH:mm")}");
                printer.WriteLine($"Nama: {usage.User?.FullName} - {usage.User?.KTP}");
                printer.LineFeed();
                printer.LineFeed();

                ItemList.Add("Volume (Liter)", usage.Volume);
                ItemList.Add("Harga (Rp)", usage.TotalHarga);

                //int total = 0;
                foreach (KeyValuePair<string, double> item in ItemList)
                {
                    CashRegister(printer, item.Key, item.Value);
                    //total += item.Value;
                }

                printer.HorizontalLine(32);

                //double dTotal = Convert.ToDouble(total) / 100;
                //double VAT = 10.0;

                printer.WriteLine(String.Format("{0:0.00}", (usage.TotalHarga)).PadLeft(32));

                //printer.WriteLine("VAT 10,0%" + String.Format("{0:0.00}", (dTotal * VAT / 100)).PadLeft(23));

                //printer.WriteLine(String.Format("$ {0:0.00}", dTotal * VAT / 100 + dTotal).PadLeft(16),
                //    ThermalPrinter.PrintingStyle.DoubleWidth);

                //printer.LineFeed();
                //printer.WriteLine("CASH" + String.Format("{0:0.00}", (double)total / 100).PadLeft(28));
                printer.LineFeed();
                printer.LineFeed();
                printer.SetAlignCenter();
                printer.WriteLine("Terima kasih, gunakan air dengan hemat.", ThermalPrinter.PrintingStyle.Bold);
                printer.LineFeed();
                printer.SetAlignLeft();
                printer.WriteLine($"Depot : {usage.WaterDepot?.Nama}");
                printer.LineFeed();
                printer.LineFeed();
                printer.LineFeed();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Printing error: {ex}");
            }
           
        }

        void CashRegister(ThermalPrinter printer, string item, double numValue)
        {
            printer.Reset();
            printer.Indent(0);

            if (item.Length > 24)
            {
                item = item.Substring(0, 23) + ".";
            }

            printer.WriteToBuffer(item.ToUpper());
            printer.Indent(25);
            string sValue = String.Format("{0:0.00}", (double)numValue);

            sValue = sValue.PadLeft(7);

            printer.WriteLine(sValue);
            printer.Reset();
        }
    }
}
