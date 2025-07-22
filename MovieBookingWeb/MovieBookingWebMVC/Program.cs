using Microsoft.AspNetCore.Authentication.Cookies;
using MovieBookingWebMVC.Areas.Booking.Services;
using MovieBookingWebMVC.Areas.Movie.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Bổ sung dòng này để inject IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ✅ Cấu hình HttpClient để gọi API
builder.Services.AddHttpClient("ApiClient_User", client =>
{
    client.BaseAddress = new Uri("https://usermanagement-g6b7euhybjaveud3.indonesiacentral-01.azurewebsites.net/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("ApiClient_Movie", client =>
{
    client.BaseAddress = new Uri("https://moviemanagement-api.azurewebsites.net/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddHttpClient("ApiClient_Booking", client =>
{
    client.BaseAddress = new Uri("https://bookingmanagement-api.azurewebsites.net/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});


builder.Services.AddScoped<MovieBookingWebMVC.Areas.Movie.Services.IRoomService, RoomLayoutService>();
builder.Services.AddScoped<MovieApiService>();
builder.Services.AddScoped<MovieBookingWebMVC.Areas.Booking.Services.IRoomService, RoomService>();
builder.Services.AddScoped<IShowtimeApiService, ShowtimeApiService>();

//DI Booking-------------------------------------------------------
builder.Services.AddScoped<IShowtimeWebService, ShowtimeWebService>();
builder.Services.AddScoped<ISeatWebService, SeatWebService>();
builder.Services.AddScoped<IBookingApiService, BookingApiService>();
builder.Services.AddScoped<IPromotionApiService, PromotionApiService>();

builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Account/Login";
        options.LogoutPath = "/User/Account/Logout";
        options.AccessDeniedPath = "/User/Account/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // ✔️ Hiện lỗi Razor và lỗi chi tiết
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// ✅ Route cho Area "User"
app.MapAreaControllerRoute(
    name: "user",
    areaName: "User",
    pattern: "User/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(
    name: "movie",
    areaName: "Movie",
    pattern: "Movie/{controller=Home}/{action=Index}/{id?}");
app.MapAreaControllerRoute(
    name: "booking",
    areaName: "Booking",
    pattern: "Booking/{controller=Home}/{action=Index}/{id?}");

// ✅ Route cho Area "Booking"
app.MapAreaControllerRoute(
    name: "booking",
    areaName: "Booking",
    pattern: "Booking/{controller=Booking}/{action=Index}/{id?}");

// ✅ Route mặc định (không có Area)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
