using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Review>> GetByRoomIdAsync(int roomId)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Where(r => r.RoomId == roomId)
                .ToListAsync();
        }

        public async Task<List<Review>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Reviews
                .Where(r => r.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<Review?> GetByReservationIdAsync(int reservationId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reservationId);
        }

        public Task<List<Review>> GetByCustomerIdAsync(string customerId)
        {
            throw new NotImplementedException();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}
