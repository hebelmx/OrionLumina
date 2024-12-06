using Microsoft.Extensions.AI;
using Orion.Lumina.Domain;
using System.Text.RegularExpressions;

namespace OrionLumina.Infrastructure.CloudToEmbeddings;
public class LlamaQueryExtractor : QueryExtractor

{
    private static readonly Regex Pattern =
        new(pattern: @"\{\s*""Message"":\s*""(.*?)"",\s*""Response"":\s*""(.*?)""\s*\}",
            options: RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string UriString = $"http://localhost:11434/";
    private const string ModelId = "llama3.1";

    private const string SystemPrompt = @"
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

    private const string EndDoc = @" </Doc>";


    public async Task<List<Prompt>> ExtractQueryPrompts(string document)
    {

        if (document is null || document.Length <= 0) return new List<Prompt>();



        var prompt = SystemPrompt + document + EndDoc;


        IChatClient client = new OllamaChatClient(new Uri(UriString), ModelId); //Initialize chat client
        var response = await client.CompleteAsync(prompt,
            cancellationToken: CancellationToken.None); //receives output asynchronously

        var messages = response.Message.ToString();


        //return early if no matches
        if (!Pattern.IsMatch(messages)) return new List<Prompt>();

        // Find matches
        var matches = Pattern.Matches(messages);

        // Convert matches to a list of Prompt objects
        var prompts = matches.Select(match => match.Groups).
            Select(match =>
                new Prompt(match[1].Value, match[2].Value)).ToList();



        return prompts;
    }

}
