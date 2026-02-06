using HotelManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Interfaces
{
    public interface IReservationRepository
    {
        Task AddAsync(Reservation reservation);
        Task<List<Reservation>> GetByCustomerAsync(int customerId);

        Task<List<Reservation>> GetByRoomAsync(int customerId);
        Task<Reservation?> GetByIdAsync(int id);
        Task UpdateAsync(Reservation reservation);

        Task<List<Reservation>> GetAllAsync();
        Task<bool> AnyAsync(Expression<Func<Reservation, bool>> predicate);
    }
}
