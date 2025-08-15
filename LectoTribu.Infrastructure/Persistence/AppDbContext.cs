using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Entities;
using LectoTribu.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LectoTribu.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
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

        modelBuilder.Entity<Book>(b =>
        {
            b.Property(x => x.Title).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<Club>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Ignore(x => x.Books); // Si implementas many-to-many, crea una tabla intermedia ClubBook
        });

        base.OnModelCreating(modelBuilder);
    }
}
