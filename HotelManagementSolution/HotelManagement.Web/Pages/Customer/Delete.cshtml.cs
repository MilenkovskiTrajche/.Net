using HotelManagement.Application.Services;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
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
    public class DeleteModel : PageModel
    {
        private readonly HotelManagement.Infrastructure.Data.ApplicationDbContext _context;
        private readonly CustomerService _customerService;

        public DeleteModel(HotelManagement.Infrastructure.Data.ApplicationDbContext context, CustomerService customerService)
        {
            _customerService = customerService;
            _context = context;
        }

        [BindProperty]
        public HotelManagement.Domain.Entities.Customer Customer { get; set; } = default!;

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
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _customerService.DeleteCustomerAsync(id.Value);
            return RedirectToPage("./Index");
        }
    }
}
