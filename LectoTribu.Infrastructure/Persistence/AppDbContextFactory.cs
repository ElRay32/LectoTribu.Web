using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LectoTribu.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Usa tu SQL. LocalDB por defecto:
        var conn = "Server=(localdb)\\MSSQLLocalDB;Database=LectoTribuDb;Trusted_Connection=True;TrustServerCertificate=True;";
        // Si usas SQLEXPRESS: var conn = "Server=.\\SQLEXPRESS;Database=LectoTribuDb;Trusted_Connection=True;TrustServerCertificate=True;";
        // Si usas usuario/clave: var conn = "Server=localhost,1433;Database=LectoTribuDb;User Id=sa;Password=TUCLAVE;TrustServerCertificate=True;";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(conn)
            .Options;

        return new AppDbContext(options);
    }
}

