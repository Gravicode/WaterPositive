using Microsoft.Azure.Amqp.Framing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterPositive.Kiosk.Helpers
{
    public class AudioPlayer
    {
        System.Media.SoundPlayer player = new System.Media.SoundPlayer(Resource.click);
        public void Click()
        {
            //Use your own filename in place of _15035
            player.Play();
        }
    }
}
