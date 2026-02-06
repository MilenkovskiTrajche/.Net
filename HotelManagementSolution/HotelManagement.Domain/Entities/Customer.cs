using HotelManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }
        public required string UserId { get; set; } // ASP.NET Identity
        public required string City { get; set; }
        public CustomerType Type { get; set; } = CustomerType.Bronze;
        public int ReservationCount { get; set; }
    }
}
