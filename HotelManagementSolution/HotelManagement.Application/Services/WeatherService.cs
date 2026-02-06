using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.DTOs.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherApiClient _client;

        public WeatherService(IWeatherApiClient client)
        {
            _client = client;
        }

        public Task<List<WeatherDayDto>> GetReservationForecast(DateTime from, DateTime to)
        {
            return _client.GetForecastAsync(from, to);
        }
    }
}
