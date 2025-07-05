using Microsoft.AspNetCore.Authentication.Cookies;
using MovieBookingWebMVC.Areas.Movie.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Bổ sung dòng này để inject IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ✅ Cấu hình HttpClient để gọi API
builder.Services.AddHttpClient("ApiClient_User", client =>
{
    client.BaseAddress = new Uri("https://localhost:7165/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("ApiClient_Movie", client =>
{
    client.BaseAddress = new Uri("https://localhost:7197/");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddScoped<IRoomService, RoomLayoutService>();
builder.Services.AddScoped<MovieApiService>();

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
if (!app.Environment.IsDevelopment())
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

// ✅ Route mặc định (không có Area)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
