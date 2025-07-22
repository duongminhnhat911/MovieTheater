using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using UserManagement;
using Microsoft.AspNetCore.Authentication.Cookies;
using UserManagement;
using UserManagement.Repositories;
using UserManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// 👉 1. Add services to the container (trước khi builder.Build())
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Đăng ký DbContext
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Cấu hình CORS cho MVC
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMVC", policy =>
    {
        policy.WithOrigins("https://moviebookingweb-mvc.azurewebsites.net/") // Port của frontend MVC
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Đăng ký dịch vụ
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.AccessDeniedPath = "/api/auth/forbidden";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

var app = builder.Build();

//Apply database migration (nếu chưa có database thì EF sẽ tạo và seed luôn)
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    dbContext.Database.Migrate();
}
catch (Exception ex)
{
    Console.WriteLine("⚠️ Failed to apply migration: " + ex.Message);
}
// Hiển thị Swagger bất kể môi trường nào (dev/prod/staging, v.v.)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1");
    c.RoutePrefix = string.Empty; // Đặt Swagger UI ở root URL (tùy chọn)
});

app.UseHttpsRedirection();

app.UseCors("AllowMVC");
app.UseAuthentication(); // 🛑 Bắt buộc phải có TRƯỚC UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();