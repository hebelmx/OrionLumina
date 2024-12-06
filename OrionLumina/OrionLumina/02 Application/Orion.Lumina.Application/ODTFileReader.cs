using AODL.Document.Content.Text;
using AODL.Document.TextDocuments; // AODL namespace
using Orion.Lumina.Domain;

namespace Orion.Lumina.Application
{
    [FileReader(".odt")]

    public class ODTFileReader : IFileReader
    {
        /// <summary>
        /// Reads the content of the `.odt` file.
        /// </summary>
        /// <param name="filePath">The path to the `.odt` file.</param>
        /// <returns>The text content of the `.odt` file.</returns>
        public string Read(string filePath)
        {
            ValidateFilePath(filePath);

            try
            {
                using var textDocument = new TextDocument();
                textDocument.Load(filePath);

                return ExtractTextContent(textDocument);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to read the .odt file.", ex);
            }
        }

        /// <summary>
        /// Extracts text content from the text document.
        /// </summary>
        /// <param name="textDocument">The loaded AODL text document.</param>
        /// <returns>The concatenated text content of the document.</returns>
        private string ExtractTextContent(TextDocument textDocument)
        {
            if (textDocument == null || textDocument.Content == null)
                return string.Empty;

            var contentBuilder = new System.Text.StringBuilder();

            foreach (var content in textDocument.Content)
            {
                if (content is not AODL.Document.Content.Text.Paragraph cont) continue;
                foreach (var textContent in cont.TextContent)
                {
                    if (textContent is not FormatedText formatedText) continue;
                    Console.WriteLine(formatedText.Text);
                    contentBuilder.Append(formatedText.Text);

                }
            }


            var result = contentBuilder.ToString();
            return result;
        }

        public IEnumerable<byte[]> ExtractImages(string filePath)
        {
            ValidateFilePath(filePath);

            try
            {
                using var textDocument = new TextDocument();
                textDocument.Load(filePath);

                return ExtractImagesFromDocument(textDocument);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to extract images from the .odt file.", ex);
            }
        }

        /// <summary>
        /// Extracts images from the `.odt` file.
        /// </summary>
        /// <param name="textDocument">The loaded AODL text document.</param>
        /// <returns>A collection of byte arrays representing the images.</returns>
        private IEnumerable<byte[]> ExtractImagesFromDocument(TextDocument textDocument)
        {
            var images = new List<byte[]>();

            foreach (var graphic in textDocument.Graphics)
            {

                images.Add(File.ReadAllBytes(graphic.ToString()));

            }

            return images;
        }

        public IEnumerable<Dictionary<string, string>> ExtractStructuredData(string filePath)
        {
            throw new NotImplementedException();
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
