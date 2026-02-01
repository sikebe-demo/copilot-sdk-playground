---
name: fix-copilot-review
description: PR の Copilot Code Review 指摘を gh CLI で取得し、指摘ごとに修正・コミットするワークフロー。「レビュー指摘を修正」「Copilot Review の対応」「PR のコメントを直して」などのリクエスト時に使用。
---

# Fix Copilot Review

PR の Copilot Code Review 指摘を取得し、指摘ごとにコード修正とコミットを行う。

## 前提条件

- gh CLI 認証済み (`gh auth status` で確認)
- 対象 PR のブランチにチェックアウト済み

## ワークフロー

1. PR 番号を特定
2. レビューコメントを取得（スレッド ID 含む）
3. **各指摘の対応方針をユーザーに確認**（修正/保留/無視）
4. 修正対象の指摘について: 修正 → コミット
5. **Push 前にユーザーに確認** → Push 実行
6. 保留したスレッド以外をすべて Resolve

## Step 1: PR 番号の特定

```bash
# 現在のブランチに紐づく PR を取得
gh pr view --json number -q '.number'
```

PR 番号が取得できない場合、ユーザーに PR 番号を確認する。

## Step 2: 未解決のレビューコメント取得

GraphQL API を使用して、未解決 (`isResolved: false`) のスレッドのみを取得する。**スレッド ID も取得**（Step 6 の Resolve で使用）。

```bash
# gh api の --jq オプションを使用（外部 jq コマンド不要、PowerShell/Bash 両対応）
gh api graphql -f query='
query($owner: String!, $repo: String!, $pr: Int!) {
  repository(owner: $owner, name: $repo) {
    pullRequest(number: $pr) {
      reviewThreads(first: 100) {
        nodes {
          id
          isResolved
          isOutdated
          path
          line
          comments(first: 10) {
            nodes {
              author { login }
              body
              createdAt
            }
          }
        }
      }
    }
  }
}' -f owner='{owner}' -f repo='{repo}' -F pr={pr_number} --jq '[.data.repository.pullRequest.reviewThreads.nodes[] | select(.isResolved == false)]'
```

レスポンスの主要フィールド:
- `id`: スレッド ID（Resolve 時に使用）
- `isResolved`: 解決済みかどうか (false = 未解決)
- `isOutdated`: コードが変更されて古くなったか
- `path`: 対象ファイルパス
- `line`: 行番号
- `comments.nodes[].body`: コメント本文 (最初のコメントが指摘内容)
- `comments.nodes[].author.login`: コメント者 (copilot など)

## Step 3: 対応方針の確認

`ask_questions` ツールを使用して、各指摘への対応方針をユーザーに確認する。

各指摘について以下の情報を表示し、対応方針を選択させる:
- ファイルパスと行番号
- 指摘内容の全文（日本語に翻訳）

```
ask_questions ツールの使用例:
- header: 短いラベル（例: "指摘1", "指摘2"）
- question: 指摘内容の全文を日本語に翻訳し、ファイルパス・行番号とともに記載
- allowFreeformInput: true（ユーザー独自の方針を入力可能にする）
- options:
  - label: "修正する", description: 修正内容の概要, recommended: true (推奨の場合)
  - label: "保留する", description: "この指摘は後で対応する"
  - label: "無視する", description: "この指摘は対応不要"
```

**選択肢の扱い:**
- 「修正する」: 指摘内容に従ってコードを修正
- 「保留する」: この指摘はスキップし、Resolve しない（後で対応）
- 「無視する」: この指摘はスキップ（push 後に Resolve される）
- 「Other」またはフリーテキスト入力: ユーザーが入力した方針に従って修正

**注意事項:**
- 最大4つの質問を1回の `ask_questions` 呼び出しでまとめる
- 5件以上の指摘がある場合は複数回に分けて確認
- ユーザーが「無視する」または「保留する」を選択した指摘はスキップする
- 「保留する」を選択した指摘は内部で記録し、Step 6 の Resolve 対象から除外する

## Step 4: 指摘ごとに修正・コミット

**ユーザーが「修正する」を選択した指摘のみ**、以下を繰り返す:

1. 指摘内容を確認し、該当ファイル・行を特定
2. コードを修正
3. コミット作成:

```bash
# 修正したファイルを個別にステージング（git add . は使用禁止）
git add {修正ファイルのパス}
git commit -m "fix: {summary of the issue in English}"
```

**重要:** `git add .` で全ファイルを一度にステージングしないこと。必ず修正したファイルのみを個別に `git add` する。

コミットメッセージ規則:
- プレフィックス: `fix:` (バグ修正)、`refactor:` (リファクタリング)、`style:` (フォーマット)
- **コミットメッセージは英語で記述**
- 要約は指摘内容から自動生成 (50文字以内)
- 例: `fix: add null check`、`refactor: remove unused variable`

## Step 5: Push 確認

コミット完了後、`ask_questions` ツールを使用して push の確認を行う。

```
ask_questions ツールの使用例:
- header: "Push確認"
- question: "コミットをリモートにプッシュしますか？\n\nプッシュ先: origin/{現在のブランチ名}"
- options:
  - label: "プッシュする", description: "origin/{ブランチ名} にプッシュ", recommended: true
  - label: "プッシュしない", description: "プッシュせずに終了"
```

ユーザーが「プッシュする」を選択した場合:

```bash
git push origin {現在のブランチ名}
```

## Step 6: レビュースレッドの Resolve

git push 完了後、**保留したスレッド以外**のすべての対応済みスレッドを Resolve する。

対象:
- 「修正する」を選択して修正完了したスレッド
- 「無視する」を選択したスレッド

除外:
- 「保留する」を選択したスレッド（未解決のまま残す）

```bash
# スレッドを Resolve する (thread_id は Step 2 で取得した reviewThread の id)
gh api graphql -f query='
mutation($threadId: ID!) {
  resolveReviewThread(input: {threadId: $threadId}) {
    thread {
      isResolved
    }
  }
}' -f threadId='{thread_id}'
```

**注意:** Resolve するには、Step 2 のクエリでスレッド ID も取得する必要がある。以下のようにクエリを拡張:

```bash
gh api graphql -f query='
query($owner: String!, $repo: String!, $pr: Int!) {
  repository(owner: $owner, name: $repo) {
    pullRequest(number: $pr) {
      reviewThreads(first: 100) {
        nodes {
          id
          isResolved
          isOutdated
          path
          line
          comments(first: 10) {
            nodes {
              author { login }
              body
              createdAt
            }
          }
        }
      }
    }
  }
}' -f owner='{owner}' -f repo='{repo}' -F pr={pr_number} --jq '[.data.repository.pullRequest.reviewThreads.nodes[] | select(.isResolved == false)]'
```

## 注意事項

- **未解決スレッドのみ対処**: `isResolved == false` のスレッドのみを修正対象とする
- **isOutdated の扱い**: `isOutdated == true` はコード変更で指摘箇所がずれた可能性あり。内容を確認して対処要否を判断
- 指摘が相互に依存する場合、依存関係を考慮して修正順序を決定
- 同一ファイルへの複数指摘は、行番号の大きい方から修正するとオフセットずれを回避できる
- 修正後、元の指摘が解決されたか確認
