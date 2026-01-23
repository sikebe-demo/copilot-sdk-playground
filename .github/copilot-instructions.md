# Copilot Instructions

## Project Overview

GitHub Copilot SDK を使用したサンプルアプリケーション。ストリーミング/非ストリーミングモードでの AI チャット機能をデモする .NET 10.0 プロジェクト。

## Architecture

```
Program.cs (DI Setup) → App.cs (Entry Point) → StreamingDemo / NonStreamingDemo
                                ↓
                    CopilotClientWrapper → GitHub.Copilot.SDK
```

- **DI パターン**: `Microsoft.Extensions.Hosting` ベースの DI コンテナを使用
- **抽象化レイヤー**: `Abstractions/` 配下のインターフェースで外部依存を分離（テスト容易性のため）
- **Infrastructure**: `CopilotClientFactory`, `ConsoleWriter` など本番実装を格納

## Key Patterns

### インターフェース分離
SDK の `CopilotClient` と `CopilotSession` は直接モック不可。`ICopilotClientWrapper` / `ICopilotSession` でラップしてテスト可能に：
```csharp
// Infrastructure/CopilotClientWrapper.cs がSDKをラップ
public interface ICopilotClientWrapper {
    Task<ICopilotSession> CreateSessionAsync(SessionConfig config);
}
```

### イベントハンドリング
ストリーミングでは `session.On(handler)` でイベントを購読し、`SessionIdleEvent` で完了を検知：
```csharp
session.On(evt => {
    switch (evt) {
        case AssistantMessageDeltaEvent delta: // ストリーミング断片
        case AssistantMessageEvent msg:         // 最終メッセージ
        case SessionIdleEvent:                  // 完了
    }
});
```

## Development Commands

```bash
# ビルド
dotnet build

# ストリーミングモード（デフォルト）
dotnet run --project src/CopilotSdkPlayground

# 非ストリーミングモード
dotnet run --project src/CopilotSdkPlayground -- --no-streaming

# ユニットテスト
dotnet test src/CopilotSdkPlayground.UnitTests

# E2Eテスト（認証必須）
$env:GH_TOKEN = $(gh auth token); dotnet test src/CopilotSdkPlayground.E2ETests
```

## Testing Strategy

### Unit Tests (`CopilotSdkPlayground.UnitTests`)
- **Moq** でインターフェースをモック
- `TestHelpers/EventFactory.cs` で SDK イベントオブジェクトを生成
- コンストラクタの null チェック、引数パースなどをテスト

### E2E Tests (`CopilotSdkPlayground.E2ETests`)
- 実際にビルド済み exe をプロセスとして起動
- `GH_TOKEN` / `GITHUB_TOKEN` 環境変数が必須（なければスキップ）
- `ProcessFixture` がプロセス実行とタイムアウト管理を担当
- 同一シナリオのアサーションは1テストにまとめる（プロセス起動コスト削減）

## Conventions

- **言語**: C# 最新プレビュー機能使用 (`LangVersion=preview`)
- **Null安全**: `Nullable=enable`、コンストラクタで `ArgumentNullException` スロー
- **命名**: サービスクラスは `*Service` サフィックス（例: `StreamingDemoService`）
- **XML ドキュメント**: public メンバーには `<summary>` 必須
- **コードスタイル**: `#region` / `#endregion` は使用しない
