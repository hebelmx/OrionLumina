using GemBox.Presentation;
using Orion.Lumina.Domain;
using Shape = GemBox.Presentation.Shape;

namespace Orion.Lumina.Application
{
    [FileReader(".odp")]
    public class OdpFileReader : IFileReader
    {
        static OdpFileReader()
        {
            // Set the license key or use a free trial mode.
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        /// <summary>
        /// Reads the content of the `.odp` file.
        /// </summary>
        /// <param name="filePath">The path to the `.odp` file.</param>
        /// <returns>The text content of the `.odp` file.</returns>
        public string Read(string filePath)
        {
            ValidateFilePath(filePath);

            try
            {
                // Load the presentation document
                var presentation = PresentationDocument.Load(filePath);
                return ExtractTextContent(presentation);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to read the .odp file.", ex);
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
        /// Extracts text content from the presentation document.
        /// </summary>
        /// <param name="presentationDocument">The loaded GemBox.Presentation document.</param>
        /// <returns>The concatenated text content of all slides.</returns>
        private string ExtractTextContent(PresentationDocument presentationDocument)
        {
            if (presentationDocument == null)
                return string.Empty;

            var contentBuilder = new System.Text.StringBuilder();

            // Iterate over each slide in the presentation
            foreach (var slide in presentationDocument.Slides)
            {
                // Iterate over each shape in the slide's content
                foreach (var shape in slide.Content.Drawings)
                {
                    // Check if the shape has a non-null Text property
                    if (shape is Shape shapeWithText && shapeWithText.Text is not null)
                    {
                        foreach (var paragraph in shapeWithText.Text.Paragraphs)
                        {
                            foreach (var run in paragraph.Elements.OfType<TextRun>())
                            {
                                contentBuilder.AppendLine(run.Text);
                            }
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
