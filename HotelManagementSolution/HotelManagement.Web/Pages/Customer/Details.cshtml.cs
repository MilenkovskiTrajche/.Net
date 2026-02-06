using HotelManagement.Domain.Entities;
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
    public class DetailsModel : PageModel
    {
        private readonly HotelManagement.Infrastructure.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetailsModel(ApplicationDbContext context,
                         UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public HotelManagement.Domain.Entities.Customer Customer { get; set; } = default!;
        public string Username { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            else
            {
                Customer = customer;
                var user = await _userManager.FindByIdAsync(Customer.UserId);
                Username = user?.UserName ?? "Unknown";
            }
            return Page();
        }
    }
}
