namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// ファイルシステム操作を抽象化するインターフェース
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// 指定されたパスにファイルが存在するかどうかを確認します
    /// </summary>
    /// <param name="path">ファイルパス</param>
    /// <returns>ファイルが存在する場合は true</returns>
    bool FileExists(string path);

    /// <summary>
    /// パスを結合します
    /// </summary>
    /// <param name="paths">結合するパス</param>
    /// <returns>結合されたパス</returns>
    string CombinePath(params string[] paths);

    /// <summary>
    /// パス区切り文字を取得します
    /// </summary>
    char PathSeparator { get; }
}
