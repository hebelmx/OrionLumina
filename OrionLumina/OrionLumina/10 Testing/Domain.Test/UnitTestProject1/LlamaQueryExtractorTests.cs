using FluentAssertions;
using Orion.Lumina.Domain;
using OrionLumina.Infrastructure.CloudToEmbeddings;
using Xunit;

namespace DomainTests;

public class LlamaQueryExtractorTests
{
    private readonly LlamaQueryExtractor _llamaQueryExtractor;

    public LlamaQueryExtractorTests()
    {
        _llamaQueryExtractor = new LlamaQueryExtractor();
    }
    public readonly string _document = @"My cat’s name is Stuart. He is a Maine Coon cat that is famous for its furry looks. Stuart is very dear to me. His walk is majestic, and he loves to hop around the house while carrying all his grace in his golden fur. Although very majestic, when he sleeps, his postures are funny to look at. 

Most of the time he stays at home playing with the ball we got him. But at times he also goes in the backyard for a stroll. He loves watching the birds from the window in my room. I have always wanted a pet cat and when my dad brought home Stuart, I was the happiest. He came home curdled like a white snowball. The cats of his breed live in cold climates, hence we have to ensure that our house is airconditioned properly, especially at night. Now, because they are habitual to such cold climates, the fur of Stuart is amazingly fluffy. We also have to take extra precautions so that Stuart doesn’t feel too much heat. 

Cats require a lot of attention and care. We take care of Stuart’s meals like we would of a baby. We feed him twice a day and make sure that he gets all the necessary nutrients through his meals. We also bathe him twice a week. Another important thing that we made sure of is that he got all his vaccinations done on time. And periodically we visit the vet to make sure that he is healthy. Although domesticated, he still likes to chase around birds. When some pigeons sit on the window, he chases them away. 

Everyone in our family love loves Stuart. We all take care of him and love him with all our hearts. He is an important member of our family.";

    [Fact]
    public async Task ExtractQueryPrompts_ShouldReturnListOfPrompts_WhenDocumentIsValid()
    {
        // Arrange


        // Act
        var result = await _llamaQueryExtractor.ExtractQueryPrompts(_document);


        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<List<Prompt>>()
            .And.NotBeEmpty();
    }

    [Fact]
    public async Task ExtractQueryPrompts_ShouldReturnEmptyList_WhenDocumentIsEmpty()
    {
        // Arrange
        string document = string.Empty;

        // Act
        var result = await _llamaQueryExtractor.ExtractQueryPrompts(document);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<List<Prompt>>()
            .And.BeEmpty();
    }

    [Fact]
    public async Task ExtractQueryPrompts_ShouldReturnEmptyList_WhenDocumentIsNull()
    {
        // Arrange
        string document = null;

        // Act
        var result = await _llamaQueryExtractor.ExtractQueryPrompts(document);

        // Assert
        result.Should().NotBeNull()
            .And.BeOfType<List<Prompt>>()
            .And.BeEmpty();
    }
}