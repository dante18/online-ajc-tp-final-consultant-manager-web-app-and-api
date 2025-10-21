using ConsultTechApp.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ConsultTechApp.Core.Context;

public class ApplicationStoreContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Consultant> Consultants => Set<Consultant>();

    public DbSet<ConsultantSkill> ConsultantSkills => Set<ConsultantSkill>();

    public DbSet<Mission> Missions => Set<Mission>();

    public DbSet<MissionAssignment> MissionAssignments => Set<MissionAssignment>();

    public DbSet<Skill> Skills => Set<Skill>();

    public ApplicationStoreContext(DbContextOptions<ApplicationStoreContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("AspNetUsers");

        // ========================
        // CATEGORY
        // ========================
        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasMany(c => c.Skills)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        
        // ========================
        // CONSULTANT
        // ========================
        modelBuilder.Entity<Consultant>(builder =>
        {
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // ========================
        // CONSULTANTSKILL
        // ========================
        modelBuilder.Entity<ConsultantSkill>(builder =>
        {
            builder.HasKey(cs => new { cs.ConsultantId, cs.SkillId });

            builder.HasOne(cs => cs.Consultant)
                .WithMany(c => c.ConsultantSkills)
                .HasForeignKey(cs => cs.ConsultantId);

            builder.HasOne(cs => cs.Skill)
                .WithMany(s => s.ConsultantSkills)
                .HasForeignKey(cs => cs.SkillId);

            builder.Property(cs => cs.Level)
                .HasConversion<string>();
        });

        // ========================
        // MISSION
        // ========================
        modelBuilder.Entity<Mission>(builder =>
        {
            builder.HasOne(x => x.Customer)
                .WithMany(c => c.Missions)
                .HasForeignKey(x => x.CustomerId);
        });

        // ========================
        // MISSION ASSIGNMENT
        // ========================
        modelBuilder.Entity<MissionAssignment>(builder =>
        {
            builder.HasOne(x => x.Mission)
                .WithMany(m => m.Assignments)
                .HasForeignKey(x => x.MissionId);

            builder.HasOne(x => x.Consultant)
                .WithMany(c => c.MissionAssignments)
                .HasForeignKey(x => x.ConsultantId);
        });

        // ========================
        // SKILL
        // ========================
        modelBuilder.Entity<Skill>(builder =>
        {
            builder.HasOne(x => x.Category)
                .WithMany(c => c.Skills)
                .HasForeignKey(x => x.CategoryId);
        });

        // ========================
        // USER
        // ========================
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasIndex(u => u.Email).IsUnique();
        });
    }
}

