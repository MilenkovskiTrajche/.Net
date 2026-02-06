using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.DTOs.Weather
{
    public class WeatherDayDto
    {
        public DateTime Date { get; set; }
        public int WeatherCode { get; set; }
        public double Temperature { get; set; }
        public string Emoji { get; set; } = "";
    }
}
