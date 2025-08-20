using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TheCutHub.Areas.Admin.Interfaces;
using TheCutHub.Areas.Admin.Services;
using TheCutHub.Areas.Barber.Services;
using TheCutHub.Data;
using TheCutHub.Models;
using TheCutHub.Services;

namespace TheCutHub
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

          
            var provider = (Environment.GetEnvironmentVariable("DB_PROVIDER")
                            ?? builder.Configuration["DB_PROVIDER"]
                            ?? "sqlserver").ToLowerInvariant();

            
            string? conn = builder.Configuration.GetConnectionString("DefaultConnection");

     
            string? dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
                            ?? builder.Configuration["DATABASE_URL"];

            if ((provider is "postgres" or "postgresql") &&
                !string.IsNullOrWhiteSpace(dbUrl) &&
                dbUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(dbUrl);
                var userInfo = uri.UserInfo.Split(':', 2);
                var npg = new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port,
                    Username = userInfo[0],
                    Password = userInfo.Length > 1 ? userInfo[1] : "",
                    Database = uri.AbsolutePath.Trim('/'),
                    SslMode = SslMode.Require,
                    TrustServerCertificate = true
                };
                conn = npg.ConnectionString;
            }

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("No connection string resolved for DefaultConnection.");

        
            builder.Services.AddDbContext<ApplicationDbContext>(opt =>
            {
                switch (provider)
                {
                    case "sqlite":
                        opt.UseSqlite(conn);
                        break;
                    case "postgres":
                    case "postgresql":
                        opt.UseNpgsql(conn);
                        break;
                    default:
                        opt.UseSqlServer(conn);
                        break;
                }
            });

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

           
            builder.Services.AddScoped<IAdminServiceService, AdminServiceService>();
            builder.Services.AddScoped<IBarberService, BarberService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IAdminAppointmentService, AdminAppointmentService>();
            builder.Services.AddScoped<IAdminBarberService, AdminBarberService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IAdminWorkingHourService, AdminWorkingHourService>();
            builder.Services.AddScoped<TheCutHub.Areas.Barber.Interfaces.IBarberAppointmentService,
                                       TheCutHub.Areas.Barber.Services.BarberAppointmentService>();

            builder.Services
                .AddDefaultIdentity<ApplicationUser>(opt => opt.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            Console.WriteLine($"[DEBUG] DB_PROVIDER = {provider}");
            Console.WriteLine($"[DEBUG] ConnectionString = {conn}");

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                Console.WriteLine($"[DEBUG] EF ProviderName = {db.Database.ProviderName}");

               
                if (provider is "postgres" or "postgresql" or "sqlite")
                    await db.Database.EnsureCreatedAsync();
                else
                    await db.Database.MigrateAsync();

                await ApplicationDbInitializer.SeedAsync(scope.ServiceProvider);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
