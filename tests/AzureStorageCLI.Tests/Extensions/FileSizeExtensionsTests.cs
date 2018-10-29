using AzureStorageCLI.Extensions;
using FluentAssertions;
using Xunit;

namespace AzureStorageCLI.Tests.Extensions
{
    public class FileSizeExtensionsTests : TestBase
    {
        [Theory]
        [InlineData(1, "1 B")]
        [InlineData(1024, "1 KB")]
        [InlineData(1024 * 1024, "1 MB")]
        [InlineData(1024 * 1024 * 1024, "1 GB")]
        [InlineData(1099511627776, "1 TB")]
        public void ToSizeString_Should_Return_Output(long inputFileSize, string output)
        {
            //Arrange

            //Act
            var result = inputFileSize.ToSizeString();

            //Assert
            result.Should().Be(output);
        }
    }
}