using WaterPositive.StreamHub.Helpers;

namespace WaterPositive.StreamHub;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    MqttService mqtt;
    DataSyncService syncSvc;
    public Worker(ILogger<Worker> logger,MqttService mqtt,DataSyncService syncSvc)
    {
        this.syncSvc = syncSvc;
        this.mqtt = mqtt;
        
        this.mqtt.MessageReceived += async(object? a, MessageEventArgs b) => {
            if(b.Topic == mqtt.DataTopic)
            {
                var fields = b.Message.Split('|').Select(x=> {
                    var vars = x.Split(':');
                    return new KeyValuePair<string, string>(vars[0].Replace(" ",string.Empty), vars[1].Replace(" ", string.Empty));
                } );
                var item = new SensorData();
                if (fields.Any())
                {
                    DateTime.TryParse( fields.Where(x => x.Key.Contains("Date")).FirstOrDefault().Value, out var timestamp);
                    item.TimeStamp = timestamp;
                    float.TryParse(fields.Where(x => x.Key.Contains("FlowIn")).FirstOrDefault().Value, out var flowin);
                    item.FlowIn = flowin;
                    float.TryParse(fields.Where(x => x.Key.Contains("FlowOut")).FirstOrDefault().Value, out var flowout);
                    item.FlowOut = flowout;
                    item.Name = fields.Where(x => x.Key.Contains("Device")).FirstOrDefault().Value;

                    var res = await syncSvc.SendData(item);
                    Console.WriteLine($"inserted to db: {res}");
                }
                //"Date/Time :5-1-2024 10:41:6 | Device:AWS001 | Flow In:58696.12 | Flow Out:13224.24"
            }
            Console.WriteLine(b.Topic);
            Console.WriteLine(b.Message);
        };
        this.mqtt.IsSubscribed = true;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}

