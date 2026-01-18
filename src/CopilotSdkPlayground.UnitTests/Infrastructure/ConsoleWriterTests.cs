using CopilotSdkPlayground.Infrastructure;

namespace CopilotSdkPlayground.UnitTests.Infrastructure;

public class ConsoleWriterTests
{
    [Fact]
    public void Constructor_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var writer = new ConsoleWriter();
        Assert.NotNull(writer);
    }

    [Fact]
    public void Write_ShouldWriteToConsole()
    {
        // Arrange
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            writer.Write("test");

            // Assert
            Assert.Equal("test", stringWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void WriteLine_ShouldWriteLineToConsole()
    {
        // Arrange
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            writer.WriteLine("test");

            // Assert
            Assert.Equal("test" + Environment.NewLine, stringWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void WriteLine_WithNull_ShouldWriteEmptyLine()
    {
        // Arrange
        var writer = new ConsoleWriter();
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            writer.WriteLine(null);

            // Assert
            Assert.Equal(Environment.NewLine, stringWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
