namespace Orion.Lumina.Domain;

public interface IProcessFile
{

    public FileStatus FileStatus { get; set; }

    public string Document { get; set; }

    public string Hash { get; set; }

    public IEnumerable<byte[]> Data { get; set; }

    public IEnumerable<float[]> Encodings { get; set; }

    public IEnumerable<Prompt> PromptList { get; set; }

    public IEnumerable<Prompt> ExtractPrompts();

    public IPromptExtractor PromptExtractor { get; set; }

    public IDocumentExtractor DocumentExtractor { get; set; }

}