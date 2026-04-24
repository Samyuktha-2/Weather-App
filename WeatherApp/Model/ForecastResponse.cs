using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Model;
using static WeatherApp.Model.WeatherResponse;

namespace WeatherApp.Model
{
    public class ForecastResponse
    {
        public List<ForecastItem> List { get; set; }
    }
    s
    public class ForecastItem
    {
        public MainInfo Main { get; set; }
        public string Dt_txt { get; set; }
    }

}
