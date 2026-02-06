using HotelManagement.Application.Services;
using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelManagement.Web.Pages
{
    public class RoomViewModel : PageModel
    {
        private readonly RoomService _roomService;
        private readonly ReservationService _reservationService;
        private readonly ReviewService _reviewService;
        private readonly CustomerService _customerService;
        private readonly UserManager<IdentityUser> _userManager;

        public RoomViewModel(
            RoomService roomService,
            ReservationService reservationService,
            ReviewService reviewService,
            CustomerService customerService,
            UserManager<IdentityUser> userManager)
        {
            _roomService = roomService;
            _reservationService = reservationService;
            _reviewService = reviewService;
            _customerService = customerService;
            _userManager = userManager;
        }

        // ===== DATA FOR PAGE =====

        public HotelManagement.Domain.Entities.Room Room { get; set; } = default!;
        
        public HotelManagement.Domain.Entities.Customer Customer { get; set; } = default!;


        [BindProperty(SupportsGet = true)]
        public int? EditReviewId { get; set; } // review ID to edit

        public Review? EditReview { get; set; } // the actual review object

        public List<Review> Reviews { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public bool IsEdit { get; set; } = false;

        // ===== FORM BINDINGS =====

        [BindProperty]
        public int Adults { get; set; } = 1;

        [BindProperty]
        public int Children { get; set; } = 0;

        [BindProperty]
        public List<AdditionalService> AdditionalServices { get; set; } = new();

        [BindProperty]
        public DateTime FromDate { get; set; }

        [BindProperty]
        public DateTime ToDate { get; set; }

        [BindProperty]
        public int NewReviewRate { get; set; } = 5;

        [BindProperty]
        public string NewReviewDescription { get; set; } = "/";

        [BindProperty]
        public bool CanLeaveReview { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public bool ShowReviewForm { get; set; } = false;

        [BindProperty]
        public int RoomId { get; set; }

        [BindProperty]
        public int Capacity { get; set; }

        [BindProperty]
        public List<DateTime> BookedDates { get; set; } = new();

        // ===== GET =====

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Room = await _roomService.GetRoomByIdAsync(id);
            if (Room == null)
                return NotFound();
            Reviews = (await _reviewService.GetReviewsByRoomAsync(id)).ToList();
            Capacity = Room.Capacity;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Page(); // not logged in

            Customer = await _customerService.GetCustomerByIdAsync(user.Id);
            if (Customer == null)
                return BadRequest("Customer profile not found.");


            if (ShowReviewForm)
            {
                CanLeaveReview = true;
            }

            if (EditReviewId.HasValue)
            {
                EditReview = await _reviewService.GetReviewByIdAsync(EditReviewId.Value);
                if (EditReview != null)
                {
                    // Pre-fill form fields
                    NewReviewRate = EditReview.Rate;
                    NewReviewDescription = EditReview.Description;
                    RoomId = EditReview.RoomId;
                }
            }

            if (IsEdit)
            {
                var reservations = await _reservationService
                    .GetReservationsByCustomerAsync(Customer.Id);

                var reservation = reservations
                    .FirstOrDefault(r => r.RoomId == Room.Id);

                if (reservation == null)
                    return RedirectToPage("/Reservation/Index");

                Adults = reservation.Adults;
                Children = reservation.Children;
                FromDate = reservation.FromDate;
                ToDate = reservation.ToDate;
                AdditionalServices = reservation.AdditionalServices;
            }
            var reservations_dates = await _reservationService.GetReservationsByRoomAsync(id);
            BookedDates = reservations_dates
                .Where(r => r.ToDate >= r.FromDate) // only valid reservations
                .SelectMany(r =>
                {
                    int days = (r.ToDate - r.FromDate).Days + 1;
                    if (days <= 0) return Enumerable.Empty<DateTime>();
                    return Enumerable.Range(0, days)
                                     .Select(offset => r.FromDate.AddDays(offset));
                })
                .ToList();
            return Page();
        }

        // ===== POST (BOOK) =====

        public async Task<IActionResult> OnPostBookAsync(int id)
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Account/Login", new { returnUrl = $"/RoomView?id={id}" });

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var customer = await _customerService.GetCustomerByIdAsync(user.Id);
            if (customer == null) return BadRequest("Customer profile not found.");

            // 2?? Validate dates
            if (ToDate <= FromDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
                await LoadRoomDataAsync(id);
                return Page();
            }

            // 3?? Force UTC (PostgreSQL safe)
            var fromUtc = DateTime.SpecifyKind(FromDate, DateTimeKind.Utc);
            var toUtc = DateTime.SpecifyKind(ToDate, DateTimeKind.Utc);

            // 4?? Convert services
            var services = AdditionalServices?
                .Select(s => Enum.Parse<AdditionalService>(s.ToString()))
                .ToList() ?? new List<AdditionalService>();

            // 5?? Get existing active reservation (same room)
            var existingReservation = (await _reservationService
                .GetReservationsByCustomerAsync(customer.Id))
                .FirstOrDefault(r =>
                    r.RoomId == id &&
                    !r.IsCanceled &&
                    r.FromDate > DateTime.UtcNow);

            // 6?? Check availability (ignore own reservation if updating)
            var available = await _reservationService.IsDateRangeAvailableAsync(
                roomId: id,
                fromDate: fromUtc,
                toDate: toUtc,
                ignoreReservationId: existingReservation?.Id
            );

            if (!available)
            {
                ModelState.AddModelError("", "Selected dates are already booked.");
                await LoadRoomDataAsync(id);
                return Page();
            }

            // 7?? Update OR create
            if (existingReservation != null)
            {
                existingReservation.Adults = Adults;
                existingReservation.Children = Children;
                existingReservation.AdditionalServices = services;
                existingReservation.FromDate = fromUtc;
                existingReservation.ToDate = toUtc;

                await _reservationService.UpdateReservationAsync(existingReservation);
            }
            else
            {
                await _reservationService.CreateReservationAsync(
                    roomId: id,
                    customerId: customer.Id,
                    adults: Adults,
                    children: Children,
                    additionalServices: services,
                    FromDate: fromUtc,
                    toDate: toUtc
                );
            }

            // 8?? Redirect on success
            return RedirectToPage("/Reservation/Index");
        }

        private async Task LoadRoomDataAsync(int roomId)
        {
            Room = await _roomService.GetRoomByIdAsync(roomId);
            Reviews = (await _reviewService.GetReviewsByRoomAsync(roomId)).ToList();

            var reservations = await _reservationService.GetReservationsByRoomAsync(roomId);

            BookedDates = reservations
                .Where(r => !r.IsCanceled && r.ToDate >= r.FromDate)
                .SelectMany(r =>
                    Enumerable.Range(0, (r.ToDate - r.FromDate).Days + 1)
                              .Select(offset => r.FromDate.AddDays(offset))
                )
                .ToList();
        }

        public async Task<IActionResult> OnPostAddReviewAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return RedirectToPage("/Account/Login");

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var customer = await _customerService.GetCustomerByIdAsync(user.Id);
            if (customer == null)
                return BadRequest("Customer profile not found.");

            var review = new Review
            {
                RoomId = RoomId,
                CustomerId = customer.Id,
                Rate = NewReviewRate,
                Description = NewReviewDescription,
                Customer = customer
            };

            await _reviewService.AddReviewAsync(review);

            return RedirectToPage(new { id = RoomId });
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id) // RoomId
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var customer = await _customerService.GetCustomerByIdAsync(user.Id);
            if (customer == null) return BadRequest("Customer not found.");

            var reservations = await _reservationService.GetReservationsByCustomerAsync(customer.Id);
            var reservation = reservations.FirstOrDefault(r => r.RoomId == id);

            if (reservation == null)
                return BadRequest("Reservation not found for this room.");

            // Convert date inputs to UTC
            reservation.Adults = Adults;
            reservation.Children = Children;
            reservation.FromDate = DateTime.SpecifyKind(FromDate, DateTimeKind.Utc);
            reservation.ToDate = DateTime.SpecifyKind(ToDate, DateTimeKind.Utc);
            reservation.AdditionalServices = AdditionalServices;

            var result = await _reservationService.UpdateReservationAsync(reservation);
            if (!result)
            {
                TempData["Error"] = "Failed to update reservation. Please try again.";
                return Page();
            }

            return RedirectToPage("/Reservation/Index");
        }

        public async Task<IActionResult> OnPostEditReviewAsync(int reviewId, int rate, string description)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");
            var customer = await _customerService.GetCustomerByIdAsync(user.Id);
            if (customer == null) return BadRequest("Customer not found.");

            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null) return NotFound();

            review.Rate = rate;
            review.Description = description;

            await _reviewService.UpdateReviewAsync(review);
            return RedirectToPage(new { id = review.RoomId });
        }

        public async Task<IActionResult> OnPostAddOrUpdateReviewAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Account/Login");

            var customer = await _customerService.GetCustomerByIdAsync(user.Id);
            if (customer == null) return BadRequest("Customer not found.");

            if (EditReviewId.HasValue)
            {
                //Update existing review
               var review = await _reviewService.GetReviewByIdAsync(EditReviewId.Value);
                if (review == null || review.CustomerId != customer.Id)
                    return BadRequest("Review not found or unauthorized.");

                review.Rate = NewReviewRate;
                review.Description = NewReviewDescription;
                await _reviewService.UpdateReviewAsync(review);
            }
            else
            {
                // Add new review
                var review = new Review
                {
                    RoomId = RoomId,
                    CustomerId = customer.Id,
                    Rate = NewReviewRate,
                    Description = NewReviewDescription
                };
        await _reviewService.AddReviewAsync(review);
    }

            return RedirectToPage(new { id = RoomId });
        }


    }
}
