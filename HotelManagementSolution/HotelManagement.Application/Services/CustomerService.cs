using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerService(
            ICustomerRepository customerRepository,
            IReservationRepository reservationRepository,
            UserManager<IdentityUser> userManager)
        {
            _customerRepository = customerRepository;
            _reservationRepository = reservationRepository;
            _userManager = userManager;
        }

        // Get customer by Id
        public async Task<Customer?> GetCustomerByIdAsync(string customerId)
        {
            return await _customerRepository.GetByUserIdAsync(customerId);
        }

        // Update customer type based on total reservations
        public async Task UpdateCustomerTypeAsync(string customerId)
        {
            var customer = await _customerRepository.GetByUserIdAsync(customerId);
            if (customer == null) return;

            var reservations_number = await _reservationRepository.GetByCustomerAsync(customer.Id);
            var totalReservations = reservations_number.Count();

            if (totalReservations > 15) customer.Type = CustomerType.Gold;
            else if (totalReservations > 5) customer.Type = CustomerType.Silver;
            else customer.Type = CustomerType.Bronze;

            await _customerRepository.UpdateAsync(customer);
        }

        // Optional: Get all reservations of a customer
        public async Task<IEnumerable<Reservation>> GetCustomerReservationsAsync(string customerId)
        {
            var customer = await _customerRepository.GetByUserIdAsync(customerId);
            if (customer == null) return Enumerable.Empty<Reservation>();

            var reservations = _reservationRepository.GetByCustomerAsync(customer.Id);
            if (reservations == null)
                return Enumerable.Empty<Reservation>();

            return await reservations;
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null) return false;

            // Delete linked Identity user
            var identityUser = await _userManager.FindByIdAsync(customer.UserId);
            if (identityUser != null)
            {
                var result = await _userManager.DeleteAsync(identityUser);
                if (!result.Succeeded)
                {
                    // optional: log errors
                    return false;
                }
            }

            // Delete customer entity
            await _customerRepository.DeleteAsync(customerId);

            return true;
        }
    }
}
