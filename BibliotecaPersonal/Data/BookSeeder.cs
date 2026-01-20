using BibliotecaPersonal.Models;

namespace BibliotecaPersonal.Data;

/// <summary>
/// Clase para poblar la base de datos con datos de prueba.
/// Útil para desarrollo y demos.
/// </summary>
public static class BookSeeder
{
    /// <summary>
    /// Agrega libros de ejemplo si la base de datos está vacía
    /// </summary>
    public static async Task SeedAsync(BibliotecaDbContext context)
    {
        // Solo hacer seed si no hay libros
        if (context.Books.Any())
            return;

        var sampleBooks = new[]
        {
            new Book
            {
                Title = "El Quijote",
                Authors = "Miguel de Cervantes",
                Publisher = "Editorial Planeta",
                PublicationYear = 2015,
                Isbn13 = "978-8408142690",
                Language = "ES",
                Format = "Tapa blanda",
                Status = BookStatus.Keep,
                Confidence = 1.0m,
                Notes = "Edición conmemorativa"
            },
            new Book
            {
                Title = "Cien años de soledad",
                Authors = "Gabriel García Márquez",
                Publisher = "Sudamericana",
                PublicationYear = 1967,
                Isbn13 = "978-0307474728",
                Language = "ES",
                Format = "Tapa dura",
                Status = BookStatus.Keep,
                Confidence = 1.0m
            },
            new Book
            {
                Title = "1984",
                Authors = "George Orwell",
                Publisher = "Debolsillo",
                PublicationYear = 2013,
                Isbn13 = "978-8499890944",
                Language = "ES",
                Format = "Tapa blanda",
                Status = BookStatus.Sell,
                Confidence = 1.0m,
                Notes = "Traducción de Miguel Temprano García"
            },
            new Book
            {
                Title = "Clean Code",
                Authors = "Robert C. Martin",
                Publisher = "Prentice Hall",
                PublicationYear = 2008,
                Isbn13 = "978-0132350884",
                Language = "EN",
                Format = "Tapa blanda",
                Status = BookStatus.Keep,
                Confidence = 1.0m,
                Notes = "A Handbook of Agile Software Craftsmanship"
            },
            new Book
            {
                Title = "El principito",
                Authors = "Antoine de Saint-Exupéry",
                Publisher = "Salamandra",
                PublicationYear = 2015,
                Isbn13 = "978-8498381498",
                Language = "ES",
                Format = "Tapa dura",
                Status = BookStatus.Keep,
                Confidence = 1.0m
            },
            new Book
            {
                Title = "Rayuela",
                Authors = "Julio Cortázar",
                Publisher = "Alfaguara",
                PublicationYear = 1963,
                Isbn13 = "978-8420471891",
                Language = "ES",
                Format = "Tapa blanda",
                Status = BookStatus.Swap,
                Confidence = 1.0m
            },
            new Book
            {
                Title = "Harry Potter y la piedra filosofal",
                Authors = "J.K. Rowling",
                Publisher = "Salamandra",
                PublicationYear = 1997,
                Isbn13 = "978-8478884452",
                Language = "ES",
                Format = "Tapa dura",
                Status = BookStatus.Keep,
                Confidence = 1.0m,
                Notes = "Primera edición española"
            },
            new Book
            {
                Title = "Design Patterns",
                Authors = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
                Publisher = "Addison-Wesley",
                PublicationYear = 1994,
                Isbn13 = "978-0201633610",
                Language = "EN",
                Format = "Tapa dura",
                Status = BookStatus.Keep,
                Confidence = 1.0m,
                Notes = "Elements of Reusable Object-Oriented Software"
            },
            new Book
            {
                Title = "El nombre del viento",
                Authors = "Patrick Rothfuss",
                Publisher = "Plaza & Janés",
                PublicationYear = 2007,
                Isbn13 = "978-8401337208",
                Language = "ES",
                Format = "Tapa blanda",
                Status = BookStatus.Keep,
                Confidence = 1.0m,
                Notes = "Crónica del asesino de reyes: Primer día"
            },
            new Book
            {
                Title = "Ficciones",
                Authors = "Jorge Luis Borges",
                Publisher = "Emecé",
                PublicationYear = 1944,
                Isbn13 = "978-8499089515",
                Language = "ES",
                Format = "Tapa blanda",
                Status = BookStatus.Keep,
                Confidence = 1.0m
            }
        };

        await context.Books.AddRangeAsync(sampleBooks);
        await context.SaveChangesAsync();
    }
}
