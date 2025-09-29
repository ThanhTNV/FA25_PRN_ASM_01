using ASM_01.BusinessLayer.Services;
using ASM_01.DataAccessLayer;
using ASM_01.DataAccessLayer.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EVRetailsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<SimpleAuthService>();
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<DealerInventoryService>();
builder.Services.AddScoped<DistributionManagementService>();
builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.AccessDeniedPath = "/Auth/Denied";
                    options.Cookie.Name = "EVRetails.auth";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                    // options.Cookie.SameSite = SameSiteMode.Lax; // adjust if needed
                    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // in production with HTTPS
                });

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EVRetailsDbContext>();
    await MigrateDatabase.ApplyMigrations(dbContext);
}

app.Run();
