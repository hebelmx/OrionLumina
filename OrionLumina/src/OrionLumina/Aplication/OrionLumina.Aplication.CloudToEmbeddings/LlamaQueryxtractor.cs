using Microsoft.Extensions.AI;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Orion.Lumina.Domain;
using System.Text.RegularExpressions;


namespace OrionLumina.Application.CloudToEmbeddings;
public class LlamaQueryExtractor : QueryExtractor

{

    public async Task<List<Prompt>> ExtractQueryPrompts(string document)
    {

        if (document is null || document.Length <= 0) return new List<Prompt>();

        var systemPrompt = @"
                Generate a list of synthetic queries based on the document provided between <Doc> and </Doc>Below.
            Please adhere to this rules:
            1. Each query should have a concise 'Message' that reflects an intent, topic, or question.
            2. Each query should have a realistic 'Response' that provides relevant and coherent information.
            3. Ensure that the structure is consistent and suitable for JSON serialization, with the keys:
               - 'Message' for the query prompt
               - 'Response' for the answer. 
            4. Start with the first Message without any header
            5. Finish with the las Response without any comment
            6. Don't Mention this rules
            
            <Doc>";

        var prompt = systemPrompt + document + " </Doc>";


        var uriString = "http://localhost:11434/";
        var modelId = "llama3.1";

        IChatClient client = new OllamaChatClient(new Uri(uriString), modelId); //Initialize chat client
        var response = await client.CompleteAsync(prompt,
            cancellationToken: CancellationToken.None); //receives output asynchronously

        var messages = response.Message.ToString();

        string pattern = @"\{\s*""Message"":\s*""(.*?)"",\s*""Response"":\s*""(.*?)""\s*\}";

        // Find matches
        var matches = Regex.Matches(messages, pattern);

        // Convert matches to a list of Prompt objects
        var prompts = new List<Prompt>();
        foreach (Match match in matches)
        {
            if (match.Groups.Count == 3) // Ensure we have both groups
            {
                prompts.Add(new Prompt
                {
                    Message = match.Groups[1].Value,
                    Response = match.Groups[2].Value
                });
            }
        }

        return prompts;
    }

}
