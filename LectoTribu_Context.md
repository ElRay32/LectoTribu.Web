# Documentación: Contexto de Datos y Ciclo de Vida de Aplicación (LectoTribu)

## Resumen ejecutivo
- **Propósito:** Plataforma donde las personas pueden unirse a clubes de lectura, comentar capítulos, establecer cronogramas y recomendar libros mediante reseñas personales.

# Documentación: Contexto de Datos y Ciclo de Vida de Aplicación (LectoTribu)

## 1) Visión general de la arquitectura
- **LectoTribu.Api** (ASP.NET Core Web API)  
  Expone endpoints REST. Configura EF Core, CORS y aplica migraciones/seed al arrancar.
- **LectoTribu.Web** (Razor Pages)  
  Cliente web. Usa `HttpClient` con `ApiBaseUrl` y servicios tipados (`IClubsApi`, `IBooksApi`, `IUsersApi`).
- **LectoTribu.Infrastructure**  
  Contiene `AppDbContext` (EF Core), migraciones, repositorio (`EfRepository`) y contratos de UoW.
- **LectoTribu.Domain**  
  Entidades de dominio (p.ej. `User`, `Book`, `Club`, `ReadingSchedule`, `Comment`, etc.) y value objects.

---

## 2) Contexto de datos: `AppDbContext`
**Archivo:** `LectoTribu.Infrastructure/Persistence/AppDbContext.cs`  
**Herencia:** `DbContext`, implementa `IUnitOfWork`.

### 2.1 DbSets principales
- `Users` — Usuarios del sistema (propiedad calculada `Name`).
- `Authors` — Autores (para los libros).
- `Books` — Libros (con `TotalChapters`, `Format`, etc.).
- `Clubs` — Clubes de lectura (agregan libros, miembros, programaciones).
- `Memberships` — Relación Usuario–Club (miembros).
- `Comments` — Comentarios por libro/capítulo en un club.
- `Schedules` — Programaciones de lectura por capítulo (`ReadingSchedule`).
- `Reviews` — Reseñas (si están presentes).

> Nota: Algunos accesores se exponen como `=> Set<T>()` para simplificar.

### 2.2 Configuración (modelado)
- **Convenciones clave**: longitudes, `IsRequired`, ignorar propiedades no persistibles.  
  Ejemplo:
  ```csharp
  modelBuilder.Entity<Club>(b =>
  {
      b.Property(x => x.Name).HasMaxLength(200).IsRequired();
      b.Ignore(x => x.Books);
  });
  ```
- **Relaciones**:  
  - `ReadingSchedule` tiene `ClubId`, `BookId`, `ChapterNumber`, `PlannedDate`.  
  - `Comment` referencia `ClubId / BookId / ChapterNumber / UserId`.
- **Índices/Restricciones** (recomendado): clave única `(ClubId, BookId, ChapterNumber)` en `Schedules` para evitar duplicados.

### 2.3 Unidad de trabajo (UoW)
`AppDbContext` implementa `IUnitOfWork`, por lo que **`SaveChangesAsync`** centraliza la confirmación de cambios.

---

## 3) Configuración de la base de datos
### 3.1 Cadena de conexión
- **Archivo:** `LectoTribu.Api/appsettings.json`
- **Clave:** `"ConnectionStrings:Default"`  
  Ejemplo (LocalDB):
  ```
  Server=(localdb)\\MSSQLLocalDB;Database=LectoTribuDb;Trusted_Connection=True;TrustServerCertificate=True;
  ```
Si no usas LocalDB, ajústala (p. ej. `.\SQLEXPRESS` o SQL Server remoto).

### 3.2 Migraciones
- Migraciones en `LectoTribu.Infrastructure/Migrations`.
- **Aplicación automática** al iniciar la API:
  ```csharp
  using var scope = app.Services.CreateScope();
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
  await db.Database.MigrateAsync();
  ```

> Manual (opcional):  
> ```bash
> dotnet ef migrations add <Nombre>
> dotnet ef database update --project LectoTribu.Infrastructure --startup-project LectoTribu.Api
> ```

---

## 4) Inicialización de la API (`Program.cs` – LectoTribu.Api)

### 4.1 Registro de servicios (resumen)
- EF Core con SQL Server: `AddDbContext<AppDbContext>(...)`
- CORS: política para permitir el origen de la Web.
- Servicios de aplicación: `IClubService`, etc.

### 4.2 Migraciones y Seed
- **Migrar** al iniciar (ver 3.2).
- **Seed opcional**:
  ```csharp
  if (!await db.Users.AnyAsync())
  {
      var user = new User("Juan", "Pérez", email: null);
      var author = new Author("Autor Demo");
      var book = new Book("Libro Demo", author.Id, totalChapters: 10, /* ... */);
      await db.AddRangeAsync(user, author, book);
      await db.SaveChangesAsync();
  }
  ```

---

