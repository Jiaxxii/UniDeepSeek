## `UniDeepSeek` 使用説明

---

### はじめに

1.  [あなたは`UniTask`の基本的な操作を学ぶ必要があります。もし`Task`を使ったことがあり、`UniTask`に切り替える場合、この移行はほとんど違いを感じないでしょう。](https://github.com/Cysharp/UniTask)
2.  `Odin`プラグインの使用を推奨します ***これはUnity Inspectorのパラメータをより直感的にします***
3.  `UniDeepSeek`は`UniTask`非同期ライブラリと`Newtonsoft.Json`解析ライブラリに依存しています。プロジェクトにこれら2つのライブラリが既に追加されていることを確認してください。
4.  `TextMeshPro`を追加する必要があります。
5.  可能な限り高い`Unity`のバージョンを使用し、最低でも`C# 9.0`をサポートするバージョンが必要です。
6.  機能の使用例は`Example`フォルダ内にあります。

---

### 1. クイックスタート

#### 1.1 非ストリーミングリクエスト

ストリーミングインターフェースを使用してチャットリクエストを素早く作成します。これは、マルチターンでの会話を行わずに単一のリクエストのみが必要な場合に便利です。

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

チャットの結果をリアルタイムで取得する必要がある場合は、ストリーミングメソッドを使用する必要があります。以下のコードは、ストリーミングメソッドを使用してチャット結果を取得する方法を示しています。
推論ではないストリーミングリクエストの処理は比較的簡単で、`StreamChatCompletionsEnumerableAsync`メソッドを直接使用し、手動で反復処理することができます。
しかし、ストリーミングリクエストの処理を簡素化するための拡張メソッド`DisplayChatStreamBasicAsync`も準備しています。
ここでは、`TMP_Text`の拡張メソッド`DisplayChatStreamBasicAsync`を使用して、チャット結果を`TextMeshProUGUI`にリアルタイムで表示します。

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

カスタム処理が必要な場合は、手動で`asyncEnumerable`を反復処理して処理できます。

```csharp
await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(
                           onCompletion: completion => { /* ストリーミングが完了すると、すべてのストリームがマージされます */ },
                           cancellation: CancellationToken.None /* リクエストをキャンセルするにはCancellationTokenを渡すことを推奨します */ ))
            {
                // チャット結果を処理する
            }
```

#### 1.3 推論モードのストリーミング処理

推論モード（Reasoning Mode）のストリーミングリクエストの場合、拡張メソッドを準備しています。`TMP_Text`の拡張メソッドを使用して呼び出すことができます。
推論コンテンツの色を設定するには、`colorHex`パラメータを指定できます。

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

### 1.4 推論ストリーミングをイベントストリームに変換する

`推論ストリーミングモード`で`deepSeekChat.StreamChatCompletionsEnumerableAsync`を直接反復処理すると、メッセージ内の`Content`フィールドと`ReasoningContaent`フィールドを区別するのが難しいことに気付くかもしれません。
例えば、推論メッセージを赤で表示したい場合、`await foreach`内で様々な判断を行う必要があるかもしれません。この目的のために、`ChatCompletionEvent`クラスを使用してメッセージストリームをイベントストリームに変換できます。

ここでは、`TMP_Text`の拡張メソッド`DisplayReasoningStreamWithEventsAsync`を使用して、チャット結果を`TextMeshProUGUI`にリアルタイムで表示し、メッセージストリームをイベントストリームに変換します。
`chatCompletionEvent`をグローバル変数として保存し、後のリクエストで再利用できます。

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

DeepSeekChat deepSeekChat = builder.Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hi,my name is player!")
        .Base.SetModel(ChatModel.Reasoner)
    .Build(youApiKey);

var asyncEnumerable = deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion, cancellationToken);

ChatCompletionEvent chatCompletionEvent = await textMeshProUGUI.DisplayReasoningStreamWithEventsAsync(asyncEnumerable, colorHex: ColorToHex(reasoningColor));

