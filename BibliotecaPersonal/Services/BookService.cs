using BibliotecaPersonal.Data;
using BibliotecaPersonal.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaPersonal.Services;

/// <summary>
/// Servicio para gestionar operaciones CRUD sobre libros.
/// Incluye búsqueda rápida para la funcionalidad "¿Lo tengo?"
/// </summary>
public class BookService : IBookService
{
    private readonly BibliotecaDbContext _context;

    public BookService(BibliotecaDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los libros ordenados por título
    /// </summary>
    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene un libro por su ID
    /// </summary>
    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await _context.Books.FindAsync(id);
    }

    /// <summary>
    /// Crea un nuevo libro en la biblioteca
    /// </summary>
    public async Task<Book> CreateBookAsync(Book book)
    {
        book.Id = Guid.NewGuid();
        book.CreatedAt = DateTime.UtcNow;

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return book;
    }

    /// <summary>
    /// Actualiza un libro existente
    /// </summary>
    public async Task<Book?> UpdateBookAsync(Book book)
    {
        var existing = await _context.Books.FindAsync(book.Id);
        if (existing == null)
            return null;

        // Actualizar propiedades (mantener CreatedAt original)
        existing.Title = book.Title;
        existing.Authors = book.Authors;
        existing.Publisher = book.Publisher;
        existing.PublicationYear = book.PublicationYear;
        existing.Isbn13 = book.Isbn13;
        existing.Language = book.Language;
        existing.Format = book.Format;
        existing.Notes = book.Notes;
        existing.Confidence = book.Confidence;
        existing.Status = book.Status;

        await _context.SaveChangesAsync();
        return existing;
    }

    /// <summary>
    /// Elimina un libro de la biblioteca
    /// </summary>
    public async Task<bool> DeleteBookAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Busca libros por título, autor o ISBN.
    /// Búsqueda case-insensitive para facilitar consultas rápidas desde el móvil.
    /// </summary>
    public async Task<List<Book>> SearchBooksAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return await GetAllBooksAsync();

        var searchTerm = query.ToLower();

        return await _context.Books
            .Where(b =>
                b.Title.ToLower().Contains(searchTerm) ||
                b.Authors.ToLower().Contains(searchTerm) ||
                (b.Isbn13 != null && b.Isbn13.Contains(searchTerm)))
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene libros filtrados por estado (Keep, Sell, Swap)
    /// </summary>
    public async Task<List<Book>> GetBooksByStatusAsync(BookStatus status)
    {
        return await _context.Books
            .Where(b => b.Status == status)
            .OrderBy(b => b.Title)
            .ToListAsync();
    }
}
