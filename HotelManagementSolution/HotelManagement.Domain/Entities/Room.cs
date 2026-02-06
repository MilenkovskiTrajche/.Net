using HotelManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public string Description { get; set; }

        public int PricePerNight { get; set; }

        public List<RoomImage> Images { get; set; } = new List<RoomImage>();

        public int Capacity { get; set; }

        public List<RoomAmenities> RoomAmenities { get; set; } = new List<RoomAmenities>();
    }
}
