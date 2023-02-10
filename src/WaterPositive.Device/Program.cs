using GHIElectronics.TinyCLR.Pins;
using Json.TinyCLR;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WaterPositive.Device
{
    internal class Program
    {
        static SimulatedDevice device;
        static SerialCom serial;
        static void Main()
        {
          
            serial = new SerialCom();
            device = new SimulatedDevice(serial);
            serial.MessageReceive += (a) => {
                OutputCls res;
                Regex reg = new Regex("\n");
                var msg = reg.Replace(a.Message, "");
                switch (msg)
                {
                    case "OPEN":
                        res = device.Open();
                        serial.WriteMessage(JsonSerializer.SerializeObject(res));
                        break;
                    case "STATE":
                        res = device.GetState();
                        serial.WriteMessage(JsonSerializer.SerializeObject(res));
                        break;
                    case "CLOSE":
                        res = device.Close();
                        serial.WriteMessage(JsonSerializer.SerializeObject(res));
                        break;
                    case "VER":
                        res = device.GetVersion();
                        serial.WriteMessage(JsonSerializer.SerializeObject(res));
                        break;  
                    default:
                        res = new OutputCls() { Message="Unknown command.", Result=false };
                        serial.WriteMessage(JsonSerializer.SerializeObject(res));
                        break;
                }
            };
            Thread.Sleep(-1);
        }
    }
}
