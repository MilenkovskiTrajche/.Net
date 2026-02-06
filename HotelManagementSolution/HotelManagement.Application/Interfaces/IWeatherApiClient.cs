using HotelManagement.Domain.DTOs.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Interfaces
{
    public interface IWeatherApiClient
    {
        Task<List<WeatherDayDto>> GetForecastAsync(DateTime from, DateTime to);
    }
}
