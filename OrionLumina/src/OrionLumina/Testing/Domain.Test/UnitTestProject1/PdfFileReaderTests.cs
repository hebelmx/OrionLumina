using FluentAssertions;
using Orion.Lumina.infrastructure;
using Xunit;

namespace DomainTests;

public class PdfFileReaderTests
{
    private readonly PdfFileReader _pdfFileReader = new();

    [Fact]
    public void ReadContent_ShouldThrowArgumentException_WhenFilePathIsNullOrEmpty()
    {
        // Arrange
        string? filePath = null;

        // Act
        Action act = () => _pdfFileReader.ReadContent(filePath!);

        // Assert
        act.Should().Throw<ArgumentException>();

    }

    [Fact]
    public void ReadContent_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = "nonexistent.pdf";

        // Act
        Action act = () => _pdfFileReader.ReadContent(filePath);

        // Assert
        act.Should().Throw<FileNotFoundException>();

    }

    [Fact]
    public void ReadContent_ShouldReturnTextContent_WhenFileExists()
    {
        // Arrange
        var filePath = "test.pdf";
        var expectedText = "ExpectedText";

        // Act
        var result = _pdfFileReader.ReadContent(filePath);

        // Assert
        result.Should().Contain(expectedText);
    }



    [Fact]
    public void ExtractImages_ShouldReturnImages_WhenFileExists()
    {
        // Arrange
        var filePath = "test.pdf";


        // Act
        var result = _pdfFileReader.ExtractImages(filePath);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ExtractStructuredData_ShouldReturnEmptyCollection_WhenNotImplemented()
    {
        // Arrange
        var filePath = "test.pdf";

        // Act
        var result = _pdfFileReader.ExtractStructuredData(filePath);

        // Assert
        result.Should().BeEmpty();
    }
}