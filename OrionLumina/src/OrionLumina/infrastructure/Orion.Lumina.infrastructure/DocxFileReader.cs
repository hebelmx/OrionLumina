using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Orion.Lumina.Domain;

namespace Orion.Lumina.infrastructure
{
    public class DocxFileReader : IFileReader
    {
        /// <summary>
        /// Reads the text content from the specified DOCX file.
        /// </summary>
        /// <param name="filePath">The path to the DOCX file.</param>
        /// <returns>The extracted text content as a single string.</returns>
        public string ReadContent(string filePath)
        {
            ValidateFilePath(filePath);

            using var doc = WordprocessingDocument.Open(filePath, false);
            return doc.MainDocumentPart?.Document?.InnerText ?? string.Empty;
        }

        /// <summary>
        /// Extracts images from the specified DOCX file.
        /// </summary>
        /// <param name="filePath">The path to the DOCX file.</param>
        /// <returns>A collection of byte arrays representing the images.</returns>
        public IEnumerable<byte[]> ExtractImages(string filePath)
        {
            ValidateFilePath(filePath);

            var images = new List<byte[]>();
            using var doc = WordprocessingDocument.Open(filePath, false);
            if (doc.MainDocumentPart == null) return images;

            foreach (var part in doc.MainDocumentPart.ImageParts)
            {
                using var stream = part.GetStream();
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                images.Add(memoryStream.ToArray());
            }

            return images;
        }

        /// <summary>
        /// Extracts structured data (e.g., tables) from the DOCX file.
        /// </summary>
        /// <param name="filePath">The path to the DOCX file.</param>
        /// <returns>A collection of dictionaries representing table rows.</returns>
        public IEnumerable<Dictionary<string, string>> ExtractStructuredData(string filePath)
        {
            ValidateFilePath(filePath);

            var tables = new List<Dictionary<string, string>>();
            using var doc = WordprocessingDocument.Open(filePath, false);
            if (doc.MainDocumentPart?.Document?.Body == null) return tables;

            foreach (var table in doc.MainDocumentPart.Document.Body.Elements<Table>())
            {
                var rows = table.Elements<TableRow>().ToList();
                if (rows.Count < 2) continue; // Skip if there is no header or no data rows

                var headers = rows[0].Elements<TableCell>().Select(cell => cell.InnerText.Trim()).ToList();
                for (var i = 1; i < rows.Count; i++)
                {
                    var cells = rows[i].Elements<TableCell>().Select(cell => cell.InnerText.Trim()).ToList();
                    var rowDict = headers.Zip(cells, (header, value) => new { header, value })
                                         .ToDictionary(x => x.header, x => x.value);
                    tables.Add(rowDict);
                }
            }

            return tables;
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
}
