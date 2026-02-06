using HotelManagement.Domain.DTOs.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Services
{
    public interface IWeatherService
    {
        Task<List<WeatherDayDto>> GetReservationForecast(DateTime from, DateTime to);
    }
}
