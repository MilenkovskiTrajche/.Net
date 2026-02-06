using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.DTOs.Weather
{
    public class WeatherApiResponseDto
    {
        public DailyWeatherDto Daily { get; set; } = new();
    }

    public class DailyWeatherDto
    {
        public List<string> Time { get; set; } = new();
        public List<int> Weathercode { get; set; } = new();
        public List<double> Temperature_2m_max { get; set; } = new();
    }
}
