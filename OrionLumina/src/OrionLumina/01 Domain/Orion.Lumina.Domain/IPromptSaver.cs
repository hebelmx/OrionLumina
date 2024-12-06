namespace Orion.Lumina.Domain;

public interface IPromptSaver
{
    Task SaveAsync(Prompt prompt);
}