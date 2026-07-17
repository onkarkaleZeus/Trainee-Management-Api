using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync(u => u.Username == Environment.GetEnvironmentVariable("Seed__Username")))
        {
            return;
        }

        var passwordHasher = new PasswordHasher<User>();

        var adminUser = new User
        {
            Username = Environment.GetEnvironmentVariable("Seed__Username")!,
            Email = "admin@test.com",
            Role = UserRole.Admin,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };

        adminUser.PasswordHash = passwordHasher.HashPassword(
            adminUser,
            Environment.GetEnvironmentVariable("Seed__Password")!);

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();
    }
}