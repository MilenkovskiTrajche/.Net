using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<Reservation, bool>> predicate)
        {
            return await _context.Reservations.AnyAsync(predicate);
        }

        public async Task<List<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public async Task<List<Reservation>> GetByCustomerAsync(int customerId)
        {
            return await _context.Reservations
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.FromDate)
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public async Task<List<Reservation>> GetByRoomAsync(int roomId)
        {
            return await _context.Reservations
                .Where(r => r.RoomId == roomId && r.IsCanceled==false)
                .OrderByDescending(r => r.FromDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            if (!_context.Reservations.Local.Any(r => r.Id == reservation.Id))
                _context.Reservations.Attach(reservation);

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }
    }
}
