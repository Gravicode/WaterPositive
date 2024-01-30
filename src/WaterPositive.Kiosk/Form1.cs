using Blazored.Toast;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WaterPositive.Kiosk.Data;

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
            services.AddTransient<PrinterService>();
           
            services.AddWindowsFormsBlazorWebView();
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<App>("#app");
            Setup();
            DataPrepare();
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
        }
    }
}