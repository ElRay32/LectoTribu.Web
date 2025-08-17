using LectoTribu.Domain.Abstractions;
using LectoTribu.Domain.Entities;
using LectoTribu.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LectoTribu.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<ReadingSchedule> Schedules => Set<ReadingSchedule>();
    public DbSet<ChapterComment> Comments => Set<ChapterComment>();
    public DbSet<Review> Reviews => Set<Review>();

    // IUnitOfWork (delegamos al DbContext)
    public new Task<int> SaveChangesAsync(CancellationToken ct = default) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // value objects
        modelBuilder.Owned<Email>();
        modelBuilder.Entity<Review>()
         .Property(r => r.Rating)
         .HasConversion(
             v => v.Value,     // de Rating -> int
             v => new Rating(v) // de int -> Rating
         )
         .HasColumnName("Rating")
        .IsRequired();

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
            b.Property(x => x.TotalChapters).IsRequired();
            // ⚠️ OJO: aquí ya NO tocamos ISBN
        });

        modelBuilder.Entity<Club>(b =>
        {
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.Ignore(x => x.Books); // si luego haces many-to-many, crea tabla intermedia ClubBook
        });

        base.OnModelCreating(modelBuilder);
    }
}




