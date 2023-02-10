using GHIElectronics.TinyCLR.Devices.Uart;
using GHIElectronics.TinyCLR.Pins;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace WaterPositive.Device
{
    public class MessageSerial
    {
        public DateTime EventDate { get; set; }
        public string Message { get; set; }
    }
    public class SerialCom
    {
        public delegate void MessageReceived(MessageSerial message);

        public event MessageReceived MessageReceive;

        UartController myUart;
        public string ComPort { get; set; }
        public SerialCom(string COM = SC20260.UartPort.Uart1)
        {
            this.ComPort = COM;
            Setup();
        }

        public void WriteMessage(string Message)
        {
            var txBuffer = Encoding.UTF8.GetBytes(Message);
            myUart.Write(txBuffer, 0, txBuffer.Length);

        }
        void Setup()
        {           
            myUart = UartController.FromName(ComPort);
            var uartSetting = new UartSetting()
            {
                BaudRate = 115200,
                DataBits = 8,
                Parity = UartParity.None,
                StopBits = UartStopBitCount.One,
                Handshaking = UartHandshake.None,
            };
            myUart.SetActiveSettings(uartSetting);
            myUart.Enable();
            myUart.DataReceived += MyUart_DataReceived;
          

        }
        byte[] rxBuffer;
        void MyUart_DataReceived(UartController sender, DataReceivedEventArgs e)
        {
            rxBuffer = new byte[e.Count];
            var bytesReceived = myUart.Read(rxBuffer, 0, e.Count);
            var msg = Encoding.UTF8.GetString(rxBuffer, 0, bytesReceived);
            MessageReceive?.Invoke(new MessageSerial() { EventDate = DateTime.Now, Message = msg });
            Debug.WriteLine(msg);
        }
    }
}
