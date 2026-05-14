using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.ViewModel;

namespace WeatherApp.Model
{
    public class ForecastModel  : BaseVM
    {
        private double _temperature;

        public string Day { get; set; }
        public string Icon { get; set; }
        public double Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }
    }
}
