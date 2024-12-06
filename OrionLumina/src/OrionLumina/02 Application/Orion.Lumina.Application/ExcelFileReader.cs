using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Orion.Lumina.Domain;

namespace Orion.Lumina.Application;

[FileReader(".xls")]
public class ExcelFileReader : IFileReader
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
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = filePath.EndsWith(".xls")
                ? (IWorkbook)new HSSFWorkbook(stream) // Handle .xls
                : new XSSFWorkbook(stream);           // Handle .xlsx

            var sheet = workbook.GetSheetAt(0); // Read the first sheet
            var allText = new List<string>();

            foreach (IRow row in sheet)
            {
                var rowText = string.Join(" ", GetRowValues(row));
                allText.Add(rowText);
            }

            return string.Join(Environment.NewLine, allText);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to read the Excel file.", ex);
        }
    }

    public IEnumerable<byte[]> ExtractImages(string filePath)
    {
        throw new NotImplementedException();
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
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = filePath.EndsWith(".xls")
                ? (IWorkbook)new HSSFWorkbook(stream) // Handle .xls
                : new XSSFWorkbook(stream);           // Handle .xlsx

            var sheet = workbook.GetSheetAt(0); // Read the first sheet
            var headers = GetRowValues(sheet.GetRow(0)); // First row as headers

            var structuredData = new List<Dictionary<string, string>>();

            for (int i = 1; i <= sheet.LastRowNum; i++) // Skip header row
            {
                var row = sheet.GetRow(i);
                if (row == null) continue;

                var rowData = new Dictionary<string, string>();
                var values = GetRowValues(row);

                for (int j = 0; j < headers.Count; j++)
                {
                    rowData[headers[j]] = j < values.Count ? values[j] : string.Empty;
                }

                structuredData.Add(rowData);
            }

            return structuredData;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to extract structured data from the Excel file.", ex);
        }
    }

    /// <summary>
    /// Extracts cell values from a given row.
    /// </summary>
    private static List<string> GetRowValues(IRow row)
    {
        var values = new List<string>();
        foreach (ICell cell in row.Cells)
        {
            values.Add(cell.ToString());
        }
        return values;
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
