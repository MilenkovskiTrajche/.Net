using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Services
{
    public class ReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICustomerRepository _customerRepository;

        private static int CHILD_PRICE = 500;
        private static int SERVICE_PRICE = 200;

        public ReservationService(
            IReservationRepository reservationRepository,
            IRoomRepository roomRepository,
            ICustomerRepository customerRepository)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
            _customerRepository = customerRepository;
        }

        // 1. Create reservation
        public async Task<Reservation> CreateReservationAsync(
            int roomId,
            int customerId,
            int adults,
            int children,
            List<AdditionalService> additionalServices,
            DateTime FromDate,
            DateTime toDate)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null) throw new Exception("Room not found");

            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null) throw new Exception("Customer not found");

            // fixed prices for adults, children, services
            decimal price = 0;
            price += children * CHILD_PRICE;
            int days = (toDate - FromDate).Days;

            price = (room.PricePerNight * days) + price;

            int services_count = additionalServices.Count;
            services_count = (services_count * SERVICE_PRICE) * days;
            price += services_count;

            var reservation = new Reservation
            {
                RoomId = roomId,
                CustomerId = customerId,
                Adults = adults,
                Children = children,
                AdditionalServices = additionalServices,
                Price = price,
                IsCanceled = false,
                FromDate = FromDate,
                ToDate = toDate
            };

            await _reservationRepository.AddAsync(reservation);

            // Update customer type
            List<Reservation> reservations = await _reservationRepository.GetByCustomerAsync(customerId);

            var totalReservations = reservations.Count;
            if (totalReservations > 15) customer.Type = CustomerType.Gold;
            else if (totalReservations > 5) customer.Type = CustomerType.Silver;
            
            customer.ReservationCount = customer.ReservationCount + 1;

            await _customerRepository.UpdateAsync(customer);

            return reservation;
        }

        // 2. Cancel reservation
        public async Task<bool> CancelReservationAsync(int reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null) return false;

            if ((reservation.FromDate - DateTime.Now).TotalDays < 2)
                return false;

            reservation.IsCanceled = true;
            await _reservationRepository.UpdateAsync(reservation);

            return true;
        }

        // 3. Get customer reservations
        public async Task<IEnumerable<Reservation>> GetReservationsByCustomerAsync(int customerId)
        {
            return await _reservationRepository.GetByCustomerAsync(customerId);
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByRoomAsync(int id)
        {
            return await _reservationRepository.GetByRoomAsync(id);
        }
        public async Task<bool> UpdateReservationAsync(Reservation reservation)
        {
            var existing = await _reservationRepository.GetByIdAsync(reservation.Id);
            if (existing == null) return false;

            decimal price = 0;
            price += reservation.Children * CHILD_PRICE;
            int days = (reservation.ToDate - reservation.FromDate).Days;
            var room = await _roomRepository.GetByIdAsync(reservation.RoomId);
            if (room != null)
            {
                price = (room.PricePerNight * days) + price;
            }

            int services_count = reservation.AdditionalServices.Count;
            services_count = (services_count * SERVICE_PRICE) * days;
            price += services_count;

            // Update fields
            existing.Adults = reservation.Adults;
            existing.Children = reservation.Children;
            existing.FromDate = reservation.FromDate;
            existing.ToDate = reservation.ToDate;
            existing.AdditionalServices = reservation.AdditionalServices;
            existing.Price = price;

            await _reservationRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> IsDateRangeAvailableAsync(
                int roomId,
                DateTime fromDate,
                DateTime toDate,
                int? ignoreReservationId = null)
        {
            return !await _reservationRepository.AnyAsync(r =>
                r.RoomId == roomId &&
                !r.IsCanceled &&
                (ignoreReservationId == null || r.Id != ignoreReservationId) &&
                fromDate < r.ToDate &&
                toDate > r.FromDate
            );
        }


    }
}
