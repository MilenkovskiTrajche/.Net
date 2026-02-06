using HotelManagement.Application.Services;
using HotelManagement.Domain.DTOs.Weather;
using HotelManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Web.Pages.Reservation
{
    public class IndexModel : PageModel
    {
        private readonly ReservationService _reservationService;
        private readonly RoomService _roomService;
        private readonly ReviewService _reviewService;
        private readonly CustomerService _customerService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWeatherService _weatherService;

        public IndexModel(
            ReservationService reservationService,
            RoomService roomService,
            ReviewService reviewService,
            CustomerService customerService,
            UserManager<IdentityUser> userManager,
            IWeatherService weatherService)
        {
            _reservationService = reservationService;
            _roomService = roomService;
            _reviewService = reviewService;
            _customerService = customerService;
            _userManager = userManager;
            _weatherService = weatherService;
        }

        public List<HotelManagement.Domain.Entities.Reservation> Reservations { get; set; } = new();
        public Dictionary<int, string> RoomNames { get; set; } = new();
        public Dictionary<int, bool> HasReview { get; set; } = new();
        public Dictionary<int, int?> ReviewRatings { get; set; } = new();
        public Dictionary<int, string?> ReviewDescription { get; set; } = new();
        public Dictionary<int, List<WeatherDayDto>> WeatherPerReservation { get; set; } = new();

        public List<HotelManagement.Domain.Entities.Reservation> PastReservations { get; set; } = new();
        public List<HotelManagement.Domain.Entities.Reservation> UpcomingReservations { get; set; } = new();
        public List<HotelManagement.Domain.Entities.Reservation> CanceledReservations { get; set; } = new();


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var customer = await _customerService.GetCustomerByIdAsync(user.Id);
            if (customer == null) return RedirectToPage("/Account/Login");

            Reservations = (await _reservationService.GetReservationsByCustomerAsync(customer.Id))
                           .OrderBy(r => r.FromDate)
                           .ToList();

            // Populate lookup dictionaries
            foreach (var r in Reservations)
            {
                var room = await _roomService.GetRoomByIdAsync(r.RoomId);
                RoomNames[r.Id] = room?.Name ?? "Unknown room";

                var review = await _reviewService.GetReviewByRoomAndCustomerAsync(r.RoomId, customer.Id);
                HasReview[r.Id] = review != null;
                ReviewRatings[r.Id] = review?.Rate;
                ReviewDescription[r.Id] = review?.Description;
            }
            PastReservations = Reservations
                .Where(r => !r.IsCanceled && r.ToDate < DateTime.UtcNow)
                .OrderByDescending(r => r.ToDate)
                .ToList();

            UpcomingReservations = Reservations
                .Where(r => !r.IsCanceled && r.ToDate >= DateTime.UtcNow)
                .OrderBy(r => r.FromDate)
                .ToList();

            //weather data for upcoming reservations
            foreach (var r in UpcomingReservations)
            {
                var forecast = await _weatherService.GetReservationForecast(r.FromDate, r.ToDate);
                WeatherPerReservation[r.Id] = forecast;
            }

            CanceledReservations = Reservations
                .Where(r => r.IsCanceled)
                .OrderByDescending(r => r.FromDate)
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var customer = await _customerService.GetCustomerByIdAsync(user.Id);
            if (customer == null) return RedirectToPage("/Account/Login");

            var success = await _reservationService.CancelReservationAsync(id);

            if (success)
            {
                // Only update customer if cancellation succeeded
                customer.ReservationCount = Math.Max(0, customer.ReservationCount - 1);
                await _customerService.UpdateCustomerTypeAsync(user.Id);

                TempData["Success"] = "Reservation canceled successfully.";
            }
            else
            {
                TempData["Error"] = "Reservation cannot be canceled (less than 2 days to check-in).";
            }

            // Redirect back to the same page, ideally with route id
            return RedirectToPage(new { id = id });
        }

        public async Task<IActionResult> OnPostDeleteReviewAsync(int reservationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var deleted = await _reviewService.DeleteByReservationAsync(reservationId);

            if (!deleted)
            {
                TempData["Error"] = "Unable to delete review.";
            }

            return RedirectToPage();
        }
    }
}
