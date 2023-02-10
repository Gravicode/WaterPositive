using static MudBlazor.CategoryTypes;
using System.Diagnostics;
using System.Text;
using Microsoft.ServiceBus.Messaging;
//using Microsoft.ServiceBus.Messaging;

namespace WaterPositive.Kiosk.Data
{
    public class MessageReceiveArgs:EventArgs
    {
        public string DeviceId { get; set; }
        public string Message { get; set; }
    }
    public class HubProcessorService
    {
        public EventHandler<MessageReceiveArgs> MessageReceived;
        static string iotHubD2cEndpoint = "messages/events";
        EventHubClient eventHubClient;
        private async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                var connectionDeviceId = eventData.SystemProperties["iothub-connection-device-id"].ToString();
                Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
                MessageReceived?.Invoke(this, new MessageReceiveArgs() { DeviceId = connectionDeviceId, Message = data });
            }
        }
        CancellationTokenSource cts;
        public void Cancel()
        {
            if (cts == null) return;
            cts.Cancel();
        }
        public async Task Start(string selectedDevice, string activeIoTHubConnectionString, DateTime startTime, string consumerGroupName)
        {
            try
            {
                
                Console.WriteLine("Receive messages. Ctrl-C to exit.\n");
                eventHubClient = EventHubClient.CreateFromConnectionString(activeIoTHubConnectionString, iotHubD2cEndpoint);

                var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

                cts = new CancellationTokenSource();


                var tasks = new List<Task>();
                foreach (string partition in d2cPartitions)
                {
                    tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
                }
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                cts = null;
                Console.WriteLine(ex);
            }
           
        }
        
    }
}
