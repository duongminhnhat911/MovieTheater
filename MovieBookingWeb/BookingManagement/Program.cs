using BookingManagement;
using BookingManagement.BackgroundServices;
using BookingManagement.Models.Entities;
using BookingManagement.Models.VnPayModels;
using BookingManagement.Repositories;
using BookingManagement.Service;
using BookingManagement.Service.VnPay;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using VNPAY.NET;

var builder = WebApplication.CreateBuilder(args);

// 👉 1. Add services to the container (trước khi builder.Build())
builder.Services.AddControllers();
builder.Services.AddSignalR()
 .AddHubOptions<SeatHub>(options =>
  {
      options.EnableDetailedErrors = true;
  });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<ISeatShowtimeRepository, SeatShowtimeRepository>();
builder.Services.AddScoped<ISeatShowtimeService, SeatShowtimeService>();
builder.Services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
builder.Services.AddScoped<IShowtimeService, ShowtimeService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddHostedService<SeatHoldCleanupService>();


builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


//Đăng ký DbContext
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<IMovieServiceClient, MovieManagementHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://moviemanagement-api.azurewebsites.net/"); // ✅ Đã chỉnh sửa đúng URI deploy
});
//Cấu hình CORS cho MVC
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.WithOrigins("https://moviebookingweb-mvc.azurewebsites.net") // Port của frontend MVC
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
// cấu hình API VNPAY
builder.Services.Configure<VnpayOptions>(builder.Configuration.GetSection("Vnpay"));
builder.Services.AddSingleton<IVnpay, Vnpay>();
builder.Services.AddScoped<IVnpayPaymentService, VnpayPaymentService>();

//Đăng ký các DI (Interface - Service - Repository)


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// -------------------- 3. MIGRATION (tùy chọn) --------------------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    dbContext.Database.Migrate();
}

// -------------------- 4. MIDDLEWARE PIPELINE --------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Booking Management API v1");
    c.RoutePrefix = string.Empty; // Đặt Swagger UI ở root URL (tùy chọn)
});

app.UseHttpsRedirection();

// ❗ Đúng thứ tự: Static Files → Routing → CORS → Auth → Controllers → SignalR
app.UseStaticFiles(); // Cho phép dùng file HTML test
app.UseRouting();

app.UseCors("AllowMVC");

app.UseAuthentication(); // nếu có
app.UseAuthorization();

// 👉 Map Controller và Hub
app.MapControllers();
app.MapHub<SeatHub>("/seatHub").RequireCors("AllowMVC");

// -------------------- 5. CHẠY APP --------------------
app.Run();