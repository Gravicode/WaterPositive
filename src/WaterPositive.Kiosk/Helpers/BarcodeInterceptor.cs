using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WaterPositive.Kiosk.Helpers
{
    public class BarcodeScanArgs:EventArgs
    {
        public string Barcode { get; set; }
    }
    public class BarcodeInterceptor
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private LowLevelKeyboardProc _proc;// = HookCallback;
        private  IntPtr _hookID = IntPtr.Zero;
         string barcode=string.Empty;
        public  EventHandler<BarcodeScanArgs> BarcodeScanned;
         KeysConverter kc = new KeysConverter();
        
        public  void SetHook()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
            
        }
        public  void UnHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private  IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var key = (Keys)vkCode;
                Console.WriteLine(key);
                var keystroke = kc.ConvertToString(vkCode);
                if (IsNumeric(keystroke))
                {
                    barcode += keystroke;
                }
                if(key == Keys.Enter)
                {
                    BarcodeScanned?.Invoke(this, new BarcodeScanArgs() { Barcode = barcode });
                    barcode = string.Empty;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
         bool IsNumeric(string input)
        {
            int number;
            return int.TryParse(input, out number);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
