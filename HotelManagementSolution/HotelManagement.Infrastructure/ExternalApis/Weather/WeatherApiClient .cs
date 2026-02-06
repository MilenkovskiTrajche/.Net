using HotelManagement.Domain.DTOs.Weather;
using HotelManagement.Application.Interfaces;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Infrastructure.ExternalApis.Weather
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _http;

        private const double LAT = 41.9981;
        private const double LON = 21.4254;

        public WeatherApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<WeatherDayDto>> GetForecastAsync(DateTime from, DateTime to)
        {
            var url =
                $"https://api.open-meteo.com/v1/forecast" +
                $"?latitude={LAT}&longitude={LON}" +
                $"&daily=weathercode,temperature_2m_max" +
                $"&timezone=Europe/Skopje";

            var response = await _http.GetFromJsonAsync<WeatherApiResponseDto>(url);

            if (response == null)
                return new();

            var result = new List<WeatherDayDto>();

            for (int i = 0; i < response.Daily.Time.Count; i++)
            {
                var date = DateTime.Parse(response.Daily.Time[i]);

                if (date < from.Date || date > to.Date)
                    continue;

                var code = response.Daily.Weathercode[i];
                var temp = response.Daily.Temperature_2m_max[i];

                result.Add(new WeatherDayDto
                {
                    Date = date,
                    WeatherCode = code,
                    Temperature = temp,
                    Emoji = WeatherEmojiMapper.Map(code)
                });
            }

            return result;
        }
    }
}
