using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class RoomImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;

        public int RoomId { get; set; }
        public Room Room { get; set; } = default!;
    }
}
