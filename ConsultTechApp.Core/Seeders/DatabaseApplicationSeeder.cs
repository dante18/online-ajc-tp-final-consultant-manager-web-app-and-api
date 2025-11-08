using ConsultTechApp.Core.Context;
using ConsultTechApp.Core.Entities;
using ConsultTechApp.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ConsultTechApp.Core.Seeders;

public static class DatabaseApplicationSeeder
{
    public static async Task SeedDevDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationStoreContext>();

        // 2️⃣ Rôles
        var roles = new[] { "Administrator", "Manager", "Consultant", "RH" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Name = role,
                    NormalizedName = role.ToUpper()
                });
            }
        }

        // ===== 2. Création des utilisateurs =====
        //await CreateUserAsync(userManager, "admin@consulttech.com", "Super", "Admin", "Administrator", "Admin@123#!");
        //await CreateUserAsync(userManager, "rh@consulttech.com", "Claire", "Dupont", "RH", "Rh@123#!");
        //await CreateUserAsync(userManager, "manager@consulttech.com", "Marc", "Leroux", "Manager", "Manager@123#!");

        // 4️⃣ Données métiers
        if (context.Consultants.Any())
            return;

        await SeedBusinessDataAsync(context);
    }

    private static async Task CreateUserAsync(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        string email,
        string firstName,
        string lastName,
        string role,
        string password)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            if (!await userManager.IsInRoleAsync(existing, role))
                await userManager.AddToRoleAsync(existing, role);
            return;
        }

        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true
        };

        // ➤ Étape critique : création
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"❌ Failed to create {email}: {errors}");
        }

        // ➤ Forcer un rechargement depuis la base via UserManager
        var createdUser = await userManager.FindByEmailAsync(email);
        if (createdUser == null)
            throw new InvalidOperationException($"User {email} not found after creation.");

        // ➤ Ajout du rôle
        var roleResult = await userManager.AddToRoleAsync(createdUser, role);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"❌ Failed to assign role {role} to {email}: {errors}");
        }
    }

    private static async Task SeedBusinessDataAsync(ApplicationStoreContext context)
    {
        // ===== Categories =====
        var catDev = new Category { Name = "Développement" };
        var catData = new Category { Name = "Data" };
        var catInfra = new Category { Name = "Infrastructure" };
        var catSoft = new Category { Name = "Soft Skills" };

        await context.Categories.AddRangeAsync(catDev, catData, catInfra, catSoft);
        await context.SaveChangesAsync();

        // ===== Competences =====
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

        // ===== Consultants =====
        var c1 = new Consultant { FirstName = "Alice", LastName = "Durand", Email = "alice@consulttech.com", HireDate = new DateTime(2021, 1, 10) };
        var c2 = new Consultant { FirstName = "Bob", LastName = "Martin", Email = "bob@consulttech.com", HireDate = new DateTime(2020, 6, 1) };
        var c3 = new Consultant { FirstName = "Julie", LastName = "Bernard", Email = "julie@consulttech.com", HireDate = new DateTime(2022, 3, 15) };

        await context.Consultants.AddRangeAsync(c1, c2, c3);
        await context.SaveChangesAsync();

        // ===== Competences des consultants =====
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

        // ===== Customer =====
        var customers = new List<Customer>
            {
                new Customer { CompanyName = "Acme Corp", Industry = "Finance", Address = "1 Rue de la Bourse, Paris", ContactName = "Jean Petit", ContactEmail = "jean@acme.com" },
                new Customer { CompanyName = "TechNova", Industry = "Informatique", Address = "10 Avenue du Web, Lyon", ContactName = "Sophie Lambert", ContactEmail = "sophie@technova.com" }
            };
        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();

        // ===== Missions =====
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

        // ===== Affectations =====
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
}