```

イベントストリームを完全にカスタマイズする必要がある場合は、`ChatCompletionEvent`クラスを直接使用できます。
以下の例は、`textMeshProUGUI.DisplayReasoningStreamWithEventsAsync`の実装とほとんど同等です。

理論的には、推論ではないストリーミングメッセージもイベントストリームに変換できますが、そうしないことをお勧めします。直接`await foreach`を使用することが最良の方法です。
イベントストリームへの変換は追加のオーバーヘッドを導入し、推論ではないストリーミングメッセージは比較的単純です。

要約すると、`TMP_Text`の拡張メソッドは`推論ではないストリーミングメッセージ`をイベントストリームに変換する方法を提供していますが、その使用はお勧めしません。
拡張メソッドはすべてパラメータとして`UniTaskCancelableAsyncEnumerable<ChatCompletion>`を受け取るため、メソッド名をよく確認してください。
推論と非推論のストリーミングメッセージを誤って混在させると、不必要なオーバーヘッドを引き起こす可能性があります。

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

// コンストラクタを介してカスタムのChatCompletionEventを作成することも直接できます

// 必要に応じて、`IChatCompletionRunning`インターフェースを継承して動作を変更できます
var chatCompletionEvent = new ChatCompletionEvent();

chatCompletionEvent.ReasoningEventSetting
    // 推論コンテンツの受信開始時のイベント
    // 「Set」で始まるメソッドはデリゲートを上書きします。追加する場合は「Append」で始まるメソッドを使用してください
    .SetEnter(completion =>
    {
        var message = completion.GetMessage();
        chatText.text = string.IsNullOrEmpty(colorHex)
            ? message.ReasoningContent
            : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
    })
    // コンテンツ受信時（最初の piece を除く）
    .SetUpdate(completion => chatText.text += completion.GetMessage().ReasoningContent)
    // 受信終了時（completion は最後の piece と同じ、つまり最後の更新）
    .SetExit(_ => chatText.text += "</color>\n\n");


chatCompletionEvent.ContentEventSetting.SetEnter(completion => chatText.text += completion.GetMessage().Content)
    .SetUpdate(completion => chatText.text += completion.GetMessage().Content);

// イベントストリームをトリガーします (asyncEnumerable を await foreach しないでください)
await chatCompletionEvent.DisplayChatStreamAsync(asyncEnumerable);
```

---

### 2. マルチターンチャットには、一部のクラスを以下のように宣言する必要があります

#### 2.1 リクエストコンフィギュレーター

まず、*リクエストコンフィギュレーター*を作成する必要があります。ここで、`MaxToken`、`Temperature`、`TopK`などのモデルパラメータを設定できます。
プロジェクトで`Odin`プラグインを使用している場合は、`[ShowInInspector]`属性を使用してリクエストコンフィギュレーターを表示できます。

```csharp
[Sirenix.OdinInspector.ShowInInspector]
public Xiyu.UniDeepSeek.ChatRequestParameter _setting = new();
```

次に、*レクエスター*が必要です。これはリクエストを送信しレスポンスを取得する責任があります。初期化時には、***リクエストコンフィギュレーター***とあなたの***APIキー***をそれぞれ渡す必要があります。
`DeepSeekChat`クラスを再利用する場合は、それをクラスのフィールドとして宣言してください。

```csharp
private Xiyu.UniDeepSeek.DeepSeekChat _deepSeekChat;
private void Awake()
{
    _deepSeekChat = new Xiyu.UniDeepSeek.DeepSeekChat(_setting, YourApiKey);
}
```

これで、`DeepSeekChat`クラスの`ChatCompletionAsync`を呼び出してチャット結果を取得できるようになります。對了、`ChatRequestParameter`にメッセージを追加することを忘れないでください。

リクエストをキャンセルするために`CancellationToken`を渡すことができます。キャンセル要求があった場合、メソッドは適切なタイミングでキャンセルします。

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

`deepSeekChat.ChatCompletionAsync()`は`UniTask<(ChatState, ChatCompletion)>`を返します。
ここで、`ChatState`はチャットの状態を示す列挙型であり、`ChatCompletion`はチャット結果を含む`ChatCompletion`オブジェクトです。

`chatCompletion.GetMessage().Content`を使用してチャット結果をすばやく取得できます。これは`chatCompletion.Choices[0].SourcesMessage.Content`と同等です。
值得一提的是、`Odin`プラグインをインストールすると、戻り値の`ChatCompletion`はシリアル化可能です。メンバ変数にその結果を割り当て、`Unity Inspector`で表示できます。

---

#### 2.2 ストリーミングリクエスト

チャット結果をリアルタイムで取得する必要がある場合は、ストリーミングメソッドを使用する必要があります
`StreamChatCompletionsEnumerableAsync(Action<ChatCompletion> onCompletion, CancellationToken cancellation)`

`onCompletion`はコールバック関数であり、このリクエスト内のすべてのストリームをマージし、新しい`ChatCompletion`オブジェクトを作成して返します。

***ストリーミングメソッドがキャンセル要求されると`OperationCanceledException`例外をスローしますが、通常の非同期メソッドは例外をスローせず、`ChatState`列挙型を返します。***

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

        // 結果を計算する
        Debug.Log($"コスト: {completion.Usage.CalculatePrice(ChatModel.Chat):F3} 元");
    }
    catch (OperationCanceledException e)
    {
        Debug.LogWarning("チャットリクエストがキャンセルされました:" + e.Message);
    }
}
```