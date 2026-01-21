using BibliotecaPersonal.Models;
using System.Text.RegularExpressions;
using Tesseract;

namespace BibliotecaPersonal.Services;

public class TesseractBookExtractionService : IBookExtractionService
{
    private readonly ILogger<TesseractBookExtractionService> _logger;
    private readonly IWebHostEnvironment _environment;

    public TesseractBookExtractionService(
        ILogger<TesseractBookExtractionService> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async Task<BookExtractionResult> ExtractBookInfoFromImagesAsync(List<Stream> images)
    {
        try
        {
            var allText = new List<string>();

            // Ruta a los datos de Tesseract (tessdata)
            var tessdataPath = Path.Combine(_environment.ContentRootPath, "tessdata");

            // Si no existe, intentar con ruta del sistema
            if (!Directory.Exists(tessdataPath))
            {
                tessdataPath = @"C:\Program Files\Tesseract-OCR\tessdata";
            }

            if (!Directory.Exists(tessdataPath))
            {
                _logger.LogError("No se encontró tessdata. Instala Tesseract OCR o descarga tessdata.");
                return new BookExtractionResult { IsBook = false, Confidence = 0 };
            }

            // Procesar cada imagen
            foreach (var imageStream in images)
            {
                try
                {
                    _logger.LogInformation("Procesando imagen con Tesseract OCR...");

                    using var engine = new TesseractEngine(tessdataPath, "spa", EngineMode.Default);

                    // Convertir stream a array de bytes
                    using var memoryStream = new MemoryStream();
                    await imageStream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    _logger.LogInformation("Tamaño de imagen: {Size} bytes", imageBytes.Length);

                    // Procesar imagen con diferentes modos de segmentación
                    using var img = Pix.LoadFromMemory(imageBytes);
                    _logger.LogInformation("Imagen cargada: {Width}x{Height}", img.Width, img.Height);

                    string? text = null;

                    // Intentar con diferentes Page Segmentation Modes
                    var psmModes = new[] { PageSegMode.Auto, PageSegMode.SingleBlock, PageSegMode.SingleColumn };

                    foreach (var psm in psmModes)
                    {
                        try
                        {
                            using var page = engine.Process(img, psm);
                            var confidence = page.GetMeanConfidence();
                            text = page.GetText();

                            _logger.LogInformation("PSM {Mode}, Confianza: {Confidence:P}, Texto: {Length} caracteres",
                                psm, confidence, text?.Length ?? 0);

                            if (!string.IsNullOrWhiteSpace(text) && text.Length > 10)
                            {
                                _logger.LogInformation("Texto extraído exitosamente con modo {Mode}", psm);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Error con PSM {Mode}", psm);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        allText.Add(text);
                        _logger.LogInformation("Texto agregado exitosamente");
                    }
                    else
                    {
                        _logger.LogWarning("El OCR no pudo extraer texto de esta imagen con ningún modo");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar imagen con OCR");
                }
            }

            if (!allText.Any())
            {
                _logger.LogWarning("No se extrajo texto de ninguna imagen. La imagen puede tener baja calidad o estar en un formato no óptimo para OCR.");
                // Devolver resultado indicando que es un libro pero sin datos
                // para que el usuario pueda completar manualmente
                return new BookExtractionResult
                {
                    IsBook = true,
                    Confidence = 0,
                    Notes = "No se pudo extraer texto automáticamente. Por favor, completa los datos manualmente."
                };
            }

            // Combinar todo el texto extraído
            var combinedText = string.Join("\n", allText);
            _logger.LogInformation("Texto total extraído: {Text}", combinedText);

            // Extraer información del libro del texto
            var result = ExtractBookInfo(combinedText);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer información del libro con Tesseract");
            return new BookExtractionResult { IsBook = false, Confidence = 0 };
        }
    }

    private BookExtractionResult ExtractBookInfo(string text)
    {
        var result = new BookExtractionResult
        {
            IsBook = true,
            Authors = new List<string>()
        };

        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        // Buscar ISBN-13
        var isbn13Patterns = new[]
        {
            @"ISBN[\s-]?13[\s:]*(\d{3}[-\s]?\d{1,5}[-\s]?\d{1,7}[-\s]?\d{1,7}[-\s]?\d)",
            @"ISBN[\s:]*(\d{3}[-\s]?\d{1,5}[-\s]?\d{1,7}[-\s]?\d{1,7}[-\s]?\d)",
            @"(\d{3}[-\s]?\d{1,5}[-\s]?\d{1,7}[-\s]?\d{1,7}[-\s]?\d)"
        };

        foreach (var pattern in isbn13Patterns)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                result.Isbn13 = Regex.Replace(match.Groups[1].Value, @"[\s-]", "");
                if (result.Isbn13.Length == 13)
                {
                    break;
                }
            }
        }

        // Buscar año de publicación (4 dígitos entre 1900 y 2030)
        var yearMatch = Regex.Match(text, @"\b(19\d{2}|20[0-3]\d)\b");
        if (yearMatch.Success)
        {
            result.PublicationYear = int.Parse(yearMatch.Value);
        }

        // Buscar editorial (común en español)
        var publisherPatterns = new[]
        {
            @"Editorial[\s:]+([^\n]+)",
            @"Editora[\s:]+([^\n]+)",
            @"Publicado por[\s:]+([^\n]+)",
            @"Ediciones[\s]+([^\n]+)"
        };

        foreach (var pattern in publisherPatterns)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                result.Publisher = match.Groups[1].Value.Trim();
                break;
            }
        }

        // Buscar autor (patrones comunes)
        var authorPatterns = new[]
        {
            @"(?:Autor|Author|Por)[\s:]+([^\n]+)",
            @"(?:Escrito por|Written by)[\s:]+([^\n]+)"
        };

        foreach (var pattern in authorPatterns)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var authorText = match.Groups[1].Value.Trim();
                result.Authors.Add(authorText);
                break;
            }
        }

        // Intentar detectar título (generalmente las primeras líneas con texto significativo)
        var potentialTitles = lines
            .Where(l => l.Length > 3 && l.Length < 150)
            .Where(l => !l.Contains("ISBN", StringComparison.OrdinalIgnoreCase))
            .Where(l => !l.Contains("Editorial", StringComparison.OrdinalIgnoreCase))
            .Where(l => !l.Contains("Autor", StringComparison.OrdinalIgnoreCase))
            .Take(10)
            .ToList();

        if (potentialTitles.Any())
        {
            // Tomar la línea más larga como posible título
            result.Title = potentialTitles.OrderByDescending(t => t.Length).FirstOrDefault();
        }

        // Si no encontramos autor con patrón, intentar extraer de las líneas
        if (!result.Authors.Any() && potentialTitles.Count > 1)
        {
            // Tomar la segunda línea más larga como posible autor
            var possibleAuthor = potentialTitles.OrderByDescending(t => t.Length).Skip(1).FirstOrDefault();
            if (!string.IsNullOrEmpty(possibleAuthor))
            {
                result.Authors.Add(possibleAuthor);
            }
        }

        // Detectar idioma
        if (text.Contains("Spanish", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("Español", StringComparison.OrdinalIgnoreCase))
        {
            result.Language = "ES";
        }
        else if (text.Contains("English", StringComparison.OrdinalIgnoreCase) ||
                 text.Contains("Inglés", StringComparison.OrdinalIgnoreCase))
        {
            result.Language = "EN";
        }

        // Calcular confianza basada en campos encontrados
        var fieldsFound = 0;
        if (!string.IsNullOrEmpty(result.Title)) fieldsFound++;
        if (result.Authors.Any()) fieldsFound++;
        if (!string.IsNullOrEmpty(result.Isbn13)) fieldsFound++;
        if (result.PublicationYear.HasValue) fieldsFound++;
        if (!string.IsNullOrEmpty(result.Publisher)) fieldsFound++;

        result.Confidence = fieldsFound / 5.0m;

        // Siempre retornar IsBook = true si encontramos al menos un campo
        // El usuario puede corregir manualmente
        if (fieldsFound > 0)
        {
            result.IsBook = true;
            result.Notes = $"Información extraída automáticamente con {fieldsFound} campos detectados";
        }
        else
        {
            result.IsBook = false;
            result.Confidence = 0;
        }

        _logger.LogInformation("Extracción completada. Campos encontrados: {Fields}, Confianza: {Confidence}", fieldsFound, result.Confidence);

        return result;
    }
}
