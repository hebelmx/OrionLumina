using FluentAssertions;
using Orion.Lumina.Application;
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
        Action act = () => _pdfFileReader.Read(filePath!);

        // Assert
        act.Should().Throw<ArgumentException>();

    }

    [Fact]
    public void ReadContent_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var filePath = "nonexistent.pdf";

        // Act
        Action act = () => _pdfFileReader.Read(filePath);

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
        var result = _pdfFileReader.Read(filePath);

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





// Unit Test Class
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
        Assert.Equal(expectedMessage, result);
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


