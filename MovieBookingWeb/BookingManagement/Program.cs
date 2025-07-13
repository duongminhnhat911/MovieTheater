using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using BookingManagement;
using BookingManagement.Service;
using BookingManagement.Repositories;
using BookingManagement.Service.VnPay;
using VNPAY.NET;
using BookingManagement.Models.VnPayModels;

var builder = WebApplication.CreateBuilder(args);

// 👉 1. Add services to the container (trước khi builder.Build())
builder.Services.AddControllers();
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

builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


//Đăng ký DbContext
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<IMovieServiceClient, MovieManagementHttpClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7197/"); // ⚠️ Đổi đúng URI MovieManagement service của bạn
});
//Cấu hình CORS cho MVC
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.WithOrigins("https://localhost:7169") // Port của frontend MVC
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

//Apply database migration (nếu chưa có database thì EF sẽ tạo và seed luôn)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    dbContext.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowMVC");

app.UseAuthorization();

app.MapControllers();

app.Run();