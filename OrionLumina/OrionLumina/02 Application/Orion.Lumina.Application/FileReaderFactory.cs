using Orion.Lumina.Domain;
using System.Reflection;

namespace Orion.Lumina.Application
{
    /// <summary>
    /// Factory class responsible for creating appropriate file reader instances based on file extensions.
    /// </summary>
    public class FileReaderFactory
    {
        /// <summary>
        /// Dictionary mapping file extensions to their respective IFileReader implementation types.
        /// </summary>
        private readonly IDictionary<string, Type> _readers;

        /// <summary>
        /// Specifies the default reader type to use when no specific implementation exists.
        /// </summary>
        private readonly Type _defaultReaderType;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReaderFactory"/> class.
        /// Discovers all available <see cref="IFileReader"/> implementations and maps them to their extensions.
        /// </summary>
        public FileReaderFactory()
        {
            // Gather all IFileReader implementations and their associated extensions
            _readers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IFileReader).IsAssignableFrom(type) && !type.IsInterface)
                .SelectMany(type => type.GetCustomAttributes<FileReaderAttribute>()
                    .Select(attr => new { attr.Extension, ReaderType = type }))
                .ToDictionary(x => x.Extension.ToLower(), x => x.ReaderType);

            // Specify the default reader type (DocFileReader)
            _defaultReaderType = typeof(DocFileReader);
        }

        /// <summary>
        /// Retrieves an <see cref="IFileReader"/> implementation based on the specified file extension.
        /// If no specific reader exists for the extension, the default reader is returned.
        /// </summary>
        /// <param name="fileExtension">The file extension (e.g., ".doc").</param>
        /// <returns>An instance of an <see cref="IFileReader"/> implementation.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="fileExtension"/> is null or empty.</exception>
        public IFileReader GetFileReader(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new ArgumentException("File extension cannot be null or empty.", nameof(fileExtension));

            // Try to get a registered reader for the specified extension
            if (_readers.TryGetValue(fileExtension.ToLower(), out var readerType))
            {
                return (IFileReader)Activator.CreateInstance(readerType)!;
            }

            // Fall back to the default reader
            return (IFileReader)Activator.CreateInstance(_defaultReaderType)!;
        }
    }

    /// <summary>
    /// Custom attribute to associate file extensions with specific <see cref="IFileReader"/> implementations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FileReaderAttribute : Attribute
    {
        /// <summary>
        /// Gets the file extension associated with the <see cref="IFileReader"/> implementation.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileReaderAttribute"/> class.
        /// </summary>
        /// <param name="extension">The file extension (e.g., ".doc", ".pdf").</param>
        public FileReaderAttribute(string extension)
        {
            Extension = extension ?? throw new ArgumentNullException(nameof(extension));
        }
    }
}
