using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterPositive.Models;

namespace WaterPositive.Kiosk.Data
{
    public class AppState
    {
        public event Action<bool> OnInternetChange;
        public  UserProfile CurrentUser { get; set; }
        public void RefreshInternet(bool State)
        {
            InternetStateChanged(State);
        }
        private void InternetStateChanged(bool state) => OnInternetChange?.Invoke(state);

    }
}
