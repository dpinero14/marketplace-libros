using BibliotecaPersonal.Models;
using System.Text;
using System.Text.Json;

namespace BibliotecaPersonal.Services;

public class GeminiBookExtractionService : IBookExtractionService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GeminiBookExtractionService> _logger;

    public GeminiBookExtractionService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GeminiBookExtractionService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini API Key not configured");
        _logger = logger;
    }

    public async Task<BookExtractionResult> ExtractBookInfoFromImagesAsync(List<Stream> images)
    {
        try
        {
            var parts = new List<object>();

            // Agregar el prompt
            parts.Add(new
            {
                text = @"Analizá las imágenes del libro y extraé la información bibliográfica.

Respondé SOLO un JSON válido con esta estructura exacta:

{
  ""is_book"": true,
  ""title"": null,
  ""authors"": [],
  ""publisher"": null,
  ""publication_year"": null,
  ""isbn_13"": null,
  ""language"": null,
  ""format"": null,
  ""notes"": null,
  ""confidence"": 0
}

Reglas:
- No inventes datos.
- Si un campo no se ve claramente, dejalo en null.
- authors debe ser un array.
- confidence va de 0 a 1 según tu seguridad general.
- Si no es un libro, devolvé is_book=false y el resto null.
- NO agregues texto fuera del JSON."
            });

            // Agregar imágenes en base64
            foreach (var imageStream in images)
            {
                using var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64Image = Convert.ToBase64String(imageBytes);

                parts.Add(new
                {
                    inline_data = new
                    {
                        mime_type = "image/jpeg",
                        data = base64Image
                    }
                });
            }

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Gemini API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                throw new Exception($"Error al llamar a Gemini API: {response.StatusCode}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Gemini Response: {Response}", responseJson);

            var geminiResponse = JsonSerializer.Deserialize<JsonElement>(responseJson);

            // Extraer el texto de la respuesta
            var textResponse = geminiResponse
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            _logger.LogInformation("Extracted text: {Text}", textResponse);

            if (string.IsNullOrWhiteSpace(textResponse))
            {
                throw new Exception("Respuesta vacía de Gemini");
            }

            // Limpiar el texto si tiene markdown code blocks
            textResponse = textResponse.Trim();
            if (textResponse.StartsWith("```json"))
            {
                textResponse = textResponse.Substring(7);
            }
            if (textResponse.StartsWith("```"))
            {
                textResponse = textResponse.Substring(3);
            }
            if (textResponse.EndsWith("```"))
            {
                textResponse = textResponse.Substring(0, textResponse.Length - 3);
            }
            textResponse = textResponse.Trim();

            _logger.LogInformation("Cleaned JSON: {Json}", textResponse);

            // Parsear el JSON de la respuesta
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<BookExtractionResult>(textResponse, options);

            _logger.LogInformation("Parsed result - IsBook: {IsBook}, Title: {Title}", result?.IsBook, result?.Title);

            if (result == null)
            {
                throw new Exception("No se pudo parsear la respuesta de Gemini");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al extraer información del libro");
            return new BookExtractionResult
            {
                IsBook = false,
                Confidence = 0
            };
        }
    }
}
