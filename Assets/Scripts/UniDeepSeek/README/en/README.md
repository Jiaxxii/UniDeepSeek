## `UniDeepSeek` Usage Instructions

---

### Preface

1. [You must learn some basic operations in `UniTask`. If you have used `Task` and are switching to `UniTask`, the transition should feel almost seamless.](https://github.com/Cysharp/UniTask)
2. It is recommended that you use the `Odin` plugin, ***which will make parameters in the Unity Inspector more intuitive***.
3. `UniDeepSeek` depends on the `UniTask` asynchronous library and the `Newtonsoft.Json` parsing library. Please ensure these two libraries are already added to your project.
4. You need to add `TextMeshPro`.
5. Use the highest possible `Unity` version, with a minimum requirement of supporting `C# 9.0`.
6. Functional usage examples are available in the `Example` folder.

---

### 1. Quick Start

#### 1.1 Non-Streaming Request

I will quickly create a chat request using the streaming interface, which is useful when you only need to make a single request without engaging in multi-turn conversations.

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

#### 1.2 Streaming Request

If you need to get chat results in real time, you should use the streaming method. The following code demonstrates how to use the streaming method to obtain chat results.  
For non-reasoning streaming requests, processing is relatively simple—you can directly use the `StreamChatCompletionsEnumerableAsync` method and manually iterate through it.  
However, I have also provided an extension method `DisplayChatStreamBasicAsync` to simplify the handling of streaming requests.  
Here, I use the extension method `DisplayChatStreamBasicAsync` for `TMP_Text` to display the chat results in real time on a `TextMeshProUGUI` component.

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

If you need custom processing, you can manually iterate through `asyncEnumerable` and handle it.

```csharp
await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(
                           onCompletion: completion => { /* When streaming is complete, all streams are merged */ },
                           cancellation: CancellationToken.None /* It is recommended to pass a CancellationToken to cancel the request */ ))
            {
                // Process chat results
            }
```

#### 1.3 Reasoning Mode Streaming Handling

For reasoning mode streaming requests, I have prepared extension methods. You can still call them using the `TMP_Text` extension method.  
You can specify the `colorHex` parameter to set the color for reasoning content.

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

### 1.4 Convert Reasoning Streaming to Event Stream

If you directly iterate through `deepSeekChat.StreamChatCompletionsEnumerableAsync` in `reasoning streaming mode`, you may find it difficult to distinguish between the `Content` and `ReasoningContent` fields in the message.  
For example, if you want reasoning messages to be displayed in red, you might need to make various judgments within `await foreach`. For this purpose, you can use the `ChatCompletionEvent` class to convert the message stream into an event stream.

Here, I will use the `TMP_Text` extension method `DisplayReasoningStreamWithEventsAsync` to display the chat results in real time on a `TextMeshProUGUI` component and convert the message stream into an event stream.  
You can save `chatCompletionEvent` as a global variable and reuse it in subsequent requests.

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

If you need to fully customize the event stream, you can directly use the `ChatCompletionEvent` class.  
The following example is almost equivalent to the implementation of `textMeshProUGUI.DisplayReasoningStreamWithEventsAsync`.

In theory, non-reasoning streaming messages can also be converted into an event stream, but I recommend against doing so. Directly using `await foreach` is the best approach.  
Converting to an event stream would introduce additional overhead, and non-reasoning streaming messages are relatively simple.

In summary, the `TMP_Text` extension methods provide a way to convert `non-reasoning streaming messages` into an event stream, but I advise against using it.  
Since the extension methods all take `UniTaskCancelableAsyncEnumerable<ChatCompletion>` as a parameter, please pay attention to the method names.  
Incorrectly mixing reasoning and non-reasoning streaming messages may cause unnecessary overhead.

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

DeepSeekChat deepSeekChat = builder.Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hi,my name is player!")
        .Base.SetModel(ChatModel.Reasoner)
    .Build(youApiKey);


var asyncEnumerable = await deepSeekChat.StreamChatCompletionsEnumerableAsync(destroyCancellationToken);

// You can set the color in the Inspector
// This color represents the reasoning content color
var colorHex = ColorUtility.ToHtmlStringRGB(new Color(1, 0, 1));

// You can also create a custom ChatCompletionEvent directly via the constructor

// If needed, you can inherit the `IChatCompletionRunning` interface to change the behavior
var chatCompletionEvent = new ChatCompletionEvent();

chatCompletionEvent.ReasoningEventSetting
    // Event when starting to receive reasoning content
    // Methods starting with "Set" will override the delegate; use methods starting with "Append" to add instead.
    .SetEnter(completion =>
    {
        var message = completion.GetMessage();
        chatText.text = string.IsNullOrEmpty(colorHex)
            ? message.ReasoningContent
            : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
    })
    // Event when receiving content (excluding the first piece)
    .SetUpdate(completion => chatText.text += completion.GetMessage().ReasoningContent)
    // Event when reception ends (completion is the same as the last piece, i.e., the last update)
    .SetExit(_ => chatText.text += "</color>\n\n");


chatCompletionEvent.ContentEventSetting.SetEnter(completion => chatText.text += completion.GetMessage().Content)
    .SetUpdate(completion => chatText.text += completion.GetMessage().Content);

// Trigger the event stream (do not await foreach asyncEnumerable)
await chatCompletionEvent.DisplayChatStreamAsync(asyncEnumerable);
```

---

### 2. For Multi-Turn Chats, Declare Some Classes as Follows

#### 2.1 Request Configurator

First, you need to create a *request configurator* where you can set model parameters such as `MaxToken`, `Temperature`, `TopK`, etc.  
If your project uses the `Odin` plugin, you can use the `[ShowInInspector]` attribute to display the request configurator.

```csharp
[Sirenix.OdinInspector.ShowInInspector]
public Xiyu.UniDeepSeek.ChatRequestParameter _setting = new();
```

Now you need a *requester*, which is responsible for sending requests and obtaining responses. During initialization, you should pass the ***request configurator*** and your ***API key*** respectively.  
If you want to reuse the `DeepSeekChat` class, declare it as a field of the class.

```csharp
private Xiyu.UniDeepSeek.DeepSeekChat _deepSeekChat;
private void Awake()
{
    _deepSeekChat = new Xiyu.UniDeepSeek.DeepSeekChat(_setting, YourApiKey);
}
```

Now you can start calling the `ChatCompletionAsync` method of the `DeepSeekChat` class to get chat results. Also, don’t forget to add messages in `ChatRequestParameter`.

You can pass a `CancellationToken` to cancel the request. When a cancellation is requested, the method will cancel at an appropriate time.

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

`deepSeekChat.ChatCompletionAsync()` returns a `UniTask<(ChatState, ChatCompletion)>`,  
where `ChatState` is an enum indicating the chat state, and `ChatCompletion` is a `ChatCompletion` object containing the chat result.

You can quickly get the chat result via `chatCompletion.GetMessage().Content`, which is equivalent to `chatCompletion.Choices[0].SourcesMessage.Content`.  
It is worth mentioning that if you have the `Odin` plugin installed, the return value `ChatCompletion` is serializable. You can assign it to a member variable and view it in the `Unity Inspector`.

---

#### 2.2 Streaming Requests

When you need to get chat results in real time, you should use the streaming method:  
`StreamChatCompletionsEnumerableAsync(Action<ChatCompletion> onCompletion, CancellationToken cancellation)`

`onCompletion` is a callback function that merges all streams in this request and returns a new `ChatCompletion` object.

***When the streaming method is canceled, it will throw an `OperationCanceledException`, whereas the ordinary async method will not throw an exception but return a `ChatState` enum.***

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

        // Calculate your result
        Debug.Log($"Cost: {completion.Usage.CalculatePrice(ChatModel.Chat):F3} CNY");
    }
    catch (OperationCanceledException e)
    {
        Debug.LogWarning("Chat request canceled: " + e.Message);
    }
}
```