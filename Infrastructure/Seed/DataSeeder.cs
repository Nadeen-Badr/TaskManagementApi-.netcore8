using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync(x => x.Role == UserRole.Admin))
            return;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Email = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}