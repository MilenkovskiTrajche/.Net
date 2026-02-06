using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Web.Pages.Customer
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ICustomerRepository _customerRepository;

        public CreateModel(UserManager<IdentityUser> userManager,
                        SignInManager<IdentityUser> signInManager,
                        ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _customerRepository = customerRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

         public string City { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email};
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                var customer = new HotelManagement.Domain.Entities.Customer
                {
                    UserId = user.Id,
                    City = Input.City,
                    Type = CustomerType.Bronze,
                    ReservationCount = 0
                };
                await _customerRepository.AddAsync(customer);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
