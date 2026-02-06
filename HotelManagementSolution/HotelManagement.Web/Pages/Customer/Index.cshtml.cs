using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Web.Pages.Customer
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly HotelManagement.Infrastructure.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(HotelManagement.Infrastructure.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();

        public class CustomerViewModel
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public CustomerType Type { get; set; }
            public int ReservationCount { get; set; }
        }

        public async Task OnGetAsync()
        {
            var customers = await _context.Customers.ToListAsync();

            Customers = new List<CustomerViewModel>();
            foreach (var c in customers)
            {
                var user = await _userManager.FindByIdAsync(c.UserId);
                Customers.Add(new CustomerViewModel
                {
                    Id = c.Id,
                    Username = user?.UserName ?? "Unknown",
                    City = c.City,
                    Type = c.Type,
                    ReservationCount = c.ReservationCount
                });
            }
        }
    }
}
