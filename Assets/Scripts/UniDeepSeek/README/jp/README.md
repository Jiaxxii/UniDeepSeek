# `UniDeepSeek` 使用説明

[![Version](https://img.shields.io/badge/version-v1.0.0-blue.svg)](https://github.com/Jiaxxii/UniDeepSeek)

---

### はじめに

1. [UniTaskの基本的な操作を学ぶ必要があります。Taskを使用したことがある場合、UniTaskへの切り替えはほとんど違和感なく行えるかもしれません。](https://github.com/Cysharp/UniTask)
2. **Odin** プラグインの使用を推奨します。***これにより、UnityのInspectorでのパラメータ表示がより直感的になります。***
3. `UniDeepSeek` は非同期ライブラリの `UniTask` と解析ライブラリの `Newtonsoft.Json` に依存しています。プロジェクトにこれら2つのライブラリが追加されていることを確認してください。
4. `TextMeshPro` を追加する必要があります。
5. 可能な限り高いバージョンの `Unity` を使用し、最低でも `C# 9.0` をサポートしている必要があります。
6. 機能の使用例は `Example` フォルダ内にあります。

---

### 1. クイックスタート

#### 1.1 非ストリーミングリクエスト

ストリーミングインターフェースを使用して、チャットリクエストを素早く作成します。これは、複数回の会話を行わずに一度だけリクエストを実行する場合に便利です。

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

builder.SetModel(ChatModel.Chat)
    .Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hello Xiyu.");

DeepSeekChat deepSeekChat = builder.Build(yourApiKey);

var (state, chatCompletion) = await deepSeekChat.ChatCompletionAsync(destroyCancellationToken);
if (state == ChatState.Success)
{
    Debug.Log(chatCompletion.GetMessage().Content);
}
```

#### 1.2 ストリーミングリクエスト

チャットの結果をリアルタイムで取得する必要がある場合は、ストリーミングメソッドを使用する必要があります。以下のコードは、ストリーミングメソッドを使用してチャットの結果を取得する方法を示しています。

非推論のストリーミングリクエストの処理は比較的簡単です。`StreamChatCompletionsEnumerableAsync` メソッドを直接使用し、手動で反復処理することができます。

ただし、ストリーミングリクエストの処理を簡素化するための拡張メソッド `DisplayChatStreamBasicAsync` も準備しています。
ここでは、`TMP_Text` の拡張メソッド `DisplayChatStreamBasicAsync` を使用して、チャットの結果を `TextMeshProUGUI` にリアルタイムで表示します。

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

builder.SetModel(ChatModel.Chat)
    .Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("Can you call me 'zako~ zako~'?");

DeepSeekChat deepSeekChat = builder.Build(ApiKey);

var asyncEnumerable = deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion);

textMeshProUGUI.text = string.Empty;
await textMeshProUGUI.DisplayChatStreamBasicAsync(asyncEnumerable);
```

カスタム処理が必要な場合は、手動で `asyncEnumerable` を反復処理して処理することができます。

```csharp
await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(
                           onCompletion: completion => { /* ストリーミングが完了すると、すべてのストリームが統合されます */ },
                           cancellation: CancellationToken.None /* リクエストをキャンセルする場合は CancellationToken を渡すことを推奨します */ ))
            {
                // チャット結果の処理
            }
```

#### 1.3 推論ストリーミング処理

推論ストリーミングリクエストの場合、拡張メソッドを準備しています。引き続き `TMP_Text` の拡張メソッドを使用して呼び出すことができます。
`colorHex` パラメータを指定して、推論コンテンツの色を設定できます。

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

DeepSeekChat deepSeekChat = builder.Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hi,my name is player!")
        .Base.SetModel(ChatModel.Reasoner)
    .Build(youApiKey);

var asyncEnumerable = deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion, cancellationToken);

await textMeshProUGUI.DisplayReasoningChatStreamBasicAsync(asyncEnumerable, colorHex: ColorToHex(reasoningColor));
```

### 1.4 推論ストリーミングをイベントストリームに変換

`推論ストリーミングモード` で `deepSeekChat.StreamChatCompletionsEnumerableAsync` を直接反復処理すると、メッセージ内の `Content` フィールドと `ReasoningContent` フィールドを区別するのが難しい場合があります。
例えば、推論メッセージを赤で表示したい場合、`await foreach` ループ内でさまざまな判断を行う必要があるかもしれません。この問題に対処するために、`StreamCompletionEventFacade` クラスを使用してメッセージストリームをイベントストリームに変換できます。

ここでは、`TMP_Text` の拡張メソッド `DisplayReasoningStreamWithEvents` を使用して、チャットの結果を `TextMeshProUGUI` にリアルタイムで表示し、メッセージストリームをイベントストリームに変換します。

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

DeepSeekChat deepSeekChat = builder.Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hi,my name is player!")
        .Base.SetModel(ChatModel.Reasoner)
    .Build(youApiKey);

var asyncEnumerable = deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion, cancellationToken);

// メッセージストリームをイベントストリームに変換
StreamCompletionEventFacade eventFacade = textMeshProUGUI.DisplayReasoningStreamWithEvents(colorHex: ColorToHex(reasoningColor));

// イベントストリームをトリガー（asyncEnumerable に対して await foreach を使用しないでください）
await eventFacade.Builder().DisplayChatStreamAsync(asyncEnumerable);
```

イベントストリームを完全にカスタマイズする必要がある場合は、直接 `StreamCompletionEventFacade` クラスを使用できます。
以下の例は、`textMeshProUGUI.DisplayReasoningStreamWithEvents` の実装とほとんど同等です。

理論的には、非推論ストリーミングメッセージもイベントストリームに変換できますが、これは推奨しません。直接 `await foreach` を使用することが最良の方法です。
イベントストリームへの変換は追加のオーバーヘッドを導入し、非推論ストリーミングメッセージは比較的単純です。

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

DeepSeekChat deepSeekChat = builder.Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hi,my name is player!")
    .Base.SetModel(ChatModel.Reasoner)
    .Build(youApiKey);


var asyncEnumerable = await deepSeekChat.StreamChatCompletionsEnumerableAsync(destroyCancellationToken);

// Inspectorで色を設定できます
// この色は推論コンテンツの色を表します
var colorHex = ColorUtility.ToHtmlStringRGB(new Color(1, 0, 1));

// コンストラクタを介してカスタムの StreamCompletionEventFacade を作成することもできます

// 必要に応じて、`IStreamCompletionConsumer` インターフェイスの実装をコンストラクタに渡して動作を変更できます
var eventFacade = StreamCompletionEventFacade.CreateByDefaultConsumer();


eventFacade.ReasoningEvent
    // 推論コンテンツの受信開始時のイベント
    // "Set" で始まるメソッドはデリゲートを上書きします。デリゲートに追加する場合は "Append" で始まるメソッドを使用してください
    .SetEnter(completion =>
    {
        var message = completion.GetMessage();
        chatText.text = string.IsNullOrEmpty(colorHex)
            ? message.ReasoningContent
            : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
    })
    // コンテンツ受信時（最初の部分を除く）
    .SetUpdate(completion => chatText.text += completion.GetMessage().ReasoningContent)
    // 受信終了時（completion は最後の部分と同じ、つまり最後の更新）
    .SetExit(_ => chatText.text += "</color>\n\n");


eventFacade.ReasoningEvent.SetEnter(completion => chatText.text += completion.GetMessage().Content)
    .SetUpdate(completion => chatText.text += completion.GetMessage().Content);

// イベントストリームをトリガー（asyncEnumerable に対して await foreach を使用しないでください）
await eventFacade.Builder().DisplayChatStreamAsync(asyncEnumerable);
```

---

### 2. 複数回のチャットには、一部のクラスを以下のように宣言する必要があります

#### 2.1 リクエストコンフィギュレーター

まず、*リクエストコンフィギュレーター* を作成する必要があります。ここでモデルのパラメータ（`MaxToken`、`Temperature`、`TopK` など）を設定できます。
プロジェクトで `Odin` プラグインを使用している場合は、`[ShowInInspector]` 属性を使用してリクエストコンフィギュレーターを表示できます。

```csharp
[Sirenix.OdinInspector.ShowInInspector]
public Xiyu.UniDeepSeek.ChatRequestParameter _setting = new();
```

次に、*リクエスター* が必要です。これはリクエストを送信しレスポンスを取得する責任があります。初期化時には、***リクエストコンフィギュレーター*** とあなたの ***APIキー*** をそれぞれ渡す必要があります。
`DeepSeekChat` クラスを再利用する場合は、クラスのフィールドとして宣言してください。

```csharp
private Xiyu.UniDeepSeek.DeepSeekChat _deepSeekChat;
private void Awake()
{
    _deepSeekChat = new Xiyu.UniDeepSeek.DeepSeekChat(_setting, YourApiKey);
}
```

これで、`DeepSeekChat` クラスの `ChatCompletionAsync` を呼び出してチャット結果を取得できます。对了，忘れずに `ChatRequestParameter` にメッセージを追加してください。

リクエストをキャンセルする場合は、`CancellationToken` を渡すことができます。キャンセル要求があった場合、メソッドは適切なタイミングでキャンセルします。

```csharp
private async UniTaskVoid ChatCompletionAsync()
{
    var (state, chatCompletion) = await deepSeekChat.ChatCompletionAsync(cancellation: destroyCancellationToken);
    if (state == ChatState.Success)
    {
        Debug.Log(chatCompletion.GetMessage().Content);
    }
}
```

`deepSeekChat.ChatCompletionAsync()` は `UniTask<(ChatState, ChatCompletion)>` を返します。
`ChatState` はチャットの状態を表す列挙型で、`ChatCompletion` はチャット結果を含む `ChatCompletion` オブジェクトです。

`chatCompletion.GetMessage().Content` を使用してチャット結果をすばやく取得できます。これは `chatCompletion.Choices[0].SourcesMessage.Content` と同等です。
值得一提的是，`Odin` プラグインをインストールすると、戻り値の `ChatCompletion` はシリアル化可能です。メンバ変数に代入し、`Unity Inspector` で確認できます。

---

#### 2.2 ストリーミングリクエスト

チャットの結果をリアルタイムで取得する必要がある場合は、ストリーミングメソッドを使用する必要があります:
`StreamChatCompletionsEnumerableAsync(Action<ChatCompletion> onCompletion, CancellationToken cancellation)`

`onCompletion` はコールバック関数で、このリクエストのすべてのストリームを統合し、新しい `ChatCompletion` オブジェクトを作成して返します。

***ストリーミングメソッドがキャンセル要求されると `OperationCanceledException` 例外をスローしますが、通常の非同期メソッドは例外をスローせず、`ChatState` 列挙型を返します。***

```csharp
public async void Start()
{
    try
    {
        var setting = new ChatRequestParameter();
        var deepSeekChat = new DeepSeekChat(setting, "you_api_key");

        ChatCompletion completion = null;
        await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(
                           onCompletion: c => completion = c,
                           cancellation: destroyCancellationToken))
        {
            Debug.Log(chatCompletion.GetMessage().Content);
        }

        // 結果の計算
        Debug.Log($"コスト: {completion.Usage.CalculatePrice(ChatModel.Chat):F3} 元");
    }
    catch (OperationCanceledException e)
    {
        Debug.LogWarning("チャットリクエストがキャンセルされました:" + e.Message);
    }
}
```