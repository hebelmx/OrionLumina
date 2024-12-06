namespace Orion.Lumina.Domain;

public class Prompt(string message, string response)
{

    public Task SaveAsync(IPromptSaver Saver)
    {
        return Saver.SaveAsync(this);
    }

    public string Message { get; set; } = message;
    public string Response { get; set; } = response;
}