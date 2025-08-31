## `UniDeepSeek` User Manual

---

### Preface

1. [You must learn some basic operations in `UniTask`. If you have used `Task` and are switching to `UniTask`, the transition should feel almost seamless.](https://github.com/Cysharp/UniTask)
2. It is recommended that you use the `Odin` plugin. ***This will make parameters in the Unity Inspector more intuitive.***
3. `UniDeepSeek` depends on the `UniTask` asynchronous library and the `Newtonsoft.Json` parsing library. Please ensure these two libraries are added to your project.
4. You need to add `TextMeshPro`.
5. Use the highest possible version of `Unity`, with a minimum requirement of supporting `C# 9.0`.
6. Functional usage examples are provided in the Example folder.

---

### 1. Quick Start

#### 1.1 Non-Streaming Requests

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

#### 1.2 Streaming Requests

If you need to get chat results in real time, you should use the streaming method. The following code demonstrates how to use the streaming method to obtain chat results.  
Handling streaming requests for non-reasoning models is relatively simple. You can directly use the `StreamChatCompletionsEnumerableAsync` method and manually iterate through it.  
However, I have also prepared an extension method `DisplayChatStreamBasicAsync` to simplify the processing of streaming requests.  
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
                           onCompletion: completion => { /* When streaming is complete, all streams will be merged */ },
                           cancellation: CancellationToken.None /* It is recommended to pass a CancellationToken to cancel the request */ ))
            {
                // Process the chat result
            }
```

#### 1.3 Reasoning Streaming Processing

For reasoning streaming requests, I have prepared extension methods. You can still call them via the `TMP_Text` extension method.  
You can specify the `colorHex` parameter to set the color for the reasoning content.

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

If you directly iterate through `deepSeekChat.StreamChatCompletionsEnumerableAsync` in `Reasoning Streaming Mode`, you may find it difficult to distinguish between the `Content` and `ReasoningContent` fields in the message.  
For example, if I want reasoning messages to be displayed in red, I might need to make various judgments inside `await foreach`. To address this, you can use the `ChatCompletionEvent` class to convert the message stream into an event stream.

Here, I will use the `TMP_Text` extension method `DisplayReasoningStreamWithEvent` to display the chat results in real time on a `TextMeshProUGUI` component and convert the message stream into an event stream.  
You can save `chatCompletionEvent` as a global variable and reuse it in subsequent requests.

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

DeepSeekChat deepSeekChat = builder.Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hi,my name is player!")
        .Base.SetModel(ChatModel.Reasoner)
    .Build(youApiKey);

var asyncEnumerable = deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion, cancellationToken);

// Convert the message stream to an event stream
ChatCompletionEvent chatCompletionEvent = textMeshProUGUI.DisplayReasoningStreamWithEvents(colorHex: ColorToHex(reasoningColor));

// Trigger the event stream (Do not use await foreach on asyncEnumerable)
await chatCompletionEvent.DisplayChatStreamAsync(asyncEnumerable);
```

If you need to fully customize the event stream, you can directly use the `ChatCompletionEvent` class.  
The following example is almost equivalent to the implementation of `textMeshProUGUI.DisplayReasoningStreamWithEvents`.

In theory, non-reasoning streaming messages can also be converted into event streams, but I recommend against it. Using `await foreach` directly is the best approach.  
Converting to an event stream would introduce additional overhead, and non-reasoning streaming messages are relatively simple.

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
    // Methods starting with 'Set' will overwrite the delegate. Use methods starting with 'Append' if you need to add.
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

// Trigger the event stream (Do not use await foreach on asyncEnumerable)
await chatCompletionEvent.DisplayChatStreamAsync(asyncEnumerable);
```

---

### 2. Multi-Turn Chat Requires Declaring Some Classes in This Section

#### 2.1 Request Configurator

First, you need to create a *Request Configurator* where you can set model parameters such as `MaxToken`, `Temperature`, `TopK`, etc.  
If your project uses the `Odin` plugin, you can use the `[ShowInInspector]` attribute to display the request configurator.

```csharp
[Sirenix.OdinInspector.ShowInInspector]
public Xiyu.UniDeepSeek.ChatRequestParameter _setting = new();
```

Now you need a *Requester*, which is responsible for sending requests and receiving responses. During initialization, you should pass the ***Request Configurator*** and your ***API Key*** respectively.  
If you want to reuse the `DeepSeekChat` class, declare it as a field of the class.

```csharp
private Xiyu.UniDeepSeek.DeepSeekChat _deepSeekChat;
private void Awake()
{
    _deepSeekChat = new Xiyu.UniDeepSeek.DeepSeekChat(_setting, YourApiKey);
}
```

Now you can start calling the `ChatCompletionAsync` method of the `DeepSeekChat` class to get chat results. Oh, and don’t forget to add messages in `ChatRequestParameter`.

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

`deepSeekChat.ChatCompletionAsync()` returns a `UniTask<(ChatState, ChatCompletion)>`, where `ChatState` is an enum representing the chat state, and `ChatCompletion` is a `ChatCompletion` object containing the chat result.

You can quickly get the chat result via `chatCompletion.GetMessage().Content`, which is equivalent to `chatCompletion.Choices[0].SourcesMessage.Content`.  
It is worth mentioning that if you have the `Odin` plugin installed, the return value `ChatCompletion` is serializable. You can assign it to a member variable and view it in the `Unity Inspector`.

---

#### 2.2 Streaming Requests

When you need real-time chat results, you should use the streaming method:  
`StreamChatCompletionsEnumerableAsync(Action<ChatCompletion> onCompletion, CancellationToken cancellation)`

`onCompletion` is a callback function that merges all streams from this request and creates a new `ChatCompletion` object to return.

***When the streaming method is canceled, it will throw an `OperationCanceledException`, whereas the regular asynchronous method will not throw an exception but return a `ChatState` enum instead.***

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
        Debug.LogWarning("Chat request canceled:" + e.Message);
    }
}
```