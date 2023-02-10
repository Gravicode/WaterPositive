using GHIElectronics.TinyCLR.Pins;
using Json.TinyCLR;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace WaterPositive.Device
{
    internal class Program
    {
        static SimulatedDevice device;
        static SerialCom serial;
        static void Main()
        {
            device = new SimulatedDevice();
            serial = new SerialCom(SC20260.UartPort.Uart1);
            serial.MessageReceive += (a) => {
                OutputCls res;
                switch (a.Message)
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
