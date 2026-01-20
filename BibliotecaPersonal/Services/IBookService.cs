using BibliotecaPersonal.Models;

namespace BibliotecaPersonal.Services;

/// <summary>
/// Interface para el servicio de gestión de libros
/// </summary>
public interface IBookService
{
    Task<List<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(Guid id);
    Task<Book> CreateBookAsync(Book book);
    Task<Book?> UpdateBookAsync(Book book);
    Task<bool> DeleteBookAsync(Guid id);
    Task<List<Book>> SearchBooksAsync(string query);
    Task<List<Book>> GetBooksByStatusAsync(BookStatus status);
}
