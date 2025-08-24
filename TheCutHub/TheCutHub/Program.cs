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

            var provider = (
                Environment.GetEnvironmentVariable("DB_PROVIDER")
                ?? builder.Configuration["DB_PROVIDER"]
                ?? (builder.Environment.IsDevelopment() ? "sqlserver" : "postgres")
            ).ToLowerInvariant();

            string? conn = builder.Configuration.GetConnectionString("DefaultConnection");

            if (provider is "postgres" or "postgresql")
            {
              
                string? raw =
                    conn ??
                    Environment.GetEnvironmentVariable("DATABASE_URL") ??
                    builder.Configuration["DATABASE_URL"];

                if (!string.IsNullOrWhiteSpace(raw) &&
                    (raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                     raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)))
                {
                    var uri = new Uri(raw);
                    var userInfo = uri.UserInfo.Split(':', 2, StringSplitOptions.None);

                    var npg = new NpgsqlConnectionStringBuilder
                    {
                        Host = uri.Host,
                        Port = uri.IsDefaultPort ? 5432 : uri.Port,
                        Username = Uri.UnescapeDataString(userInfo[0]),
                        Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "",
                        Database = uri.AbsolutePath.Trim('/'),
                        SslMode = SslMode.Require
                 
                    };

                    conn = npg.ConnectionString;
                }
            }

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("No connection string resolved for DefaultConnection.");

           
            if (provider is "postgres" or "postgresql")
            {
                builder.Services.AddDbContext<ApplicationDbContext>(opt =>
                    opt.UseNpgsql(conn));
            }
            else 
            {
                builder.Services.AddDbContext<ApplicationDbContext>(opt =>
                    opt.UseSqlServer(conn));
            }

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddScoped<IAdminServiceService, AdminServiceService>();
            builder.Services.AddScoped<IBarberService, BarberService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IAdminAppointmentService, AdminAppointmentService>();
            builder.Services.AddScoped<IAdminBarberService, AdminBarberService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IBarberProfileService, BarberProfileService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<TheCutHub.Areas.Barber.Interfaces.IBarberAppointmentService,
                                       TheCutHub.Areas.Barber.Services.BarberAppointmentService>();
           
            builder.Services
                .AddDefaultIdentity<ApplicationUser>(opt => opt.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

     
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (app.Environment.IsDevelopment() && provider is not ("postgres" or "postgresql"))
                {
                    await db.Database.MigrateAsync();
                }
                else
                {
                    await db.Database.EnsureCreatedAsync();
                }

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
