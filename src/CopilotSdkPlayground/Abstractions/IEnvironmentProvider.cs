namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// 環境変数へのアクセスを抽象化するインターフェース
/// </summary>
public interface IEnvironmentProvider
{
    /// <summary>
    /// 環境変数の値を取得します
    /// </summary>
    /// <param name="variable">環境変数名</param>
    /// <returns>環境変数の値、存在しない場合は null</returns>
    string? GetEnvironmentVariable(string variable);
}
