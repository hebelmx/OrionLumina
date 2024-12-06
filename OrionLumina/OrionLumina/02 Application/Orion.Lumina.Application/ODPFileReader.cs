
//using AODL.Document.SpreadsheetDocuments; // AODL namespace for ODS
//using DocumentFormat.OpenXml.Packaging;
//using global::Orion.Lumina.Domain;
//using Orion.Lumina.Domain;
//using Orion.Lumina.Domain;

//namespace Orion.Lumina.Application;


//[FileReader(".odp")]
//    public class ODPFileReader : IFileReader
//    {
//        /// <summary>
//        /// Reads the content of the `.odp` file.
//        /// </summary>
//        /// <param name="filePath">The path to the `.odp` file.</param>
//        /// <returns>The text content of the `.odp` file.</returns>
//        public string Read(string filePath)
//        {
//            ValidateFilePath(filePath);

//            try
//            {
//                using var presentationDocument = new PresentationDocument();
//                presentationDocument.Load(filePath);

//                return ExtractTextContent(presentationDocument);
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException("Failed to read the .odp file.", ex);
//            }
//        }

//        public IEnumerable<byte[]> ExtractImages(string filePath)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<Dictionary<string, string>> ExtractStructuredData(string filePath)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Extracts text content from the presentation document.
//        /// </summary>
//        /// <param name="presentationDocument">The loaded AODL presentation document.</param>
//        /// <returns>The concatenated text content of all slides.</returns>
//        private string ExtractTextContent(PresentationDocument presentationDocument)
//        {
//            if (presentationDocument == null || presentationDocument.Content == null)
//                return string.Empty;

//            var contentBuilder = new System.Text.StringBuilder();

//            foreach (var content in presentationDocument.Content)
//            {
//                if (content is AODL.Document.Content.Text.Paragraph paragraph)
//                {
//                    foreach (var textContent in paragraph.TextContent)
//                    {
//                        if (textContent is AODL.Document.Content.Text.FormatedText formattedText)
//                        {
//                            contentBuilder.AppendLine(formattedText.Text);
//                        }
//                    }
//                }
//            }

//            return contentBuilder.ToString();
//        }

//        /// <summary>
//        /// Validates the file path for null, empty, or invalid file.
//        /// </summary>
//        private static void ValidateFilePath(string filePath)
//        {
//            if (string.IsNullOrWhiteSpace(filePath))
//                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
//            if (!File.Exists(filePath))
//                throw new FileNotFoundException("The specified file does not exist.", filePath);
//        }
//    }
//}
