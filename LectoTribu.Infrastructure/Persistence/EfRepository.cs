using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace LectoTribu.Infrastructure.Persistence;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _db;
    public EfRepository(AppDbContext db) => _db = db;

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Set<T>().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
        => await _db.AddAsync(entity, ct);

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    { _db.Update(entity); return Task.CompletedTask; }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    { _db.Remove(entity); return Task.CompletedTask; }
}
