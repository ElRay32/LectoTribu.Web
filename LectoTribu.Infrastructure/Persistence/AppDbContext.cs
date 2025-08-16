using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Entities;
using LectoTribu.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public new Task<int> SaveChangesAsync(CancellationToken ct = default)
        => base.SaveChangesAsync(ct);
    public DbSet<User> Users => Set<User>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<ReadingSchedule> Schedules => Set<ReadingSchedule>();
    public DbSet<ChapterComment> Comments => Set<ChapterComment>();
    public DbSet<Review> Reviews => Set<Review>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Owned<Email>();
        modelBuilder.Owned<Rating>();

        modelBuilder.Entity<User>(b =>
        {
            b.OwnsOne(u => u.Email, e =>
            {
                e.Property(p => p.Value).HasColumnName("Email").HasMaxLength(256);
            });
        });

        modelBuilder.Entity<Review>(b =>
        {
            b.OwnsOne(r => r.Rating, r =>
            {
                r.Property(p => p.Value).HasColumnName("Rating").IsRequired();
            });
        });

        // ... resto (Book, Club, etc.)
        base.OnModelCreating(modelBuilder);
    }
}
