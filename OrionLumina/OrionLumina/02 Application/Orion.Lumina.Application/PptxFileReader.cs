using DocumentFormat.OpenXml.Packaging;
using Orion.Lumina.Domain;
using System.Text;

namespace Orion.Lumina.Application;

[FileReader(".pptx")]
public class PptxFileReader : IFileReader
{
    /// <summary>
    /// Reads the text content from the specified PPTX file.
    /// </summary>
    /// <param name="filePath">The path to the PPTX file.</param>
    /// <returns>The extracted text content as a single string.</returns>
    public string Read(string filePath)
    {
        ValidateFilePath(filePath);

        var stringBuilder = new StringBuilder();

        using var presentationDocument = PresentationDocument.Open(filePath, false);
        var presentationPart = presentationDocument.PresentationPart;
        if (presentationPart == null) return string.Empty;

        foreach (var slidePart in presentationPart.SlideParts)
        {
            foreach (var text in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
            {
                stringBuilder.AppendLine(text.Text);
            }
        }

        return stringBuilder.ToString();
    }

    public IEnumerable<byte[]> ExtractImages(string filePath)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dictionary<string, string>> ExtractStructuredData(string filePath)
    {
        throw new NotImplementedException();
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