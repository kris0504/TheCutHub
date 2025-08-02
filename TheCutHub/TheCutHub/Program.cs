using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            // builder.Services.AddScoped<AppointmentService>();
            builder.Services.AddScoped<IAdminServiceService, AdminServiceService>();

            builder.Services.AddScoped<IBarberService, BarberService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IAdminAppointmentService, AdminAppointmentService>();
            builder.Services.AddScoped<IAdminBarberService, AdminBarberService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IAdminUserService, AdminUserService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IAdminWorkingHourService, AdminWorkingHourService>();
            builder.Services.AddScoped<IBarberProfileService, BarberProfileService>();
            builder.Services.AddScoped<IAdminServiceService, AdminServiceService>();
            
            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
            .AddRoles<IdentityRole>() 
            .AddEntityFrameworkStores<ApplicationDbContext>();
            

            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            
            
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
            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseExceptionHandler("/Error");

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            
            await ApplicationDbInitializer.SeedRolesAndAdminAsync(app.Services);
            await ApplicationDbInitializer.SeedWorkingHoursAsync(app.Services);
            app.Run();
        }
    }
}
