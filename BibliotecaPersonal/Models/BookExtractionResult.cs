using System.Text.Json.Serialization;

namespace BibliotecaPersonal.Models;

public class BookExtractionResult
{
    [JsonPropertyName("is_book")]
    public bool IsBook { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("authors")]
    public List<string> Authors { get; set; } = new();

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("publication_year")]
    public int? PublicationYear { get; set; }

    [JsonPropertyName("isbn_13")]
    public string? Isbn13 { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    [JsonPropertyName("confidence")]
    public decimal Confidence { get; set; }
}
