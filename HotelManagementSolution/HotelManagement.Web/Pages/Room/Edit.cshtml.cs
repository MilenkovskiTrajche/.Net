using HotelManagement.Application.Services;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using HotelManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManagement.Web.Pages.Room
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly HotelManagement.Infrastructure.Data.ApplicationDbContext _context;
        private readonly RoomService _roomService;
        private readonly IWebHostEnvironment _env;

        public EditModel(HotelManagement.Infrastructure.Data.ApplicationDbContext context, RoomService roomService, IWebHostEnvironment env)
        {
            _roomService = roomService;
            _env = env;
            _context = context;
        }

        [BindProperty]
        public HotelManagement.Domain.Entities.Room Room { get; set; } = default!;

        [BindProperty]
        public List<IFormFile> UploadImages { get; set; } = new List<IFormFile>();

        [BindProperty]
        public List<RoomAmenities> RoomAmenities { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }
            Room = room;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Ensure the upload folder exists
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // Load the room from the database including its existing images
            var trackedRoom = await _context.Rooms
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == Room.Id);

            if (trackedRoom == null)
                return NotFound();

            // Update basic room properties
            trackedRoom.Name = Room.Name;
            trackedRoom.Description = Room.Description;
            trackedRoom.Type = Room.Type;

            // Upload new images
            foreach (var file in UploadImages)
            {
                if (file.Length > 0)
                {
                    // Sanitize file name
                    var fileName = Path.GetFileName(file.FileName).Replace(" ", "_");
                    var filePath = Path.Combine(uploadFolder, fileName);

                    // Save file to wwwroot/uploads
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    // Add new RoomImage to the tracked room
                    trackedRoom.Images.Add(new RoomImage
                    {
                        Url = "/uploads/" + fileName
                    });
                }
            }
            var amenities = RoomAmenities?
                .Select(s => Enum.Parse<RoomAmenities>(s.ToString()))
                .ToList() ?? new List<RoomAmenities>();

            trackedRoom.RoomAmenities = amenities;

            // Save all changes to the database
            await _context.SaveChangesAsync();

            // Redirect back to the index page
            return RedirectToPage("./Index");
        }


        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
