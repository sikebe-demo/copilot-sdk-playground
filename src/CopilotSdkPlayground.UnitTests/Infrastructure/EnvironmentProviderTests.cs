using CopilotSdkPlayground.Infrastructure;

namespace CopilotSdkPlayground.UnitTests.Infrastructure;

public class EnvironmentProviderTests
{
    private readonly EnvironmentProvider _sut;

    public EnvironmentProviderTests()
    {
        _sut = new EnvironmentProvider();
    }

    [Fact]
    public void GetEnvironmentVariable_WithExistingVariable_ShouldReturnValue()
    {
        // Arrange
        const string variableName = "PATH";

        // Act
        var result = _sut.GetEnvironmentVariable(variableName);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetEnvironmentVariable_WithNonExistingVariable_ShouldReturnNull()
    {
        // Arrange
        const string variableName = "NON_EXISTING_VARIABLE_12345";

        // Act
        var result = _sut.GetEnvironmentVariable(variableName);

        // Assert
        Assert.Null(result);
    }
}
