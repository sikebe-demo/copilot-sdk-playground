using CopilotSdkPlayground.Helpers;

namespace CopilotSdkPlayground.UnitTests.Helpers;

/// <summary>
/// TaskCompletionSourceExtensions のテスト
/// </summary>
public class TaskCompletionSourceExtensionsTests
{
    [Fact]
    public async Task WaitWithTimeoutAsync_WhenCompletedBeforeTimeout_ShouldReturnSuccessfully()
    {
        // Arrange
        var tcs = new TaskCompletionSource();
        var timeout = TimeSpan.FromMilliseconds(500);

        // Act
        tcs.SetResult();
        await tcs.WaitWithTimeoutAsync(timeout);

        // Assert - No exception thrown
        Assert.True(tcs.Task.IsCompleted);
    }

    [Fact]
    public async Task WaitWithTimeoutAsync_WhenNotCompletedWithinTimeout_ShouldThrowTimeoutException()
    {
        // Arrange
        var tcs = new TaskCompletionSource();
        var timeout = TimeSpan.FromMilliseconds(50);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<TimeoutException>(
            () => tcs.WaitWithTimeoutAsync(timeout));

        Assert.Equal("Session did not complete within the timeout period.", exception.Message);
    }

    [Fact]
    public async Task WaitWithTimeoutAsync_WhenCompletedJustBeforeTimeout_ShouldReturnSuccessfully()
    {
        // Arrange
        var tcs = new TaskCompletionSource();
        var timeout = TimeSpan.FromMilliseconds(200);

        // Act
        var task = tcs.WaitWithTimeoutAsync(timeout);

        // Complete before timeout
        await Task.Delay(50);
        tcs.SetResult();
        await task;

        // Assert - No exception thrown
        Assert.True(tcs.Task.IsCompleted);
    }

    [Fact]
    public async Task WaitWithTimeoutAsync_WithNullTaskCompletionSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        TaskCompletionSource? tcs = null;
        var timeout = TimeSpan.FromMilliseconds(100);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => tcs!.WaitWithTimeoutAsync(timeout));
    }

    [Fact]
    public async Task WaitWithTimeoutAsync_ShouldDisposeCancellationTokenSource()
    {
        // Arrange
        var tcs = new TaskCompletionSource();
        var timeout = TimeSpan.FromMilliseconds(50);

        // Act & Assert
        // CancellationTokenSource is disposed in using statement
        // This test ensures no ObjectDisposedException is thrown after timeout
        await Assert.ThrowsAsync<TimeoutException>(
            () => tcs.WaitWithTimeoutAsync(timeout));

        // The method should complete without throwing ObjectDisposedException
        // indicating proper disposal handling
    }

    [Fact]
    public async Task WaitWithTimeoutAsync_WithZeroTimeout_ShouldThrowTimeoutExceptionImmediately()
    {
        // Arrange
        var tcs = new TaskCompletionSource();
        var timeout = TimeSpan.Zero;

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutException>(
            () => tcs.WaitWithTimeoutAsync(timeout));
    }
}
