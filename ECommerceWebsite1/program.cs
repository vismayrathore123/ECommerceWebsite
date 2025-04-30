using ECommerceWebsite.CommonHelper;
using ECommerceWebsite.DataAccessLayer;
using ECommerceWebsite.DataAccessLayer.Infrastructure.IRepository;
using ECommerceWebsite.DataAccessLayer.Infrastructure.Repository;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/myapp-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog instead of default logging
builder.Host.UseSerilog();

// Register services
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Custom Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IApplicationUser, ApplicationUserRepository>();

var app = builder.Build();

// Middleware configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Centralized exception logging with Serilog
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        logger.LogError(exceptionHandlerPathFeature?.Error, "Unhandled exception occurred");
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
