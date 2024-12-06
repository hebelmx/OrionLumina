using AODL.Document.Content.Tables;
using AODL.Document.SpreadsheetDocuments;
using Orion.Lumina.Domain;

namespace Orion.Lumina.Application
{
    [FileReader(".ods")]
    public class ODSFileReader : IFileReader
    {
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
                using var spreadsheetDocument = new SpreadsheetDocument();
                spreadsheetDocument.Load(filePath);

                return ExtractTextContent(spreadsheetDocument);
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
        /// Extracts text content from the spreadsheet document.
        /// </summary>
        /// <param name="spreadsheetDocument">The loaded AODL spreadsheet document.</param>
        /// <returns>The concatenated text content of all sheets.</returns>
        private string ExtractTextContent(SpreadsheetDocument spreadsheetDocument)
        {
            if (spreadsheetDocument == null || spreadsheetDocument.TableCollection == null)
                return string.Empty;

            var contentBuilder = new System.Text.StringBuilder();

            foreach (var table in spreadsheetDocument.TableCollection)
            {

                Table t = table as Table;

                foreach (var row in t.RowCollection)
                {

                    Row r = row as Row;

                    foreach (var cell in r.CellCollection)
                    {
                        Cell c = cell as Cell;
                        contentBuilder.Append(c.Document.XmlDoc.InnerText + " ");
                    }
                }
            }



            return contentBuilder.ToString();
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