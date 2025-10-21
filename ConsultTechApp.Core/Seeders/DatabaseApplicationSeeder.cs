using ConsultTechApp.Core.Context;
using ConsultTechApp.Core.Entities;
using ConsultTechApp.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConsultTechApp.Core.Seeders;

public static class DatabaseApplicationSeeder
{
    public static async Task SeedDevDataAsync(
        ApplicationStoreContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        await context.Database.MigrateAsync();

        // ===== 1. Création des rôles =====
        var roles = new[] { "Administrator", "Manager", "Consultant", "RH" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
            }
        }

        // ===== 2. Création des utilisateurs =====
        await CreateUserAsync(userManager, "admin@consulttech.com", "Super", "Admin", "Administrator", "Admin@123#!");
        await CreateUserAsync(userManager, "rh@consulttech.com", "Claire", "Dupont", "RH", "Rh@123#!");
        await CreateUserAsync(userManager, "manager@consulttech.com", "Marc", "Leroux", "Manager", "Manager@123#!");

        // ===== 3. Vérifie si des données existent déjà =====
        if (context.Consultants.Any()) return;

        // ===== 4. Catégories =====
        var catDev = new Category { Name = "Développement" };
        var catData = new Category { Name = "Data" };
        var catInfra = new Category { Name = "Infrastructure" };
        var catSoft = new Category { Name = "Soft Skills" };

        await context.Categories.AddRangeAsync(catDev, catData, catInfra, catSoft);
        await context.SaveChangesAsync();

        // ===== 5. Compétences =====
        var skills = new List<Skill>
            {
                new Skill { Name = "C#", CategoryId = catDev.Id },
                new Skill { Name = "ASP.NET Core", CategoryId = catDev.Id },
                new Skill { Name = "SQL", CategoryId = catData.Id },
                new Skill { Name = "Azure DevOps", CategoryId = catInfra.Id },
                new Skill { Name = "Communication", CategoryId = catSoft.Id }
            };
        await context.Skills.AddRangeAsync(skills);
        await context.SaveChangesAsync();

        // ===== 6. Consultants =====
        var c1 = new Consultant { FirstName = "Alice", LastName = "Durand", Email = "alice@consulttech.com", HireDate = new DateTime(2021, 1, 10) };
        var c2 = new Consultant { FirstName = "Bob", LastName = "Martin", Email = "bob@consulttech.com", HireDate = new DateTime(2020, 6, 1) };
        var c3 = new Consultant { FirstName = "Julie", LastName = "Bernard", Email = "julie@consulttech.com", HireDate = new DateTime(2022, 3, 15) };

        await context.Consultants.AddRangeAsync(c1, c2, c3);
        await context.SaveChangesAsync();

        // ===== 7. Compétences des consultants =====
        var consultantSkills = new List<ConsultantSkill>
            {
                new ConsultantSkill { ConsultantId = c1.Id, SkillId = skills[0].Id, Level = ExpertiseLevel.Expert },
                new ConsultantSkill { ConsultantId = c1.Id, SkillId = skills[1].Id, Level = ExpertiseLevel.Advanced },
                new ConsultantSkill { ConsultantId = c2.Id, SkillId = skills[2].Id, Level = ExpertiseLevel.Intermediate },
                new ConsultantSkill { ConsultantId = c3.Id, SkillId = skills[3].Id, Level = ExpertiseLevel.Advanced },
                new ConsultantSkill { ConsultantId = c3.Id, SkillId = skills[4].Id, Level = ExpertiseLevel.Expert }
            };
        await context.ConsultantSkills.AddRangeAsync(consultantSkills);
        await context.SaveChangesAsync();

        // ===== 8. Clients =====
        var customers = new List<Customer>
            {
                new Customer { CompanyName = "Acme Corp", Industry = "Finance", Address = "1 Rue de la Bourse, Paris", ContactName = "Jean Petit", ContactEmail = "jean@acme.com" },
                new Customer { CompanyName = "TechNova", Industry = "Informatique", Address = "10 Avenue du Web, Lyon", ContactName = "Sophie Lambert", ContactEmail = "sophie@technova.com" }
            };
        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();

        // ===== 9. Missions =====
        var mission1 = new Mission
        {
            Title = "Migration Base de Données",
            Description = "Migration du système SQL Server vers Azure SQL.",
            StartDate = DateTime.UtcNow.AddDays(7),
            EndDate = DateTime.UtcNow.AddMonths(2),
            EstimatedBudget = 30000,
            CustomerId = customers[0].Id
        };

        var mission2 = new Mission
        {
            Title = "Développement API REST",
            Description = "Conception d'une API REST pour un portail client.",
            StartDate = DateTime.UtcNow.AddDays(15),
            EndDate = DateTime.UtcNow.AddMonths(3),
            EstimatedBudget = 50000,
            CustomerId = customers[1].Id
        };

        await context.Missions.AddRangeAsync(mission1, mission2);
        await context.SaveChangesAsync();

        // ===== 10. Affectations =====
        var assignments = new List<MissionAssignment>
            {
                new MissionAssignment
                {
                    MissionId = mission1.Id,
                    ConsultantId = c2.Id,
                    AssignmentStart = mission1.StartDate,
                    AssignmentEnd = mission1.EndDate,
                    DailyRate = 550,
                    Role = "DB Engineer"
                },
                new MissionAssignment
                {
                    MissionId = mission2.Id,
                    ConsultantId = c1.Id,
                    AssignmentStart = mission2.StartDate,
                    AssignmentEnd = mission2.EndDate,
                    DailyRate = 600,
                    Role = "Backend Developer"
                }
            };
        await context.MissionAssignments.AddRangeAsync(assignments);
        await context.SaveChangesAsync();
    }

    private static async Task CreateUserAsync(
        UserManager<User> userManager,
        string email,
        string firstName,
        string lastName,
        string role,
        string password)
    {
        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null) return;

        var user = new User
        {
            UserName = email.Split('@')[0],
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true
        };

        var createdUser = await userManager.FindByEmailAsync(email);
        if (createdUser == null)
            throw new InvalidOperationException($"User {email} not found after creation.");

        await userManager.AddToRoleAsync(createdUser, role);
    }
}
