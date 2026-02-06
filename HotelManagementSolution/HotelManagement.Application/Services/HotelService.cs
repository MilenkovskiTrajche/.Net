using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Application.Services
{
    public class HotelService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        public async Task<Hotel?> GetHotelAsync()
        {
            return await _hotelRepository.GetAsync();
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            var existing_hotel = await _hotelRepository.GetAsync();
            if (hotel == null || existing_hotel == null)
            {
                throw new InvalidOperationException("Hotel not found.");
            }
            existing_hotel.Name = hotel.Name;
            existing_hotel.Description = hotel.Description;
            existing_hotel.ImageUrl = hotel.ImageUrl;
            existing_hotel.Address = hotel.Address;
            existing_hotel.SocialNetworks = hotel.SocialNetworks;

            await _hotelRepository.UpdateAsync(existing_hotel);
        }
    }
}
