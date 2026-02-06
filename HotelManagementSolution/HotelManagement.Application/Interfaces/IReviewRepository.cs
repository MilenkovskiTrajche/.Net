using HotelManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Interfaces
{
    public interface IReviewRepository
    {

        Task<Review?> GetByIdAsync(int id);
        Task AddAsync(Review review);
        Task<List<Review>> GetByRoomIdAsync(int roomId);

        Task<List<Review>> GetByCustomerIdAsync(string customerId);
        Task DeleteAsync(int id);

        Task<Review?> GetByReservationIdAsync(int reservationId);

        Task UpdateAsync(Review review);

    }
}
