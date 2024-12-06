using FluentAssertions;
using Microsoft.Extensions.AI;
using Xunit;

namespace DomainTests;

public class OllamaTestsChats
{

    [Theory]
    [InlineData("Tell me a Joke")]
    [InlineData("Tell me a Idea")]
    [InlineData("Tell me a Secret")]
    private static async Task TestOllamaResponse(string inputStr)
    {
     
        var uriString = "http://localhost:11434/";
        var modelId = "tinyllama";

        IChatClient client = new OllamaChatClient(new Uri(uriString), modelId); //Initialize chat client
        var response = await client.CompleteAsync(inputStr); //receives output asynchronously
        
        response.Should().NotBeNull();


        IEmbeddingGenerator<string, Embedding<float>> embed = new OllamaEmbeddingGenerator(new Uri(uriString), modelId);


        var messageLines = response.Message.ToString()
            .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

        var results = await embed.GenerateAsync(messageLines); //Generates embeddings asynchronously


        results.Should().NotBeNull();
        results.Count().Should().BeGreaterThan(0);

    }
}