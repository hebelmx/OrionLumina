using FluentAssertions;
using Orion.Lumina.Domain;
using System.Text.Json;
using Xunit;

namespace DomainTests;

public class JsonHelperTests
{
    [Fact]
    public void ConvertFromJson_ValidJson_ReturnsListOfPrompts()
    {
        // Arrange
        string json = "[{\"Message\": \"Hello\", \"Response\": \"Hi\"}, {\"Message\": \"How are you?\", \"Response\": \"I'm fine, thanks!\"}]";

        // Act
        var result = json.ConvertFromJson<Prompt>();

        // Assert
        result.Should().HaveCount(2);
        result[0].Message.Should().Be("Hello");
        result[0].Response.Should().Be("Hi");
        result[1].Message.Should().Be("How are you?");
        result[1].Response.Should().Be("I'm fine, thanks!");
    }

    [Fact]
    public void ConvertFromJson_EmptyJson_ThrowsArgumentNullException()
    {
        // Arrange
        string json = "";

        // Act
        Action act = () => json.ConvertFromJson<Prompt>();

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("JSON input cannot be null or empty*");
    }

    [Fact]
    public void ConvertFromJson_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = "{InvalidJson}";

        // Act
        Action act = () => json.ConvertFromJson<Prompt>();

        // Assert
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ConvertFromJson_NullJson_ThrowsArgumentNullException()
    {
        // Arrange
        string? json = null;

        // Act
        Action act = () => json.ConvertFromJson<Prompt>();

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("JSON input cannot be null or empty*");
    }

    [Fact]
    public void ConvertFromJson_EmptyArray_ReturnsEmptyList()
    {
        // Arrange
        string json = "[]";

        // Act
        var result = json.ConvertFromJson<Prompt>();

        // Assert
        result.Should().BeEmpty();
    }



}


