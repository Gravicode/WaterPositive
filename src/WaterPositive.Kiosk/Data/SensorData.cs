using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
namespace WaterPositive.Kiosk.Data
{
    //public class SensorDataTemp
    //{
    //    public double Temp { get; set; }
    //    public double Light { get; set; }
    //    public double Pressure { get; set; }
    //    public DateTime TimeStamp { get; set; }
    //}
    public class DataSensorTower
    {
        public double Cm { get; set; }
        public double Ph { get; set; }
        public double Tds { get; set; }
        public override string ToString()
        {
            return $"torren:{Cm} tds:{Tds} ph:{Ph}";
        }
    }

    public class DataSensor
    {
        public double Flow { get; set; }
        public double Toren { get; set; }
        public double Ph { get; set; }
        public double Tds { get; set; }
        public override string ToString()
        {
            return $"flow:{Flow} torren:{Toren} tds:{Tds} ph:{Ph}";
        }
    }

    public class WaterSensorData
    {
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
        public bool Result { get; set; }
        public DataSensor Data { get; set; }
    }
    public class WaterSensorTowerData
    {
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
        public bool Result { get; set; }
        public DataSensorTower Data { get; set; }
    }

}


