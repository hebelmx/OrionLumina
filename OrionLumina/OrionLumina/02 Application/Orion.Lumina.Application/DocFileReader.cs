using IFilterTextReader;
using Orion.Lumina.Domain;
using File = System.IO.File;

namespace Orion.Lumina.Application
{

    [FileReader(".doc")]
    public class DocFileReader : IFileReader
    {
        /// <summary>
        /// Reads the content of the `.doc` file.
        /// </summary>
        /// <param name="filePath">The path to the `.doc` file.</param>
        /// <returns>The text content of the `.doc` file.</returns>
        public string Read(string filePath)
        {
            ValidateFilePath(filePath);

            try
            {

                string txt;
                TextReader reader = new FilterReader(filePath);
                using (reader)
                {
                    txt = reader.ReadToEnd();
                }

                return txt;


            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to read the file.", ex);
            }
        }



        /// <summary>
        /// Extracts images from the `.doc` file.
        /// </summary>
        /// <param name="filePath">The path to the `.doc` file.</param>
        /// <returns>A collection of byte arrays representing the images.</returns>
        public IEnumerable<byte[]> ExtractImages(string filePath)
        {
            ValidateFilePath(filePath);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Extracts images from the Word document.
        /// </summary>
        /// <param name="document">The loaded HWPF document.</param>
        /// <returns>A collection of byte arrays representing the images.</returns>
        private IEnumerable<byte[]> ExtractImagesFromDocument(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Extracts structured data from the `.doc` file (not implemented).
        /// </summary>
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
