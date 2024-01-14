using System.IO.Ports;
using System.Timers;

namespace QRScannerSerial
{
    public class QRData : EventArgs
    {
        public string QRCode { get; set; }
    }
    public class QRScanner : IDisposable
    {
        public EventHandler<QRData> QRDataReceived;
        private static SerialPort currentPort = new SerialPort();
        private static System.Timers.Timer aTimer;

        private delegate void updateDelegate(string txt);
        public QRScanner()
        {
        }

        public void Start(string ComPort = "COM11", int BaudRate = 9600)
        {
            if (currentPort.IsOpen)
            {
                currentPort.Close();
            }
            currentPort.PortName = ComPort;
            currentPort.BaudRate = BaudRate;
            currentPort.ReadTimeout = 1000;

            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        public void Stop()
        {
            aTimer.Stop();
            aTimer.Dispose();
            currentPort.Close();
        }
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!currentPort.IsOpen)
            {
                currentPort.Open();
                System.Threading.Thread.Sleep(100); /// for recieve all data from scaner to buffer
                currentPort.DiscardInBuffer();      /// clear buffer          
            }
            try
            {
                string strFromPort = currentPort.ReadExisting();
                updateTextBox(strFromPort);
            }
            catch { }
        }

        private void updateTextBox(string txt)
        {
            if (txt.Length > 0)
            {
              
                QRDataReceived?.Invoke(this, new QRData() { QRCode = txt });
            }
           
        }



        public void Dispose()
        {
            if (currentPort.IsOpen)
                currentPort.Close();
        }
    }
}
