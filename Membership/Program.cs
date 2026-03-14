
using Membership.Data;
using Membership.Models;
using Membership.Services;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings")); // تم التعديل
builder.Services.AddScoped<IEmailService, SmtpEmailService>(); // تم التعديل

// Add services to the container.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Expiration = TimeSpan.FromMinutes(60);
});

// إضافة خدمات الكوكيز لتسجيل الدخول // تم التعديل
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "UserLoginCookie"; // تم التعديل
        config.LoginPath = "/Account/Login"; // تم التعديل
        config.AccessDeniedPath = "/Account/Login"; // تم التعديل
    });

builder.Services.AddAuthorization(); // تم التعديل

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

app.UseAuthentication(); // تم التعديل
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();