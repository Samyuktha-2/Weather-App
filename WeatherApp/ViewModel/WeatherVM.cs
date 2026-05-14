using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WeatherApp.Command;
using WeatherApp.Model;

namespace WeatherApp.ViewModel
{
    public class WeatherVM : BaseVM
    {
        private string city;
        private string displayCity;
        private double _temperature;
        private string _condition;
        private string _greet;
        private string _description;
        private int _humidity;
        private double _wind;
        private int _visibility;
        private bool _isCelsius = true;
        private string _humidityStat;
        private string _windStat;
        private string _visibilityStat;
        private bool _buttonVisibility = false;
        private string _icon;  

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
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
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }
        public string Condition
        {
            get => _condition;
            set
            {
                _condition = value;
                OnPropertyChanged(nameof(Condition));
            }
        }
        public string Descrpt
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Descrpt));
            }
        }
        public string Greet
        {
            get => _greet;
            set
            {
                _greet = value;
                OnPropertyChanged(nameof(Greet));
            }
        }

        public int Humidity
        {
            get => _humidity;
            set
            {
                _humidity = value;
                OnPropertyChanged(nameof(Humidity));
            }
        }
        public double Wind
        {
            get => _wind;
            set
            {
                _wind = value;
                OnPropertyChanged(nameof(Wind));
            }
        }
        public int Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        public bool IsCelsius
        {
            get => _isCelsius;
            set
            {
                _isCelsius = value;
                OnPropertyChanged(nameof(IsCelsius));
            }
        }
        public bool ButtonVisibility
        {
            get => _buttonVisibility;
            set
            {
                _buttonVisibility = value;
                OnPropertyChanged(nameof(ButtonVisibility));
            }
        }

        public string HumidityStat
        {
            get => _humidityStat;
            set
            {
                _humidityStat = value;
                OnPropertyChanged(nameof(HumidityStat));
            }
        }
        public string WindStat
        {
            get => _windStat;
            set
            {
                _windStat = value;
                OnPropertyChanged(nameof(WindStat));
            }
        }
        public string VisibilityStat
        {
            get => _visibilityStat;
            set
            {
                _visibilityStat = value;
                OnPropertyChanged(nameof(VisibilityStat));
            }
        }
        
        public ICommand SearchCommand { get; }
        public ICommand CelciusCommand { get; }
        public ICommand FarenheitCommand { get; }

        public ObservableCollection<ForecastModel> WeeklyForecasts { get; set; } = new ObservableCollection<ForecastModel>();

        public WeatherVM()
        {
            SearchCommand = new RelayCommand(() => _ = GetWeather());
            Greet = GetGreeting(DateTime.Now.Hour);

            CelciusCommand = new RelayCommand(InCelcius);
            FarenheitCommand = new RelayCommand(InFarenheit);
        }

        private async Task GetWeather()
        {
            string apiKey = "c62f5ec5a571e9535a681638958e7a0b";
            if (!IsValidCity(City))
            {
                MessageBox.Show("Enter city name");
                return;
            }
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={City}&appid={apiKey}&units=metric";
                using (HttpClient client = new HttpClient())
                {
                    var json = await client.GetStringAsync(url);

                    var data = JsonConvert.DeserializeObject<WeatherResponse>(json);

                    DisplayCity = City;
                    Temperature = data.Main.Temp;
                    Condition = data.Weather[0].Main;
                    Descrpt = data.Weather[0].Description;
                    Humidity = data.Main.Humidity;
                    Wind = data.Wind.Speed;
                    Visibility = data.Visibility;
                    Icon = GetWeatherIcon(Condition);
                }
            }
            catch
            {
                MessageBox.Show("City not found");
            }

            GetForecast();
            HumidityStatus();
            WindStatus();
            VisibilityStatus();
            ButtonVisibility = true;
        }

        private string GetGreeting(int hour)
        {
            string currentTime = DateTime.Now.ToString("HH:mm");
            if (hour >= 5 && hour < 12) return $"Morning {currentTime}am";
            if (hour >= 12 && hour < 17) return $"Afternoon {currentTime}pm";
            if (hour >= 17 && hour < 21) return $"Evening {currentTime}pm";
            return $"Night {currentTime}pm";
        }

        public async void GetForecast()
        {
            try
            {
                string apiKey = "c62f5ec5a571e9535a681638958e7a0b";

                string url = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&appid={apiKey}&units=metric";

                using (HttpClient client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);

                    JObject data = JObject.Parse(json);

                    WeeklyForecasts.Clear();

                    var addedDays = new HashSet<string>();
                    DateTime today = DateTime.Today;

                    foreach (var item in data["list"])
                    {
                        DateTime date =
                            DateTime.Parse(item["dt_txt"].ToString());

                        if (date.Date == today)
                            continue;

                        if (date.Hour != 12)
                            continue;

                        string day = date.ToString("ddd");

                        if (!addedDays.Contains(day))
                        {
                            addedDays.Add(day);
                            WeeklyForecasts.Add(new ForecastModel
                            {
                                Day = day,
                                Temperature = (int)Math.Round((double)item["main"]["temp"]),
                                Icon = GetWeatherIcon(item["weather"][0]["main"].ToString())
                            });
                        }

                        if (WeeklyForecasts.Count == 5)
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetWeatherIcon(string condition)
        {
            switch (condition)
            {
                case "Clouds":
                    return "☁";

                case "Rain":
                    return "🌧";

                case "Clear":
                    return "☀";

                case "Thunderstorm":
                    return "⛈";

                case "Snow":
                    return "❄";

                default:
                    return "☀";
            }
        }

        private void InCelcius()
        {
            if (!IsCelsius)
            {
                double c = (Temperature - 32) * 5 / 9;
                Temperature = c;

                foreach (var item in WeeklyForecasts)
                {
                    double c1 = (item.Temperature - 32) * 5 / 9;
                    item.Temperature = c1;
                }

                IsCelsius = true;
            }
        }

        private void InFarenheit()
        {
            if (IsCelsius)
            {
                double f = (Temperature * 9 / 5) + 32;
                Temperature = f;

                foreach (var item in WeeklyForecasts)
                {
                    double f1 = (item.Temperature * 9 / 5) + 32;
                    item.Temperature = f1;
                }

                IsCelsius = false;
            }
        }

        private void HumidityStatus()
        {
            if (Humidity < 30)
            {
                HumidityStat = "Low 😓";
                return;
            }
            if (Humidity <= 60)
            {
                HumidityStat = "Normal 👍";
                return;
            }
            HumidityStat = "High 💧";
            return;
        }

        private void WindStatus()
        {
            if (Wind < 10)
            {
                WindStat = "Light 🍃";
                return;
            }

            if (Wind <= 25)
            {
                WindStat = "Moderate 🌬";
                return;
            }
            WindStat = "Strong 💨";
            return;
        }

        private void VisibilityStatus()
        {
            if (Visibility < 2)
            {
                VisibilityStat = "Poor 😵";
                return;
            }

            if (Visibility <= 6)
            {
                VisibilityStat = "Average 🙂";
                return;
            }

            VisibilityStat = "Clear 😎";
        }

        private bool IsValidCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
                return false;

            return System.Text.RegularExpressions.Regex
                .IsMatch(city, @"^[a-zA-Z\s\-]+$");
        } 
    }
}