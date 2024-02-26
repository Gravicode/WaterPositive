using System;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WaterPositive.StreamHub.Helpers
{
	public class DataSyncService
	{
		static string UrlInsertData = "https://waterpositive.my.id/api/WaterTankData/InsertData";
		static string UrlToken = "https://waterpositive.my.id/UserApi/authenticate";
		HttpClient client;
		DateTime LastGetToken = DateTime.MinValue;
		int IntervalMin = 5;
		string CurrentToken;
		public DataSyncService()
		{
			if (client == null) client = new();
        }
        public async Task<bool> SendData(SensorData item)
		{
			try
			{
                var ts = DateTime.Now - LastGetToken;
                if (ts.TotalMinutes >= IntervalMin)
                {
                    var token = await GetToken();
                    if (!string.IsNullOrEmpty(token))
                        SetToken(token);
                    CurrentToken = token;
                }
                if (string.IsNullOrEmpty(CurrentToken)) return false;
                var now = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
                if (item.TimeStamp < now)
                {
                    item.TimeStamp = DateTime.Now;
                }
                Dictionary<string, string> data = new();
                data.Add("Id", "0");
                data.Add("Nama", item.Name);
                data.Add("TanggalUpdate", item.TimeStamp.ToString("yyyy/MM/dd HH:mm:ss") ?? DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                data.Add("FlowIn", item.FlowIn.ToString("n2"));
                data.Add("FlowOut", item.FlowOut.ToString("n2"));

                using HttpContent formContent = new FormUrlEncodedContent(data);
                using HttpResponseMessage response = await client.PostAsync(UrlInsertData, formContent).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var res = await response.Content.ReadAsStringAsync();
                bool.TryParse(res, out var resVal);
                return resVal;
            }
			catch (Exception)
			{
                return false;
			}
			
        }

        void SetToken(string Token)
		{
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
			LastGetToken = DateTime.Now;
        }

        async Task<string?> GetToken()
		{
			var content = new { apiKey = "123qweasd" };
			var res = await client.PostAsync(UrlToken, new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json"));
			if (res.IsSuccessStatusCode)
			{
				var json =await res.Content.ReadAsStringAsync();
				var obj = JsonConvert.DeserializeObject<TokenResult>(json);
				return obj?.token;
			}
			return default;
		}
	}

    public class TokenResult
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public object password { get; set; }
        public string token { get; set; }
    }


}

