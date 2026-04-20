using Microsoft.EntityFrameworkCore;
using TaskForge.Domain.Entities;

namespace TaskForge.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}