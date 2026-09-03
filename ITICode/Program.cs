using Healthcare.Application.ServiceContracts;
using Healthcare.Application.Services;
using Healthcare.Infrastructure.AuthServices;
using Healthcare.Infrastructure.Stripe;
using Microsoft.AspNetCore.Identity;
using Healthcare.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Healthcare.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.InfraServices(builder.Configuration);

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});


// Add Razor Pages support (needed for Identity UI scaffolding)
builder.Services.AddRazorPages();


//use stripesettings as an optionsservice

var stripeSettings = builder.Configuration.GetSection("Stripe").Get<StripeSettings>();
if (stripeSettings != null)
{
	Stripe.StripeConfiguration.ApiKey = stripeSettings.SecretKey;
}

var app = builder.Build();

await IdentityDbInitializer.SeedRolesAsync(app.Services);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();
