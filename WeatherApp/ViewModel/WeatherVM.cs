using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Model;
using System.Windows.Input;
using WeatherApp.Command;
using System.Configuration;
using System.Net.Http;
using WeatherApp.Model;
using Newtonsoft.Json;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows;

namespace WeatherApp.ViewModel
{
    public class WeatherVM : BaseVM
    {
        private string city;
        private double temperature;
        private string time;
        private string date;
        private string day;
        private string condition;
        private string displayCity;
        private int humidity;
        private string greeting;
        private double wind;

        public string City
        {
            get => city;
            set
            {
                city = value;
                OnPropertyChanged(nameof(City));
            }
        }
        public string DisplayCity
        {
            get => displayCity;
            set
            {
                displayCity = value;
                OnPropertyChanged(nameof(DisplayCity));
            }
        }
        public double Temperature
        {
            get => temperature;
            set
            {
                temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }
        public string Time
        {
            get => time;
            set
            {
                time = value;
                OnPropertyChanged(nameof(Time));
            }
        }
        public string Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
        public string Day
        {
            get => day;
            set
            {
                day = value;
                OnPropertyChanged(nameof(Day));
            }
        }
        public string Condition
        {
            get => condition;
            set
            {
                condition = value;
                OnPropertyChanged(nameof(Condition));
            }
        }
        public int Humidity
        {
            get => humidity;
            set
            {
                humidity = value;
                OnPropertyChanged(nameof(Humidity));
            }
        }
        public double Wind
        {
            get => wind;
            set
            {
                wind = value;
                OnPropertyChanged(nameof(Wind));
            }
        }
        public string Greeting
        {
            get => greeting;
            set
            {
                greeting = value;
                OnPropertyChanged(nameof(Greeting));
            }
        }

        public ObservableCollection<ForecastModel> DailyData { get; set; } = new ObservableCollection<ForecastModel>();

        public ICommand SearchCommand { get; }

        public WeatherVM()
        {

            DisplayCity = "Coimbatore";
            SearchCommand = new RelayCommand(async () => await GetWeather());
            _ = GetWeather("Coimbatore");

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                var now = DateTime.Now;

                Time = now.ToString("hh:mm tt");
                Date = DateTime.Now.ToString("dd/MM/yyyy");
                Day = DateTime.Now.ToString("dddd");
                Greeting = GetGreeting(now.Hour);
            };

            timer.Start();
        }

        private string GetGreeting(int hour)
        {
            if (hour >= 5 && hour < 12) return "Good Morning!";
            if (hour >= 12 && hour < 17) return "Good Afternoon!";
            if (hour >= 17 && hour < 21) return "Good Evening!";
            return "Good Night!";
        }

        private async Task GetWeather(string cityName = null)
        {
            try
            {
                string apiKey = "c62f5ec5a571e9535a681638958e7a0b";
                string queryCity = string.IsNullOrWhiteSpace(cityName) ? City : cityName;

                string url = $"https://api.openweathermap.org/data/2.5/weather?q={queryCity}&appid={apiKey}&units=metric";

                using (HttpClient client = new HttpClient())
                {
                    var json = await client.GetStringAsync(url);

                    var data = JsonConvert.DeserializeObject<WeatherResponse>(json);

                    Temperature = data.Main.Temp;
                    Condition = data.Weather[0].Main;
                    Humidity = data.Main.Humidity;
                    Wind = data.Wind.Speed;
                }

                DisplayCity = queryCity;
                await GetForecast(queryCity);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProcessDailyData(ForecastResponse data)
        {
            DailyData.Clear();

            var grouped = data.List
                .GroupBy(x => DateTime.Parse(x.Dt_txt).Date)
                .Take(5);

            foreach (var day in grouped)
            {
                var first = day.First();

                DailyData.Add(new ForecastModel
                {
                    Day = day.Key == DateTime.Now.Date ? "Today" : day.Key.ToString("ddd"),
                    Temperature = first.Main.Temp,
                    IsToday = day.Key == DateTime.Now.Date
                });
            }
        }

        private async Task GetForecast(string cityName)
        {
            try
            {
                string apiKey = "c62f5ec5a571e9535a681638958e7a0b";

                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={apiKey}&units=metric";

                using (HttpClient client = new HttpClient())
                {
                    var json = await client.GetStringAsync(url);

                    var data = JsonConvert.DeserializeObject<ForecastResponse>(json);

                    ProcessDailyData(data); // ✅ NOW CORRECT
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
//string apiKey = Environment.GetEnvironmentVariable("WeatherAPI", EnvironmentVariableTarget.User);
//string apiKey = Environment.GetEnvironmentVariable("WeatherAPI")
//string apiKey = ConfigurationManager.AppSettings["WeatherAPI"];
//dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(response);