
namespace Orion.Lumina.Domain;

public interface QueryExtractor
{

    Task<List<Prompt>> ExtractQueryPrompts(string document);

}