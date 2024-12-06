using FluentAssertions;
using Orion.Lumina.Application;
using Xunit;

namespace DomainTests;

public class TextSanitizerTests
{
    [Theory]
    [InlineData("Visit https://example.com", "Visit [URL]", SanitizationOptions.RemoveUrls, "Replaces URLs with [URL]")]
    [InlineData("Email: support@example.com", "Email: [EMAIL]", SanitizationOptions.RemoveEmails, "Replaces emails with [EMAIL]")]
    [InlineData("<b>Bold Text</b>", "Bold Text", SanitizationOptions.RemoveHtmlTags, "Removes HTML tags.")]
    [InlineData("Hello!! Extra   spaces...", "Hello!! Extra spaces...", SanitizationOptions.RemoveExtraSpaces, "Collapses extra spaces.")]
    [InlineData("1234$%^&Text", "1234%Text", SanitizationOptions.RemoveSpecialCharacters, "Removes special characters.")]
    [InlineData("", "", SanitizationOptions.RemoveExtraSpaces, "Handles empty strings without error.")]
    [InlineData("   ", "", SanitizationOptions.RemoveExtraSpaces, "Collapses spaces in whitespace-only strings.")]
    public void SanitizeText_ShouldApplySingleOptionCorrectly(string input, string expected, SanitizationOptions options, string intent)
    {
        // Act
        string result = TextSanitizer.SanitizeText(input, options);

        // Assert
        result.Should().Be(expected, because: intent);
    }

    [Theory]
    [InlineData("Visit https://example.com and email support@example.com", "Visit [URL] and email [EMAIL]", SanitizationOptions.RemoveEmails | SanitizationOptions.RemoveUrls, "Sanitizes emails and URLs.")]
    [InlineData("<b>Bold</b> and https://example.com", "Bold and [URL]", SanitizationOptions.RemoveHtmlTags | SanitizationOptions.RemoveUrls, "Sanitizes HTML tags and URLs.")]
    [InlineData("Special!! Text??", "Special Text", SanitizationOptions.RemoveSpecialCharacters, "Sanitizes special characters.")]
    public void SanitizeText_ShouldApplyMultipleOptionsCorrectly(string input, string expected, SanitizationOptions options, string intent)
    {
        // Act
        string result = TextSanitizer.SanitizeText(input, options);

        // Assert
        result.Should().Be(expected, because: intent);
    }

    [Theory]
    [InlineData("Visit https://example.com", "Visit https://example.com", SanitizationOptions.None, "Does not sanitize URLs.")]
    [InlineData("Email: support@example.com", "Email: support@example.com", SanitizationOptions.None, "Does not sanitize emails.")]
    [InlineData("<b>Bold Text</b>", "<b>Bold Text</b>", SanitizationOptions.None, "Does not sanitize HTML tags.")]
    public void SanitizeText_ShouldRespectNoneOption(string input, string expected, SanitizationOptions options, string intent)
    {
        // Act
        string result = TextSanitizer.SanitizeText(input, options);

        // Assert
        result.Should().Be(expected, because: intent);
    }

    [Fact]
    public void SanitizeText_ShouldDefaultToSanitizeAll()
    {
        // Arrange
        string input = "<b>Email me</b> at user@example.com or visit https://example.com!";
        string expected = "Email me at user@example.com or visit https://example.com!";

        // Act
        string result = TextSanitizer.SanitizeText(input);

        // Assert
        result.Should().Be(expected, because: "default behavior sanitizes all text.");
    }
}

