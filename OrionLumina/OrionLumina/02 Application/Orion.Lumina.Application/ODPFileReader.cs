using System;
using System.IO;
using System.Text;
using Orion.Lumina.Domain;


namespace Orion.Lumina.Application
{
    [FileReader(".odp")]
    public class OdpFileReader : IFileReader
    {


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
                string result = TextSanitizer.SanitizeText(File.ReadAllText(filePath));


                return  result ;
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
