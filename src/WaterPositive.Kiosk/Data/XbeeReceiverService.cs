using WaterPositive.Kiosk.Data;
using WaterPositive.Models;
using System;
using Newtonsoft.Json;
public class XbeeReceiverService:IDisposable
    {
        SerialDevice serial;
        SensorDataService service {set;get;}
        int ErrorCount{set;get;}
       public XbeeReceiverService(SensorDataService service)
       {
        this.ErrorCount=0;
        this.service = service;
        serial = new SerialDevice(AppConstants.XbeePort);
        serial.DataReceived += async (s, x) =>
        {
            try
            {
                WaterSensorTowerData? res = JsonConvert.DeserializeObject<WaterSensorTowerData>(x.Data);
                if (res != null)
                {
                    //messages.Add($"[{DateTime.Now}] => ({res.Result}) : {(res.Data==null ? res.Message : res.Data)}");
                    switch (res.Message)
                    {
                        case "AIRMONITORING":
                            //var msg = JsonSerializer.Deserialize<WaterUsage>(res.Data);
                            //var msg = JsonSerializer.Deserialize<DataSensor>(res.Data);


                            break;
                    }
                    if (res.Data != null)
                    {
                        var newItem = new SensorData();
                        newItem.WaterDepotId = AppConstants.WaterDepotId;
                        newItem.Tanggal = DateTime.Now;
                        newItem.Tds = res.Data.Tds;
                        newItem.WaterLevel = res.Data.Cm;
                        newItem.Ph = res.Data.Ph;
                        newItem.Temperature = res.Data.Temperature;
                        newItem.Altitude = res.Data.Altitude;
                        newItem.Pressure = res.Data.Pressure;
                        newItem.DeviceId = "WaterSensor001";
                        var hasil = this.service.InsertData(newItem);
                        System.Console.WriteLine("insert data: " + hasil);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCount++;
                if (ErrorCount > 3)
                {
                    //if (IsFlowing) await Close();
                    //AppConstants.NeedMaintenance = true;
                    //MessageBox.Show("Terjadi masalah pada alat, hubungi petugas, dan lakukan prosedur reset.", "Warning");
                }
                Console.WriteLine("read sensor failed: " + ex);
            }
        };
       }

    public void Dispose()
    {
        this.serial.Dispose();
    }
}