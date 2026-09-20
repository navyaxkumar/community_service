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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Database model configuration will be added in later phases.
    }
}
