using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TheCutHub.Models;
using TheCutHub.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TheCutHub.Areas.Admin.Services;

namespace TheCutHub.Tests.Services.Admin
{
    public class AdminUserServiceTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AdminUserServiceTests_" + System.Guid.NewGuid())
                .Options;

            return new ApplicationDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var store = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
        }

        [Fact]
        public async Task MakeBarberAsync_Should_Add_Role_And_Create_Barber()
        {
            var userId = "user1";
            var user = new ApplicationUser { Id = userId, Email = "barber@example.com", FullName = "Test User" };

            var userManagerMock = GetMockUserManager();
            var roleManagerMock = GetMockRoleManager();
            var context = GetInMemoryContext();

            userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.IsInRoleAsync(user, "Barber")).ReturnsAsync(false);
            userManagerMock.Setup(x => x.AddToRoleAsync(user, "Barber")).ReturnsAsync(IdentityResult.Success);

            var service = new AdminUserService(userManagerMock.Object, roleManagerMock.Object, context);

            var result = await service.MakeBarberAsync(userId);

            Assert.True(result);
            var barber = await context.Barbers.FirstOrDefaultAsync();
            Assert.NotNull(barber);
            Assert.Equal(userId, barber.UserId);
            Assert.Equal("Test User", barber.FullName);
        }

        [Fact]
        public async Task RemoveBarberAsync_Should_Remove_Role_And_Barber()
        {
            var userId = "user1";
            var user = new ApplicationUser { Id = userId, Email = "barber@example.com", FullName = "Test User" };

            var userManagerMock = GetMockUserManager();
            var roleManagerMock = GetMockRoleManager();
            var context = GetInMemoryContext();

            context.Barbers.Add(new TheCutHub.Models.Barber { UserId = userId, FullName = "Test User" });
            await context.SaveChangesAsync();

            userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            userManagerMock.Setup(x => x.IsInRoleAsync(user, "Barber")).ReturnsAsync(true);
            userManagerMock.Setup(x => x.RemoveFromRoleAsync(user, "Barber")).ReturnsAsync(IdentityResult.Success);

            var service = new AdminUserService(userManagerMock.Object, roleManagerMock.Object, context);

            var result = await service.RemoveBarberAsync(userId);

            Assert.True(result);
            Assert.Empty(context.Barbers);
        }

        [Fact]
        public async Task GetAllUsersAsync_Should_Return_Users()
        {
            var context = GetInMemoryContext();

            var user1 = new ApplicationUser { Id = "1", UserName = "User1" };
            var user2 = new ApplicationUser { Id = "2", UserName = "User2" };
            await context.Users.AddRangeAsync(user1, user2);
            await context.SaveChangesAsync();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(u => u.Users).Returns(context.Users);

            var service = new AdminUserService(userManagerMock.Object, null!, context);

            var result = await service.GetAllUsersAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, u => u.UserName == "User1");
        }


        [Fact]
        public async Task GetUserRolesMapAsync_Should_Return_Map()
        {
            var context = GetInMemoryContext();

            var user = new ApplicationUser { Id = "1", UserName = "User1" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Setup(x => x.Users).Returns(context.Users);
            userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin", "User" });

            var service = new AdminUserService(userManagerMock.Object, null!, context);

            var result = await service.GetUserRolesMapAsync();

            Assert.Single(result);
            Assert.Equal(2, result["1"].Count);
            Assert.Contains("Admin", result["1"]);
        }

    }
}
