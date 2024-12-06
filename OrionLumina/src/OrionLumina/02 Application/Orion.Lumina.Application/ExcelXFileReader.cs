using ClosedXML.Excel;
using Orion.Lumina.Domain;

namespace Orion.Lumina.Application;

[FileReader(".xlsx")]
public class ExcelXFileReader : IFileReader
{
    /// <summary>
    /// Reads the text content from the specified Excel file.
    /// Combines all cell values into a single text output, row by row.
    /// </summary>
    /// <param name="filePath">The path to the Excel file.</param>
    /// <returns>The extracted text content from the Excel file.</returns>
    public string Read(string filePath)
    {
        ValidateFilePath(filePath);

        try
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1); // Read the first worksheet.
            var allText = new List<string>();

            foreach (var row in worksheet.RowsUsed())
            {
                var rowText = string.Join(" ", row.CellsUsed().Select(cell => cell.Value.ToString()));
                allText.Add(rowText);
            }

            return string.Join(Environment.NewLine, allText);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to read the Excel file.", ex);
        }
    }

    /// <summary>
    /// Throws an exception as image extraction is not supported for Excel files.
    /// </summary>
    /// <param name="filePath">The path to the Excel file.</param>
    /// <returns>Throws a <see cref="NotSupportedException"/>.</returns>
    public IEnumerable<byte[]> ExtractImages(string filePath)
    {
        throw new NotSupportedException("Image extraction is not supported for Excel files.");
    }

    /// <summary>
    /// Extracts structured data (e.g., tables) from the Excel file.
    /// Each row is returned as a dictionary with column headers as keys.
    /// </summary>
    /// <param name="filePath">The path to the Excel file.</param>
    /// <returns>A collection of dictionaries representing rows of data.</returns>
    public IEnumerable<Dictionary<string, string>> ExtractStructuredData(string filePath)
    {
        ValidateFilePath(filePath);

        try
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1); // Read the first worksheet.
            var headers = worksheet.Row(1).CellsUsed().Select(cell => cell.Value.ToString()).ToList();

            return worksheet.RowsUsed()
                .Skip(1) // Skip the header row.
                .Select(row => headers
                    .Zip(row.CellsUsed().Select(cell => cell.Value.ToString()), (header, value) => new { header, value })
                    .ToDictionary(x => x.header, x => x.value))
                .ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to extract structured data from the Excel file.", ex);
        }
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
