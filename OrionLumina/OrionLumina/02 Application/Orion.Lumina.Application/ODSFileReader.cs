using GemBox.Spreadsheet;
using Orion.Lumina.Domain;

namespace Orion.Lumina.Application
{
    [FileReader(".ods")]
    public class OdsFileReader : IFileReader
    {
        static OdsFileReader()
        {
            // Set the license key or use a free trial mode.
            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
        }

        /// <summary>
        /// Reads the content of the `.ods` file.
        /// </summary>
        /// <param name="filePath">The path to the `.ods` file.</param>
        /// <returns>The text content of the `.ods` file.</returns>
        public string Read(string filePath)
        {
            ValidateFilePath(filePath);

            try
            {
                var workbook = ExcelFile.Load(filePath);
                return ExtractTextContent(workbook);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to read the .ods file.", ex);
            }
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
        /// Extracts text content from the workbook.
        /// </summary>
        /// <param name="workbook">The loaded GemBox.Spreadsheet workbook.</param>
        /// <returns>The concatenated text content of all sheets.</returns>
        private string ExtractTextContent(ExcelFile workbook)
        {
            var contentBuilder = new System.Text.StringBuilder();

            foreach (var worksheet in workbook.Worksheets)
            {
                foreach (var row in worksheet.Rows)
                {
                    foreach (var cell in row.AllocatedCells)
                    {
                        if (cell.ValueType != CellValueType.Null)
                        {
                            contentBuilder.Append(cell.Value.ToString() + " ");
                        }
                    }
                }
            }

            return contentBuilder.ToString().Trim();
        }

        /// <summary>
        /// Validates the file path for null, empty, or invalid file.
        /// </summary>
        private static void ValidateFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file does not exist.", filePath);
        }
    }
}
