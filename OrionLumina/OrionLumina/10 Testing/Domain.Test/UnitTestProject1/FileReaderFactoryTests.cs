﻿using FluentAssertions;
using Orion.Lumina.Application;
using Xunit;

namespace DomainTests;

public class FileReaderFactoryTests
{
    private readonly FileReaderFactory _factory = new();

    [Theory]
    [InlineData("test.odt", "ExpectedText")]
    [InlineData("test.pdf", "ExpectedText")]
    [InlineData("test.xls", "ExpectedText")]
    [InlineData("test.xlsx", "ExpectedText")]
    [InlineData("test.docx", "ExpectedText")]
    [InlineData("test.doc", "ExpectedText")]
    [InlineData("test.txt", "ExpectedText")]
    public void GetFileReader_ShouldResolveCorrectReader(string fileName, string expectedMessage)
    {
        // Act
        var reader = _factory.GetFileReader(Path.GetExtension(fileName));
        var result = reader.Read(fileName);

        // Assert

        result.Should().Contain(expectedMessage);
        
    }

    [Fact]
    public void GetFileReader_UnsupportedExtension_ShouldThrowException()
    {
        // Arrange
        var unsupportedFile = "test.unknown";

        // Act & Assert
        var exception = Assert.Throws<NotSupportedException>(() =>
            _factory.GetFileReader(Path.GetExtension(unsupportedFile)));
        Assert.Equal($"File extension '.unknown' is not supported.", exception.Message);
    }
}