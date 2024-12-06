
using global::Orion.Lumina.Domain;

namespace Orion.Lumina.Application;

[FileReader(".txt")]
public class TxtFileReader : IFileReader
{
    /// <summary>
    /// Reads the content of a text file.
    /// </summary>
    /// <param name="filePath">The path to the text file.</param>
    /// <returns>The text content of the file.</returns>
    public string Read(string filePath)
    {
        ValidateFilePath(filePath);

        try
        {
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to read the text file.", ex);
        }
    }

    /// <summary>
    /// Throws an exception as image extraction is not supported for text files.
    /// </summary>
    /// <param name="filePath">The path to the text file.</param>
    /// <returns>Throws a <see cref="NotSupportedException"/>.</returns>
    public IEnumerable<byte[]> ExtractImages(string filePath)
    {
        throw new NotSupportedException("Image extraction is not supported for text files.");
    }

    /// <summary>
    /// Throws an exception as structured data extraction is not supported for text files.
    /// </summary>
    /// <param name="filePath">The path to the text file.</param>
    /// <returns>Throws a <see cref="NotSupportedException"/>.</returns>
    public IEnumerable<Dictionary<string, string>> ExtractStructuredData(string filePath)
    {
        throw new NotSupportedException("Structured data extraction is not supported for text files.");
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

