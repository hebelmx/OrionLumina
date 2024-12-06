namespace Orion.Lumina.Domain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FileReaderAttribute(string extension) : Attribute
    {
        public string Extension { get; } = extension.ToLower();
    }

}
