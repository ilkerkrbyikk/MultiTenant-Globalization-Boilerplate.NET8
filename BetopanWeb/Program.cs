using BetopanWeb.Data.Context;
using BetopanWeb.Localization;
using BetopanWeb.Services;
using BetopanWeb.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BetopanWeb.Middleware; 
using BetopanWeb.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<BetopanDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//cache 
builder.Services.AddMemoryCache();

// http context
builder.Services.AddHttpContextAccessor();

// Scopped DI
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseTenantResolution();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
