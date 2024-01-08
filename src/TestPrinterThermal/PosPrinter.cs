﻿using ESC_POS_USB_NET.Enums;
using ESC_POS_USB_NET.Printer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPrinterThermal
{
    public class PosPrinter
    {
        public PosPrinter()
        {
            
        }

        public void TestPrint()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Printer printer = new Printer("POS58 Printer");
            printer.TestPrinter();
            printer.FullPaperCut();
            printer.PrintDocument();
        }

        void PrintImage()
        {
            Printer printer = new Printer("Printer Name");
            Bitmap image = new Bitmap(Bitmap.FromFile("Icon.bmp"));
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