## 5) Cliente Web (`Program.cs` – LectoTribu.Web)
- **ApiBaseUrl** configurable: `appsettings.json` → `"ApiBaseUrl": "https://localhost:7261"`
- **HttpClients tipados**:
  ```csharp
  builder.Services.AddHttpClient<IClubsApi, ClubsApi>(c => c.BaseAddress = new Uri(baseUrl));
  builder.Services.AddHttpClient<IUsersApi, UsersApi>(c => c.BaseAddress = new Uri(baseUrl));
  builder.Services.AddHttpClient<IBooksApi, BooksApi>(c => c.BaseAddress = new Uri(baseUrl));
  ```
- Razor Pages: `app.MapRazorPages();`

> Asegúrate de que **los puertos no colisionen** entre Web y API y que `ApiBaseUrl` coincide con el puerto real de la API.

---

## 6) Flujos clave y uso del contexto

### 6.1 Programar un capítulo (Schedule)
1. **Web** (`/Clubs/Schedule`) → `POST /api/clubs/{clubId}/schedule/one` con `{ bookId, chapter, date }`.
2. **API** `ClubsController.ScheduleOne` valida y llama `IClubService.ScheduleAsync(...)`.
3. **Servicio** carga `Club` desde `AppDbContext`, crea `ReadingSchedule` y guarda.
4. **DB** inserta en `Schedules`.

**Recomendación**: índice único `(ClubId, BookId, ChapterNumber)` en `Schedules`.

### 6.2 Publicar comentario (Comments)
1. **Web** (`/Read`) permite elegir **Club** y **Libro**, ingresar **Nombre** y comentario.  
   El cliente resuelve/crea `UserId` con `UsersApi.GetOrCreateByNameAsync`.
2. **Web** → `POST /api/clubs/{clubId}/comment` con `{ bookId, chapter, userId, content }`.
3. **API** → `IClubService.CommentAsync(...)`.
4. **DB** inserta en `Comments`.

---

## 7) Ejemplos rápidos

### 7.1 Consultar comentarios (Controller + LINQ join)
```csharp
[HttpGet("{id}/comments")]
public async Task<ActionResult<IEnumerable<CommentDtoOut>>> GetComments(
    Guid id, [FromQuery] Guid bookId, [FromQuery] int chapter,
    [FromServices] AppDbContext db, CancellationToken ct)
{
    var list = await (
        from c in db.Comments
        join u in db.Users on c.UserId equals u.Id
        where c.ClubId == id && c.BookId == bookId && c.ChapterNumber == chapter
        orderby c.CreatedAtUtc
        select new CommentDtoOut(c.UserId, u.Name, c.Content, c.CreatedAtUtc)
    ).ToListAsync(ct);

    return Ok(list);
}
```

### 7.2 Repositorio genérico
```csharp
public class EfRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _db;
    public EfRepository(AppDbContext db) => _db = db;

    public Task<T?> GetAsync(Guid id, CancellationToken ct)
        => _db.Set<T>().FindAsync(new object[] { id }, ct).AsTask();

    public Task AddAsync(T entity, CancellationToken ct)
        => _db.Set<T>().AddAsync(entity, ct).AsTask();

    public Task SaveAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}
```

---

## 8) Añadir una nueva entidad (paso a paso)
1. Crear entidad en `Domain/Entities` (hereda de `BaseEntity` si aplica).
2. Agregar `DbSet` en `AppDbContext`: `public DbSet<MiEntidad> MiEntidades => Set<MiEntidad>();`
3. Configurar en `OnModelCreating` (longitudes, índices, relaciones).
4. Generar migración: `dotnet ef migrations add AddMiEntidad`  
   Aplicar: `dotnet ef database update` (o iniciar la API para migración automática).
5. Exponer en API (controller/servicio/DTOs).
6. Consumir en la Web vía `HttpClient` tipado.

---

## 9) Errores frecuentes y cómo evitarlos
- **500 al programar** → Club/Libro inexistente o datos inválidos.  
  *Solución:* validar en controller (devolver **400/404**) y en Web no usar `EnsureSuccessStatusCode` en flujos esperados de validación.
- **CORS bloqueando la Web** → registrar `UseCors` en API con el origen correcto.
- **Puertos mal configurados** → usar puertos distintos y alinear `ApiBaseUrl`.
- **`record` detectado como controller** → no colocar `[ApiController]/[Route]` sobre records; usar `[NonController]` o moverlos fuera.

---

## 10) Checklist rápido
- [ ] `ApiBaseUrl` correcto en la Web.  
- [ ] Puertos distintos entre Web y API.  
- [ ] Cadena de conexión válida.  
- [ ] Migraciones aplicadas.  
- [ ] CORS configurado.  
- [ ] Controllers devuelven 400/404 en validaciones.  
- [ ] Servicios/Repos usan `AppDbContext` por DI.

---

**Contacto / mantenimiento**  
- Mantener sincronía de DTOs entre API y Web.  
- Versionar migraciones y revisar índices/constraints ante nuevas reglas.  
- Agregar logs en `Program.cs` de la API para diagnósticos de arranque/migración.
