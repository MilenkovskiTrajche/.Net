using HotelManagement.Application.Interfaces;
using HotelManagement.Application.Services;
using HotelManagement.Domain.Entities;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.ExternalApis.Weather;
using HotelManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

builder.Services.AddRazorPages();

// Repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();

// Services
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<HotelService>();
builder.Services.AddScoped<CustomerService>();

//weather service
builder.Services.AddHttpClient<IWeatherApiClient, WeatherApiClient>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@hotel.com";
    string adminPassword = "Admin123!";

    // Create Admin role if not exists
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // Create default admin user
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    var uploadsPath = Path.Combine(env.WebRootPath, "uploads");
    Directory.CreateDirectory(uploadsPath);

    var seedPath = Path.Combine(env.ContentRootPath, "Data", "SeedImages");

    // If DB has image paths but files are missing ? recreate them
    foreach (var img in db.RoomImages.ToList())
    {
        var fileName = Path.GetFileName(img.Url);
        var targetPath = Path.Combine(uploadsPath, fileName);
        var sourcePath = Path.Combine(seedPath, fileName);

        if (!File.Exists(targetPath) && File.Exists(sourcePath))
        {
            File.Copy(sourcePath, targetPath, overwrite: true);
        }
    }

    // If no images in DB ? seed both DB + files
    if (!db.RoomImages.Any() && Directory.Exists(seedPath))
    {
        foreach (var file in Directory.GetFiles(seedPath))
        {
            var fileName = Path.GetFileName(file);
            var destPath = Path.Combine(uploadsPath, fileName);

            File.Copy(file, destPath, overwrite: true);

            db.RoomImages.Add(new RoomImage
            {
                Url = "/uploads/" + fileName
            });
        }

        db.SaveChanges();
    }
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
