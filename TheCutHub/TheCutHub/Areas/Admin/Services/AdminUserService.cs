using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheCutHub.Data;
using TheCutHub.Models;

namespace TheCutHub.Areas.Admin.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IDictionary<string, IList<string>>> GetUserRolesMapAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var map = new Dictionary<string, IList<string>>();

            foreach (var user in users)
            {
                map[user.Id] = await _userManager.GetRolesAsync(user);
            }

            return map;
        }

        public async Task<bool> MakeBarberAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (!await _userManager.IsInRoleAsync(user, "Barber"))
            {
                await _userManager.AddToRoleAsync(user, "Barber");
            }

            if (!await _context.Barbers.AnyAsync(b => b.UserId == user.Id))
            {
                var barber = new TheCutHub.Models.Barber
                {
                    UserId = user.Id,
                    FullName = user.FullName ?? user.Email
                };

                _context.Barbers.Add(barber);
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveBarberAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (await _userManager.IsInRoleAsync(user, "Barber"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Barber");
            }

            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (barber != null)
            {
                _context.Barbers.Remove(barber);
                await _context.SaveChangesAsync();
            }

            return true;
        }
    }
}

