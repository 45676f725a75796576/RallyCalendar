using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RallyCalendar
{
    internal class Weather
    {
        private string weather = "";
        public string WeatherString { get { return weather; } }
        public Weather() { GetWeather(); }
        public async Task GetWeather()
        {
            string location = "Prague";
            string url = $"https://wttr.in/{location}?format=3";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    weather = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching weather: {ex.Message}");
                }
            }
        }
    }
}
