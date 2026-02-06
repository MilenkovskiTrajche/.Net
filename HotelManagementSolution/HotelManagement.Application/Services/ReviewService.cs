using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly CustomerService _customerService;
        private readonly IReservationRepository _reserva;

        public ReviewService(IReviewRepository reviewRepository, CustomerService customerService, IReservationRepository reserva)
        {
            _reviewRepository = reviewRepository;
            _customerService = customerService;
            _reserva = reserva;
        }

        public async Task AddReviewAsync(Review review)
        {
            await _reviewRepository.AddAsync(review);
        }

        public async Task<IEnumerable<Review>> GetReviewsByRoomAsync(int roomId)
        {
            return await _reviewRepository.GetByRoomIdAsync(roomId);
        }

        public async Task<Review?> GetReviewByRoomAndCustomerAsync(int roomId, int customerId)
        {
            var reviews = await _reviewRepository.GetByRoomIdAsync(roomId);
            return reviews.FirstOrDefault(r => r.CustomerId == customerId);
        }

        public async Task<bool> DeleteByReservationAsync(int reservationId)
        {
            var reservation = await _reserva.GetByIdAsync(reservationId);

            if (reservation == null) return false;

            var review = await GetReviewByRoomAndCustomerAsync(reservation.RoomId, reservation.CustomerId);
            if (review == null) return false;

            await _reviewRepository.DeleteAsync(review.Id);
            return true;
        }

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _reviewRepository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateReviewAsync(Review review)
        {
            var existing = await _reviewRepository.GetByIdAsync(review.Id);
            if (existing == null) return false;

            // Update fields
            existing.Rate = review.Rate;
            existing.Description = review.Description;

            await _reviewRepository.UpdateAsync(existing);
            return true;
        }
    }
}
