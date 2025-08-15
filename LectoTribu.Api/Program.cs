using LectoTribu.Application.Interfaces;
using LectoTribu.Application.Services;
using LectoTribu.Infrastructure.Persistence;
using LectoTribu.Infrastructure.Messaging;
using LectoTribu.Domain.Abstractions;
using LectoTribu.Domain.Entities;
using LectoTribu.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddSingleton<IMessageBus, InMemoryBus>();

builder.Services.AddScoped<IClubService, ClubService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Seed de datos de prueba
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Users.AnyAsync())
    {
        var user = new User("Tokio", new Email("tokio@example.com"));
        var author = new Author("Autor Demo");
        var book = new Book("Libro Demo", author.Id, totalChapters: 10, isbn: null);
        await db.AddRangeAsync(user, author, book);
        await db.SaveChangesAsync();
        Console.WriteLine($"[SEED] UserId: {user.Id}\n[SEED] BookId: {book.Id}");
    }
}

app.Run();
