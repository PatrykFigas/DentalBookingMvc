using DentalBookingMvc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DentalBookingMvc.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Role
        string[] roles = new[] { "Admin", "User" };
        foreach (var r in roles)
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        // Admin
        var adminEmail = "admin@demo.pl";
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "Administrator"
            };

            await userMgr.CreateAsync(admin, "Admin123!");
            await userMgr.AddToRoleAsync(admin, "Admin");
        }
        // User
        var userEmail = "user@demo.pl";
        var user = await userMgr.FindByEmailAsync(userEmail);
        if (user == null)
        {
            user = new AppUser
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true,
                FullName = "Jan Kowalski"
            };

            await userMgr.CreateAsync(user, "User123!");
            await userMgr.AddToRoleAsync(user, "User");
        }

        // Dane demo
        if (!await db.Dentists.AnyAsync())
        {
            db.Dentists.AddRange(
                new Dentist { FullName = "lek. dent. Anna Nowak", Specialty = "Stomatologia zachowawcza" },
                new Dentist { FullName = "lek. dent. Piotr Kowalski", Specialty = "Endodoncja" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Services.AnyAsync())
        {
            db.Services.AddRange(
                new Service { Name = "Przegląd", Description = "Konsultacja i ocena stanu uzębienia.", Price = 100, DefaultDurationMinutes = 20 },
                new Service { Name = "Wypełnienie", Description = "Leczenie próchnicy + wypełnienie.", Price = 250, DefaultDurationMinutes = 40 },
                new Service { Name = "Skaling", Description = "Usuwanie kamienia i osadów.", Price = 200, DefaultDurationMinutes = 30 }
            );
            await db.SaveChangesAsync();
        }

        // Terminy: jeśli nie ma żadnych PRZYSZŁYCH terminów, dodaj nowe
        if (!await db.TimeSlots.AnyAsync(t => t.Start >= DateTime.Today))
        {
            var dentists = await db.Dentists.OrderBy(d => d.Id).ToListAsync();
            var d1 = dentists[0];
            var d2 = dentists[1];

            var day = DateTime.Today.AddDays(1); // jutro

            db.TimeSlots.AddRange(
                new TimeSlot { DentistId = d1.Id, Start = day.AddHours(9), DurationMinutes = 30 },
                new TimeSlot { DentistId = d1.Id, Start = day.AddHours(10), DurationMinutes = 30 },
                new TimeSlot { DentistId = d2.Id, Start = day.AddHours(12), DurationMinutes = 30 },
                new TimeSlot { DentistId = d2.Id, Start = day.AddHours(13), DurationMinutes = 30 }
            );

            await db.SaveChangesAsync();
        }

    }
}
