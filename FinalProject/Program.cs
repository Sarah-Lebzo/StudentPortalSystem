using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContexts

builder.Services.AddDbContext<FinalAuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinalDbConnectionString")));

builder.Services.AddDbContext<FinalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FinalDbConnectionString")));

// Identity

//builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>

{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<FinalAuthDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});


// Email service
builder.Services.AddTransient<IEmailSender, DummyEmailSender>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
