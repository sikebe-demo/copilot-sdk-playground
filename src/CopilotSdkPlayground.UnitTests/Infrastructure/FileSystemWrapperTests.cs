using CopilotSdkPlayground.Infrastructure;

namespace CopilotSdkPlayground.UnitTests.Infrastructure;

public class FileSystemWrapperTests
{
    private readonly FileSystemWrapper _sut;

    public FileSystemWrapperTests()
    {
        _sut = new FileSystemWrapper();
    }

    [Fact]
    public void FileExists_WithExistingFile_ShouldReturnTrue()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        try
        {
            // Act
            var result = _sut.FileExists(tempFile);

            // Assert
            Assert.True(result);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void FileExists_WithNonExistingFile_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act
        var result = _sut.FileExists(nonExistingPath);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CombinePath_WithMultiplePaths_ShouldCombineCorrectly()
    {
        // Arrange
        var path1 = "folder1";
        var path2 = "folder2";
        var path3 = "file.txt";

        // Act
        var result = _sut.CombinePath(path1, path2, path3);

        // Assert
        Assert.Equal(Path.Combine(path1, path2, path3), result);
    }

    [Fact]
    public void PathSeparator_ShouldReturnCorrectSeparator()
    {
        // Act
        var result = _sut.PathSeparator;

        // Assert
        Assert.Equal(Path.PathSeparator, result);
    }
}
