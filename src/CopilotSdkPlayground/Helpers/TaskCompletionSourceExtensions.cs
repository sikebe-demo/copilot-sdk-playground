namespace CopilotSdkPlayground.Helpers;

/// <summary>
/// TaskCompletionSource の拡張メソッド
/// </summary>
public static class TaskCompletionSourceExtensions
{
    /// <summary>
    /// タイムアウト付きで TaskCompletionSource の完了を待機します
    /// </summary>
    /// <param name="tcs">待機対象の TaskCompletionSource</param>
    /// <param name="timeout">タイムアウト時間</param>
    /// <exception cref="TimeoutException">タイムアウト時間内に完了しなかった場合</exception>
    public static async Task WaitWithTimeoutAsync(this TaskCompletionSource tcs, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(tcs);

        using var cts = new CancellationTokenSource(timeout);
        try
        {
            await tcs.Task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Session did not complete within the timeout period.");
        }
    }
}
