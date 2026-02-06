using HotelManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;
        public int RoomId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int Adults { get; set; }
        public int Children { get; set; }

        public List<AdditionalService> AdditionalServices { get; set; }

        public decimal Price { get; set; }
        public bool IsCanceled { get; set; }
    }
}
