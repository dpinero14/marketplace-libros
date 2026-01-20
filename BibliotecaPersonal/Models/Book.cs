namespace BibliotecaPersonal.Models;

/// <summary>
/// Representa un libro en la biblioteca personal.
/// Incluye información básica y estado para gestión futura (vender/intercambiar/conservar).
/// </summary>
public class Book
{
    /// <summary>
    /// Identificador único del libro
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Título del libro
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Autores del libro (puede ser múltiples separados por coma)
    /// </summary>
    public string Authors { get; set; } = string.Empty;

    /// <summary>
    /// Editorial/Publisher
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// Año de publicación
    /// </summary>
    public int? PublicationYear { get; set; }

    /// <summary>
    /// ISBN-13 del libro
    /// </summary>
    public string? Isbn13 { get; set; }

    /// <summary>
    /// Idioma del libro (ej: ES, EN, PT)
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Formato del libro (ej: Tapa blanda, Tapa dura, Digital)
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// Notas adicionales sobre el libro
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Nivel de confianza en la extracción de datos (0.0 a 1.0)
    /// Útil cuando se extrae información desde imágenes con IA
    /// </summary>
    public decimal Confidence { get; set; } = 1.0m;

    /// <summary>
    /// Estado del libro: Keep (conservar), Sell (vender), Swap (intercambiar)
    /// </summary>
    public BookStatus Status { get; set; } = BookStatus.Keep;

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Estado del libro en la biblioteca
/// </summary>
public enum BookStatus
{
    /// <summary>
    /// Conservar en la biblioteca
    /// </summary>
    Keep = 0,

    /// <summary>
    /// Disponible para vender
    /// </summary>
    Sell = 1,

    /// <summary>
    /// Disponible para intercambiar
    /// </summary>
    Swap = 2
}
