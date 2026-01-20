namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// コンソール出力を抽象化するインターフェース
/// </summary>
public interface IConsoleWriter
{
    /// <summary>
    /// テキストを出力します（改行なし）
    /// </summary>
    /// <param name="text">出力するテキスト</param>
    void Write(string? text);

    /// <summary>
    /// テキストを出力します（改行あり）
    /// </summary>
    /// <param name="text">出力するテキスト</param>
    void WriteLine(string? text = null);
}
