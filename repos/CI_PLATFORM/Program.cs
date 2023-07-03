using System;
using CI_PLATFORM.REPOSITORY.INTERFACE;
using CI_PLATFORM.REPOSITORY.SERVICES;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(c=>c.LoginPath="/UserAuthentication/Login");
builder.Services.AddAuthentication("AuthCookie").AddCookie("AuthCookie", options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(2);
    options.Cookie.Name = "AuthCookie";
    options.LoginPath = "/UserAuthentication/Login";
    options.LogoutPath = "/UserAuthentication/LogOut";
});

       
builder.Services.AddSession(options =>
{
    // Set session timeout duration (optional)
    options.IdleTimeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddDbContext<CI_PLATFORM.DataDB.CiPlatformContext>(
        options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
builder.Services.AddScoped<IStoriesListing,StoriesListing>();



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
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=UserAuthentication}/{action=login}");


app.MapControllerRoute(
    name: "search-missions",
    pattern: "Mission/SearchMissions",
    defaults: new { controller = "Mission", action = "SearchMissions" });
app.Run();
