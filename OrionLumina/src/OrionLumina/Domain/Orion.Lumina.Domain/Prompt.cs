namespace Orion.Lumina.Domain;

public class Prompt
{

    public Prompt(string message, string response)
    {
        Message = message;
        Response = response;

    }
    public string Message { get; set; } = String.Empty;
    public string Response { get; set; } = String.Empty;
}