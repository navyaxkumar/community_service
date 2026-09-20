using DigitalShield.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalShield.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<FraudCategory> FraudCategories => Set<FraudCategory>();
    public DbSet<LearningModule> LearningModules => Set<LearningModule>();
    public DbSet<Scenario> Scenarios => Set<Scenario>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();
    public DbSet<QuizOption> QuizOptions => Set<QuizOption>();
    public DbSet<UserProgress> UserProgress => Set<UserProgress>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<Badge> Badges => Set<Badge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("User");

            entity.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);

            entity.HasMany(u => u.UserProgress)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(u => u.QuizAttempts)
                .WithOne(qa => qa.User)
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FraudCategory>(entity =>
        {
            entity.Property(fc => fc.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.HasIndex(fc => fc.Name)
                .IsUnique();

            entity.Property(fc => fc.Description)
                .HasMaxLength(500);

            entity.Property(fc => fc.IsActive)
                .HasDefaultValue(true);

            entity.Property(fc => fc.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<LearningModule>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_LearningModules_Order_NonNegative", "[Order] >= 0");
            });

            entity.Property(lm => lm.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(lm => lm.Description)
                .HasMaxLength(1000);

            entity.Property(lm => lm.Content)
                .IsRequired();

            entity.Property(lm => lm.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(lm => lm.FraudCategory)
                .WithMany(fc => fc.LearningModules)
                .HasForeignKey(lm => lm.FraudCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Scenario>(entity =>
        {
            entity.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(s => s.Description)
                .HasMaxLength(1000);

            entity.Property(s => s.Situation)
                .IsRequired();

            entity.Property(s => s.CorrectAction)
                .IsRequired();

            entity.Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(s => s.FraudCategory)
                .WithMany(fc => fc.Scenarios)
                .HasForeignKey(s => s.FraudCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(q => q.Description)
                .HasMaxLength(1000);

            entity.Property(q => q.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(q => q.FraudCategory)
                .WithMany(fc => fc.Quizzes)
                .HasForeignKey(q => q.FraudCategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<QuizQuestion>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_QuizQuestions_Order_NonNegative", "[Order] >= 0");
            });

            entity.Property(qq => qq.QuestionText)
                .IsRequired();

            entity.Property(qq => qq.Explanation)
                .HasMaxLength(1000);

            entity.HasOne(qq => qq.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(qq => qq.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuizOption>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_QuizOptions_Order_NonNegative", "[Order] >= 0");
            });

            entity.Property(qo => qo.OptionText)
                .IsRequired();

            entity.HasOne(qo => qo.QuizQuestion)
                .WithMany(qq => qq.Options)
                .HasForeignKey(qo => qo.QuizQuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserProgress>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_UserProgress_ProgressPercentage_Range", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
            });

            entity.HasIndex(up => new { up.UserId, up.LearningModuleId })
                .IsUnique();

            entity.Property(up => up.ProgressPercentage)
                .HasDefaultValue(0);

            entity.Property(up => up.StartedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(up => up.User)
                .WithMany(u => u.UserProgress)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(up => up.LearningModule)
                .WithMany(lm => lm.UserProgress)
                .HasForeignKey(up => up.LearningModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<QuizAttempt>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_QuizAttempts_Score_NonNegative", "[Score] >= 0");
                table.HasCheckConstraint("CK_QuizAttempts_TotalQuestions_NonNegative", "[TotalQuestions] >= 0");
                table.HasCheckConstraint("CK_QuizAttempts_Score_NotGreaterThanTotalQuestions", "[Score] <= [TotalQuestions]");
            });

            entity.Property(qa => qa.StartedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(qa => qa.TotalQuestions)
                .HasDefaultValue(0);

            entity.Property(qa => qa.Score)
                .HasDefaultValue(0);

            entity.HasOne(qa => qa.User)
                .WithMany(u => u.QuizAttempts)
                .HasForeignKey(qa => qa.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(qa => qa.Quiz)
                .WithMany(q => q.QuizAttempts)
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Badge>(entity =>
        {
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_Badges_RequiredPoints_NonNegative", "[RequiredPoints] >= 0");
            });

            entity.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(b => b.Description)
                .HasMaxLength(500);

            entity.Property(b => b.Icon)
                .HasMaxLength(200);

            entity.Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(b => b.IsActive)
                .HasDefaultValue(true);
        });
    }
}
