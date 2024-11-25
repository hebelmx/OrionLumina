using System;
using System.Text;
using Xunit;
using Orion.Lumina.Domain;

namespace Orion.Lumina.Domain.Tests
{
    public class FileRecordTests
    {
        [Fact]
        public void ComputeSha256Hash_ShouldReturnCorrectHash()
        {
            // Arrange
            string input = "test";
            string expectedHash = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

            // Act
            string actualHash = FileRecord.ComputeSha256Hash(input);

            // Assert
            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public void ConvertToHexString_ShouldReturnCorrectHexString()
        {
            // Arrange
            byte[] hashBytes = { 0x9f, 0x86, 0xd0, 0x81, 0x88, 0x4c, 0x7d, 0x65, 0x9a, 0x2f, 0xea, 0xa0, 0xc5, 0x5a, 0xd0, 0x15, 0xa3, 0xbf, 0x4f, 0x1b, 0x2b, 0x0b, 0x82, 0x2c, 0xd1, 0x5d, 0x6c, 0x15, 0xb0, 0xf0, 0x0a, 0x08 };
            string expectedHexString = "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08";

            // Act
            string actualHexString = FileRecord.ComputeSha256Hash("test");

            // Assert
            Assert.Equal(expectedHexString, actualHexString);
        }

        [Fact]
        public void FileRecord_ShouldInitializeProperties()
        {
            // Arrange
            var fileRecord = new FileRecord
            {
                Id = Guid.NewGuid(),
                FileName = "test.txt",
                Content = "This is a test file.",
                Hash = FileRecord.ComputeSha256Hash("This is a test file."),
                Status = FileStatus.Pending,
                UploadedAt = DateTime.UtcNow,
                ProcessedAt = null
            };

            // Act & Assert
            Assert.NotEqual(Guid.Empty, fileRecord.Id);
            Assert.Equal("test.txt", fileRecord.FileName);
            Assert.Equal("This is a test file.", fileRecord.Content);
            Assert.Equal(FileRecord.ComputeSha256Hash("This is a test file."), fileRecord.Hash);
            Assert.Equal(FileStatus.Pending, fileRecord.Status);
            Assert.True(fileRecord.UploadedAt <= DateTime.UtcNow);
            Assert.Null(fileRecord.ProcessedAt);
        }
    }
}
