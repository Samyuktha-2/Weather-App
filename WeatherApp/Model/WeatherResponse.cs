using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Model
{
    public class WeatherResponse
    {
        public MainInfo Main { get; set; }
        public WindInfo Wind { get; set; }
        public List<WeatherInfo> Weather { get; set; }
        public int Visibility { get; set; }
        public class MainInfo
        {
            public double Temp { get; set; }
            public int Humidity { get; set; }
        }

        public class WindInfo
        {
            public double Speed { get; set; }
        }

        public class WeatherInfo
        {
            public string Main { get; set; }
            public string Description { get; set; }
        }
    }
}
