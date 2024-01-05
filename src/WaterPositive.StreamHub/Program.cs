using WaterPositive.StreamHub.Helpers;

namespace WaterPositive.StreamHub;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<Worker>();
        builder.Services.AddSingleton<MqttService>();
        builder.Services.AddSingleton<DataSyncService>();

        var host = builder.Build();
        host.Run();
    }
}
