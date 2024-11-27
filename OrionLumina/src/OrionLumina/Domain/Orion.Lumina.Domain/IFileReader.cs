namespace Orion.Lumina.Domain;

public interface IFileReader
{
    /// <summary>
    /// Reads the content of a file and returns it as a string.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <returns>Content of the file.</returns>
    string ReadContent(string filePath);

    /// <summary>
    /// Extracts images from the file.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <returns>A collection of images.</returns>
    IEnumerable<byte[]> ExtractImages(string filePath);

    /// <summary>
    /// Extracts structured data (e.g., tables) from the file.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <returns>Structured data as key-value pairs or other formats.</returns>
    IEnumerable<Dictionary<string, string>> ExtractStructuredData(string filePath);

}


