using System;
namespace WaterPositive.StreamHub.Helpers
{
	public class SensorData
	{
		
        public string Name { set; get; }
        public DateTime TimeStamp { set; get; }
        public float FlowIn { set; get; }
        public float FlowOut { set; get; }

    }
}

