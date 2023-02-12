using Json.TinyCLR;
using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace WaterPositive.Device
{

    public class OutputCls
    {
     
        public bool Result { get; set; }
     
        public string Message { get; set; }
    
        public string Data { get; set; }
    }
    public class WaterUsageTemp
    {
        public double Volume { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        
    }
    public class SimulatedDevice
    {
        public float AppVer { get; set; } = 0.1f;
        WaterUsageTemp info = new();
        public double PricePerLiter { get; set; } = 6000; //Rp 6000/L
        public double FlowPerMinuteInMili { get; set; } = 1000 * 60; //60L
        Thread ThreadTimer;
        public bool IsOpen { get; set; } = false;
        SerialCom serial;
        public SimulatedDevice(SerialCom SerialDevice)
        {
            this.serial = SerialDevice;
        }
        public OutputCls GetState()
        {
            var output = new OutputCls() { Result = IsOpen };
            output.Message = $"Valve is {(IsOpen ? "Open":"Close")}";
            return output;
        }
        public OutputCls GetVersion()
        {
            var output = new OutputCls() { Result = true };
            output.Message = $"Simulated Device V{AppVer}";
            return output;
        }

        public OutputCls Open()
        {
            var output = new OutputCls() { Result = false };
            if (IsOpen)
            {
                output.Message = "OPENED";
                output.Data = "valve is already open";
                return output;
            }
           
            ThreadTimer = new Thread(new ThreadStart(StartCounting));
            ThreadTimer.Start();
            output.Message = "OPENED";
            output.Data = "valve is open";
            output.Result = true;
            return output;

        }
        public OutputCls Close()
        {
            var output = new OutputCls() { Result = false };
            if (!IsOpen)
            {
                output.Message = "CLOSED";
                output.Data = "valve is already close";
                return output;
            }
            IsOpen = false;
            Thread.Sleep(1000); //hold on a sec
            output.Message = "CLOSED";
            output.Data = "valve is closed";
            
            var result = JsonSerializer.SerializeObject(info);
            output.Data = result;
            output.Result = true;
            return output;

        }
        public OutputCls Reset()
        {
            var output = new OutputCls() { Result = false };
            if (IsOpen)
            {
                output.Message = "RESET";
                output.Data = "cannot reset when its opened";
                return output;
            }

            info.Start = DateTime.Now;
            info.End = DateTime.Now;
            info.Volume = 0;
            
            output.Message = "RESET";
            output.Data = "reset ok";

            var result = JsonSerializer.SerializeObject(info);
            output.Data = result;
            output.Result = true;
            return output;
        }
        void StartCounting()
        {
            info.Start = DateTime.Now;
            IsOpen = true;
            OutputCls output = new OutputCls() { Result=true, Message="FLOWING" };
            while (true)
            {
                if (!IsOpen) break;
                info.Volume += FlowPerMinuteInMili / 60;
                info.End = DateTime.Now;
                output.Data = JsonSerializer.SerializeObject(info);
                this.serial.WriteMessage(JsonSerializer.SerializeObject(output));
                Thread.Sleep(1000);
            }
        }
    }
}
