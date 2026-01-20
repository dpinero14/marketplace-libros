using BibliotecaPersonal.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaPersonal.Data;

/// <summary>
/// Contexto de base de datos para la aplicación de biblioteca personal.
/// Utiliza SQLite como motor de base de datos.
/// </summary>
public class BibliotecaDbContext : DbContext
{
    public BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Colección de libros en la biblioteca
    /// </summary>
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad Book
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Authors)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Publisher)
                .HasMaxLength(200);

            entity.Property(e => e.Isbn13)
                .HasMaxLength(17); // ISBN-13 con guiones

            entity.Property(e => e.Language)
                .HasMaxLength(10);

            entity.Property(e => e.Format)
                .HasMaxLength(50);

            entity.Property(e => e.Notes)
                .HasMaxLength(2000);

            entity.Property(e => e.Confidence)
                .HasPrecision(5, 4); // Permite valores como 0.9999

            entity.Property(e => e.Status)
                .HasConversion<string>(); // Guardar enum como string para legibilidad

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            // Índices para búsquedas rápidas
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Authors);
            entity.HasIndex(e => e.Isbn13);
            entity.HasIndex(e => e.Status);
        });
    }
}
