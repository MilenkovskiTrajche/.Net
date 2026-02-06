using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }= 0;

        public Customer Customer { get; set; } = null!;
        public int RoomId { get; set; }

        public int Rate { get; set; } // 1–5
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
