using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Orion.Lumina.Domain.Tests
{
    public class LlamaQueryExtractorTests
    {
        private readonly LlamaQueryExtractor _llamaQueryExtractor;

        public LlamaQueryExtractorTests()
        {
            _llamaQueryExtractor = new LlamaQueryExtractor();
        }

        [Fact]
        public async Task ExtractQueryPrompts_ShouldReturnListOfPrompts_WhenDocumentIsValid()
        {
            // Arrange
            string document = "Some valid document content";

            // Act
            var result = await _llamaQueryExtractor.ExtractQueryPrompts(document);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Prompt>>(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("What is your name?", result[0].Message);
            Assert.Equal("My name is Llama.", result[0].Response);
            Assert.Equal("How are you?", result[1].Message);
            Assert.Equal("I am fine, thank you.", result[1].Response);
        }

        [Fact]
        public async Task ExtractQueryPrompts_ShouldReturnEmptyList_WhenDocumentIsEmpty()
        {
            // Arrange
            string document = string.Empty;

            // Act
            var result = await _llamaQueryExtractor.ExtractQueryPrompts(document);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Prompt>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task ExtractQueryPrompts_ShouldReturnEmptyList_WhenDocumentIsNull()
        {
            // Arrange
            string document = null;

            // Act
            var result = await _llamaQueryExtractor.ExtractQueryPrompts(document);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Prompt>>(result);
            Assert.Empty(result);
        }
    }
}
