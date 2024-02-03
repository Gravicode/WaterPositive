using Blazored.Toast;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WaterPositive.Kiosk.Data;
using WaterPositive.Kiosk.Helpers;

namespace WaterPositive.Kiosk
{
    public partial class Form1 : Form
    {
        System.Timers.Timer SyncTimer;
        bool IsSync = false;
        SyncHelper sync;
        XbeeReceiverService xbeeReceiverService;
        
        public Form1()
        {
            
            InitializeComponent();

            //full screen and top most
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            //TopMost = true;

            var services = new ServiceCollection();
            services.AddBlazoredToast();
            services.AddMudServices();
            services.AddSingleton<HubProcessorService>();
            services.AddSingleton<UserProfileService>();
            services.AddTransient<WaterDepotService>();
            services.AddTransient<CCTVService>();
            services.AddTransient<DataCounterService>();
            services.AddTransient<SensorDataService>();
            services.AddTransient<WaterUsageService>();
            services.AddTransient<UsageLimitService>();
            services.AddTransient<UserProfileService>();
            services.AddTransient<WaterPriceService>();
            services.AddSingleton<AppState>();
            services.AddTransient<SerialDevice>();
            services.AddTransient<PosPrinterService>();
            services.AddTransient<BarcodeInterceptor>();
           
            services.AddWindowsFormsBlazorWebView();
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<App>("#app");
            Setup();
            DataPrepare();
        }
        private void GoFullscreen(bool fullscreen)
        {
            if (fullscreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }
        async void DataPrepare()
        {
            await sync.SyncData();
        }

        void Setup()
        {
            SyncTimer = new System.Timers.Timer(new TimeSpan(0, 5, 0));
            var db = new WaterPositiveDB(true);
            db.Database.EnsureCreated();
            var db_remote = new WaterPositiveDB(false);
            sync = new SyncHelper(db,db_remote);
            SyncTimer.Elapsed += async(a, b) => {
                if (IsSync) return;
                IsSync = true;
                var res = await sync.SyncData();
                IsSync = false;
            };
            SyncTimer.Start();
            this.xbeeReceiverService = new(new SensorDataService());
            
            //capture keyboard
            //Set up scanner key handling
            //this.KeyPreview = true;
            //this.KeyDown += new KeyEventHandler(KeyDownHandler);
            //this.KeyPress += Form1_KeyPress;
            
        }
        #region keyboard press / barcode handler
        private void Form1_KeyPress(object? sender, KeyPressEventArgs e)
        {
            Console.WriteLine(e.KeyChar);
            //throw new NotImplementedException();
        }

        private string outputString;
        public KeysConverter keysConverter = new KeysConverter();
        private void KeyDownHandler(object? sender, KeyEventArgs e)
        {
            //When return is pressed send string and start over
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                ScannerInputHandler(outputString);
                outputString = string.Empty;
                System.Windows.Forms.Application.DoEvents();
            }
            //Ignore these keycodes
            else if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.J || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.CapsLock)
            {

            }
            //Add to string
            else
            {
                string character = keysConverter.ConvertToString(e.KeyCode);
                outputString += character;
            }
        }
        private void ScannerInputHandler(string inputString)
        {
            
            if (inputString == "123")
            {
                MessageBox.Show("oke");
            }
           

            //Search for a number
            if (IsNumeric(inputString) == true)
            {
                //FindSelectedPartUsingTONumber(inputString);
            }
        }

        public static bool IsNumeric(string input)
        {
            int number;
            return int.TryParse(input, out number);
        }

        #endregion
    }
}
