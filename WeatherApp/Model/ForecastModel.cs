using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Model
{
    public class ForecastModel
    {
        public string Day { get; set; } 
        public double Temperature { get; set; }
        public bool IsToday { get; set; }
        //public class DailyForecast {  }
    }
}
