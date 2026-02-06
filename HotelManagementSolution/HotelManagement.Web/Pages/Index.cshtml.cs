using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Application.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HotelService _hotelService;
        private readonly RoomService _roomService;

        public IndexModel(HotelService hotelService, RoomService roomService)
        {
            _hotelService = hotelService;
            _roomService = roomService;
        }

        public HotelManagement.Domain.Entities.Hotel? Hotel { get; set; }
        public List<HotelManagement.Domain.Entities.Room> Rooms { get; set; } = new List<HotelManagement.Domain.Entities.Room>();

        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public RoomType? RoomType { get; set; }

        public async Task OnGetAsync()
        {
            // Get the hotel info (assuming one hotel for simplicity)
            Hotel = await _hotelService.GetHotelAsync();

            // Get rooms
            var allRooms = await _roomService.GetAllRoomsAsync();

            // Filter by search
            if (!string.IsNullOrEmpty(SearchName))
                allRooms = allRooms.Where(r => r.Name.Contains(SearchName)).ToList();

            if (RoomType.HasValue)
                allRooms = allRooms.Where(r => r.Type == RoomType.Value).ToList();

            Rooms = (List<Domain.Entities.Room>)allRooms;
        }
    }
}
