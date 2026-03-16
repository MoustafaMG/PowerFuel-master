using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PowerFuel.Domain.Entities;

namespace PowerFuel.Infrastructure.Data;

public static class DbSeed
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            if (await context.Categories.AnyAsync()) return;

            var productCategories = new[]
            {
                new Category { Name = "Supplements", Slug = "supplements", Description = "Premium supplements", Kind = "Product" },
                new Category { Name = "Equipment", Slug = "equipment", Description = "Gym equipment", Kind = "Product" },
                new Category { Name = "Workout Programs", Slug = "workout-programs", Description = "Expert programs", Kind = "Product" }
            };
            var equipmentCategories = new[]
            {
                new Category { Name = "Chest", Slug = "chest", Description = "Chest equipment", Kind = "Equipment" },
                new Category { Name = "Back", Slug = "back", Description = "Back equipment", Kind = "Equipment" },
                new Category { Name = "Shoulder", Slug = "shoulder", Description = "Shoulder equipment", Kind = "Equipment" },
                new Category { Name = "Leg", Slug = "leg", Description = "Leg equipment", Kind = "Equipment" }
            };

            await context.Categories.AddRangeAsync(productCategories);
            await context.Categories.AddRangeAsync(equipmentCategories);
            await context.SaveChangesAsync();

            var categoriesBySlug = await context.Categories.AsNoTracking().ToDictionaryAsync(c => c.Slug);
            int supplementsId = categoriesBySlug["supplements"].Id;
            int chestId = categoriesBySlug["chest"].Id;
            int backId = categoriesBySlug["back"].Id;
            int shoulderId = categoriesBySlug["shoulder"].Id;
            int legId = categoriesBySlug["leg"].Id;

            var coach = new Coach
            {
                FirstName = "Ahmed",
                LastName = "Mohamed",
                Title = "Professional Bodybuilding Coach",
                Specialization = "Bodybuilding",
                AboutDescription = "Ahmed is a professional bodybuilding coach with over 8 years of experience. He has won multiple championships and specializes in helping athletes achieve their peak physical condition through customized training programs and nutrition plans.",
                YearsExperience = 8,
                HappyClientsCount = 150,
                CertificationsCount = 5,
                PhoneNumber = "+20 123 456 7890",
                Email = "ahmed.mohamed@example.com",
                HourlyRate = 50m,
                CreatedAt = DateTime.UtcNow
            };
            context.Coaches.Add(coach);
            await context.SaveChangesAsync();

            var availabilities = new[]
            {
                new CoachAvailability { CoachId = coach.Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) },
                new CoachAvailability { CoachId = coach.Id, DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) },
                new CoachAvailability { CoachId = coach.Id, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) },
                new CoachAvailability { CoachId = coach.Id, DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(17, 0) },
                new CoachAvailability { CoachId = coach.Id, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(13, 0) }
            };
            await context.CoachAvailabilities.AddRangeAsync(availabilities);

            var products = new[]
            {
                new Product { Name = "Quantum Whey Isolate", ShortDescription = "Ultra-filtered whey protein with 25g protein per serving", Price = 59.99m, OriginalPrice = 74.99m, IsOnSale = true, CategoryId = supplementsId, StockQuantity = 100, AverageRating = 4.9m, ReviewCount = 24, IsBestSeller = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "NeuroFocus Pre-Workout", ShortDescription = "Mental clarity and explosive energy formula", Price = 39.99m, CategoryId = supplementsId, StockQuantity = 80, AverageRating = 4.8m, ReviewCount = 18, IsBestSeller = true, CreatedAt = DateTime.UtcNow },
                new Product { Name = "RecoverElite BCAA", ShortDescription = "2:1:1 BCAA ratio with electrolytes and vitamins", Price = 32.99m, CategoryId = supplementsId, StockQuantity = 120, AverageRating = 4.7m, ReviewCount = 31, IsBestSeller = true, CreatedAt = DateTime.UtcNow }
            };
            await context.Products.AddRangeAsync(products);

            var equipments = new[]
            {
                new Equipment { Name = "Chest Press Machine", ShortDescription = "Certified trainer with 8 years of experience in bodybuilding and fitness.", Price = 499.99m, OriginalPrice = 599.99m, IsOnSale = true, CategoryId = chestId, StockQuantity = 15, FeaturedCoachId = coach.Id, CreatedAt = DateTime.UtcNow },
                new Equipment { Name = "Dual Lat Pulldown / Low Row Machine", ShortDescription = "Certified fitness and functional training specialist.", Price = 799.99m, CategoryId = backId, StockQuantity = 10, CreatedAt = DateTime.UtcNow },
                new Equipment { Name = "Shoulder Press Machine", ShortDescription = "Certified nutrition specialist with 10 years of experience.", Price = 549.99m, CategoryId = shoulderId, StockQuantity = 12, CreatedAt = DateTime.UtcNow },
                new Equipment { Name = "Titan Adjustable Dumbbells", ShortDescription = "5-52.5 lbs adjustable dumbbells with quick-lock system", Price = 349.99m, OriginalPrice = 399m, IsOnSale = true, CategoryId = legId, StockQuantity = 25, AverageRating = 4.7m, ReviewCount = 15, CreatedAt = DateTime.UtcNow }
            };
            await context.Equipments.AddRangeAsync(equipments);
            await context.SaveChangesAsync();

            logger.LogInformation("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
}
