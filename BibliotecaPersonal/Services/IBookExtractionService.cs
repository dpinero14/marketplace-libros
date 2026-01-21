using BibliotecaPersonal.Models;

namespace BibliotecaPersonal.Services;

public interface IBookExtractionService
{
    Task<BookExtractionResult> ExtractBookInfoFromImagesAsync(List<Stream> images);
}
