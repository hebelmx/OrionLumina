namespace Orion.Lumina.Domain;

public interface IPromptExtractor
{
    public IEnumerable<Prompt> ExtractPrompts();

}