using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WaterPositive.Kiosk.Data;

namespace WaterPositive.Kiosk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var services = new ServiceCollection();
            services.AddMudServices();
            services.AddSingleton<HubProcessorService>();
            services.AddSingleton<UserProfileService>();
            services.AddTransient<WaterDepotService>();
            services.AddTransient<CCTVService>();
            services.AddTransient<DataCounterService>();
            services.AddTransient<SensorDataService>();
            services.AddTransient<WaterUsageService>();
            services.AddTransient<UserProfileService>();
            services.AddTransient<WaterPriceService>();
            services.AddSingleton<AppState>();
            services.AddTransient<SerialDevice>();

            services.AddWindowsFormsBlazorWebView();
            blazorWebView1.HostPage = "wwwroot\\index.html";
            blazorWebView1.Services = services.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<App>("#app");
        }
    }
}