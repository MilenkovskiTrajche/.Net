using HotelManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByUserIdAsync(string userId);
        Task<Customer?> GetByIdAsync(int id);
        Task UpdateAsync(Customer customer);
        Task AddAsync(Customer customer);

        Task DeleteAsync(int customerId);
    }
}
