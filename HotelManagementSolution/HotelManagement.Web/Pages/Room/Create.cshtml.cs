using HotelManagement.Application.Interfaces;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Web.Pages.Room
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IWebHostEnvironment _env;  // ✅ inject this

        [BindProperty]
        public HotelManagement.Domain.Entities.Room Room { get; set; } = new HotelManagement.Domain.Entities.Room();

        [BindProperty]
        public List<IFormFile> UploadImages { get; set; } = new List<IFormFile>();

        [BindProperty]
        public List<RoomAmenities> RoomAmenities { get; set; } = new();

        public CreateModel(IRoomRepository roomRepository, IWebHostEnvironment env)
        {
            _roomRepository = roomRepository;
            _env = env ?? throw new ArgumentNullException(nameof(env)); // ✅ ensures not null
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // ✅ this now works
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            foreach (var file in UploadImages)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(uploadFolder, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    Room.Images.Add(new RoomImage
                    {
                        Url = "/uploads/" + file.FileName
                    });
                }
            }

            var amenities = RoomAmenities?
                .Select(s => Enum.Parse<RoomAmenities>(s.ToString()))
                .ToList() ?? new List<RoomAmenities>();

            Room.RoomAmenities = amenities;

            Room.Capacity = Room.Type switch
            {
                Domain.Enums.RoomType.Single => 1,
                Domain.Enums.RoomType.Double => 2,
                Domain.Enums.RoomType.Triple => 4,
                _ => 1
            };


            await _roomRepository.AddAsync(Room);

            return RedirectToPage("./Index");
        }
    }
}
