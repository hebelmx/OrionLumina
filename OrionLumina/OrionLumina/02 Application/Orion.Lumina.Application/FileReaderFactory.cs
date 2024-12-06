using Orion.Lumina.Domain;
using System.Reflection;

namespace Orion.Lumina.Application
{
    public class FileReaderFactory
    {
        private readonly IDictionary<string, Type> _readers;

        public FileReaderFactory()
        {
            _readers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IFileReader).IsAssignableFrom(type) && !type.IsInterface)
                .SelectMany(type => type.GetCustomAttributes<FileReaderAttribute>()
                    .Select(attr => new { attr.Extension, ReaderType = type }))
                .ToDictionary(x => x.Extension, x => x.ReaderType);
        }

        public IFileReader GetFileReader(string fileExtension)
        {
            if (_readers.TryGetValue(fileExtension.ToLower(), out var readerType))
            {
                return (IFileReader)Activator.CreateInstance(readerType)!;
            }
            throw new NotSupportedException($"File extension '{fileExtension}' is not supported.");
        }
    }

}
