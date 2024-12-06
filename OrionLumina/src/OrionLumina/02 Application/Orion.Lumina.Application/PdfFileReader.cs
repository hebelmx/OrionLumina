using Orion.Lumina.Domain;
using UglyToad.PdfPig;

namespace Orion.Lumina.Application;

[FileReader(".pdf")]

public class PdfFileReader : IFileReader
{
    /// <summary>
    /// Reads the text content from the specified PDF file.
    /// </summary>
    /// <param name="filePath">The path to the PDF file.</param>
    /// <returns>The extracted text content.</returns>
    public string Read(string filePath)
    {
        ValidateFilePath(filePath);

        using var document = PdfDocument.Open(filePath);
        var allText = string.Join(Environment.NewLine,
            document.GetPages().Select(page => string.Join(string.Empty, page.Letters.Select(letter => letter.Value))));

        return allText;
    }

    /// <summary>
    /// Extracts images from the specified PDF file.
    /// </summary>
    /// <param name="filePath">The path to the PDF file.</param>
    /// <returns>A collection of byte arrays representing the images.</returns>
    public IEnumerable<byte[]> ExtractImages(string filePath)
    {
        ValidateFilePath(filePath);

        using var document = PdfDocument.Open(filePath);
        var images = new List<byte[]>();

        foreach (var page in document.GetPages())
        {
            images.AddRange(page.GetImages().Select(image => image.RawBytes.ToArray()));
        }

        return images;
    }

    /// <summary>
    /// Extracts structured data (e.g., tables) from the PDF file.
    /// </summary>
    /// <param name="filePath">The path to the PDF file.</param>
    /// <returns>Structured data as key-value pairs or other formats.</returns>
    public IEnumerable<Dictionary<string, string>> ExtractStructuredData(string filePath)
    {
        // Implement your structured data extraction logic.
        // For now, it returns an empty collection.
        return Enumerable.Empty<Dictionary<string, string>>();
    }

    /// <summary>
    /// Validates the file path for null, empty, or invalid file.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    private static void ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        if (!File.Exists(filePath))
            throw new FileNotFoundException("The specified file does not exist.", filePath);
    }
}
