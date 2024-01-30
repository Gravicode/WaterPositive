using WaterPositive.Kiosk.Data;
using WaterPositive.Models;
using System;
public class XbeeReceiverService
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
                WaterSensorData? res = JsonSerializer.Deserialize<WaterSensorData>(x.Data);
                if (res != null)
                {
                    //messages.Add($"[{DateTime.Now}] => ({res.Result}) : {(res.Data==null ? res.Message : res.Data)}");
                    switch (res.Message)
                    {
                        case "FLOWING":
                            //var msg = JsonSerializer.Deserialize<WaterUsage>(res.Data);
                            //var msg = JsonSerializer.Deserialize<DataSensor>(res.Data);
                            

                            break;
                    }
                    if (res.Data != null)
                            {
                              var newItem = new SensorData();
                              newItem.Data = res.Data;
                              newItem.Result = res.Result;
                              newItem.TimeStamp = res.TimeStamp;
                              var hasil = this.service.InsertData(newItem);
                              System.Console.WriteLine("insert data: "+hasil);
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
                Console.WriteLine("read sensor failed: "+ex);
            }
        };
       }
    }