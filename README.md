# Copilot SDK Playground

GitHub Copilot SDK を使用したサンプルアプリケーションです。ストリーミング・非ストリーミングモードでの Copilot との対話をデモンストレーションします。

## 🚀 Features

- **GitHub Copilot SDK** - GitHub Copilot SDK を使用した AI チャット機能
- **ストリーミングモード** - リアルタイムでレスポンスを受信
- **非ストリーミングモード** - 完了したレスポンスをまとめて受信
- **GPT-5 モデル対応** - 最新の AI モデルを使用
- **.NET 10.0** - 最新の .NET バージョン

## 📋 Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 以降
- [GitHub Copilot CLI](https://docs.github.com/en/copilot) がインストールされ、認証済みであること
- [Git](https://git-scm.com/)

## 📁 Project Structure

```
├── src/
│   └── CopilotSdkPlayground/
│       ├── Program.cs                  # エントリーポイント
│       ├── CopilotSdkPlayground.csproj # プロジェクトファイル
│       ├── LoggerSetup.cs              # ロガー設定
│       ├── CopilotClientInfoLogger.cs  # 接続情報ロガー
│       └── Demos/
│           ├── StreamingDemo.cs        # ストリーミングモードのデモ
│           └── NonStreamingDemo.cs     # 非ストリーミングモードのデモ
├── Directory.Build.props               # MSBuild 共通プロパティ
├── global.json                         # .NET SDK バージョン指定
├── copilot-sdk.sln                     # ソリューションファイル
└── README.md                           # このファイル
```

## 🔧 Dependencies

| パッケージ | バージョン | 説明 |
|-----------|-----------|------|
| GitHub.Copilot.SDK | 0.1.16 | GitHub Copilot SDK |
| Microsoft.Extensions.Hosting | 10.0.2 | DI / ホスティング |

## 🎮 Usage

### ストリーミングモード（デフォルト）

リアルタイムでレスポンスを受信して表示します：

```bash
dotnet run --project src/CopilotSdkPlayground
```

### 非ストリーミングモード

完了したレスポンスをまとめて表示します：

```bash
dotnet run --project src/CopilotSdkPlayground --no-streaming
```

## 📝 Demo Details

### StreamingDemo

- `AssistantMessageDeltaEvent` でテキスト断片をリアルタイム表示
- `AssistantReasoningDeltaEvent` で推論中のテキストを表示
- `SessionIdleEvent` でセッション完了を検知

### NonStreamingDemo

- `AssistantMessageEvent` で完了したメッセージを表示
- `AssistantReasoningEvent` で推論結果を表示
- `SessionIdleEvent` でセッション完了を検知

## 📄 License

[LICENSE](LICENSE) ファイルに記載されたライセンス条項に従います。

---

Happy coding! 🎉
