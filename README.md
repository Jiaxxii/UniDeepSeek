## `UniDeepSeek` 使用说明

---

### 前言

1. [您必须学会 `UniTask` 中的一些基本操作，如果您使用过 `Task` 切换到
   `UniTask` 的使用这可能几乎无感](https://github.com/Cysharp/UniTask)
2. 我推荐您使用插件 `Odin` 插件 ***这将使得Unity Inspector中的参数更加直观***
3. `UniDeepSeek` 依赖 `UniTask` 异步库与 `Newtonsoft.Json` 解析库，请确保您的项目中已经添加了这两个库。
4. 你需要添加 `TextMeshPro`
5. 使用尽可能高的 `Unity` 版本，最低需支持 `C# 9.0` 版本。
6. 示例中Example中有功能使用示例

---

### 1. 快速开始

#### 1.1 非流式请求

我将通过流式接口来快速创建一个聊天请求，这在只需要执行一次请求不会进行多轮聊天的情况下非常有用。

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

#### 1.2 流式请求

如果你需要实时获取聊天结果，你应该使用流式方法，以下代码展示了如何使用流式方法来获取聊天结果。  

对于非深度思考的流式请求处理起来相对简单，你可以直接使用 `StreamChatCompletionsEnumerableAsync` 方法然后手动遍历它。

但是我还是准备了一个拓展方法 `DisplayChatStreamBasicAsync` 来简化流式请求的处理。  
这里我使用了对 `TMP_Text` 的拓展方法 `DisplayChatStreamBasicAsync` 来将聊天结果实时显示到 `TextMeshProUGUI` 上。

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

如果您需要自定义处理可以手段遍历 `asyncEnumerable` 并进行处理。

```csharp
await foreach (var chatCompletion in deepSeekChat.StreamChatCompletionsEnumerableAsync(
                           onCompletion: completion => { /*流式完成时会将所有的流进行合并*/ },
                           cancellation: CancellationToken.None /*建议传入一个CancellationToken来取消请求*/ ))
            {
                // 处理聊天结果
            }
```

#### 1.3 深度思考流式处理

对于深度思考的流式请求，我已经准备好了拓展方法，你仍然可以通过 `TMP_Text` 的拓展方法来调用它。  
你可以指定 `colorHex` 参数来设置深度思考的颜色。

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

### 1.4 深度思考流式处理转换为事件流

如果你在 `深度思考流式模式` 的情况下直接遍历 `deepSeekChat.StreamChatCompletionsEnumerableAsync` 会发现想要分辨消息中的
`Content` 和 `ReasoningContaent` 字段是比较困难的。  
比如，我希望深度思考的消息显示为红色，那么我可能想要在 `await foreach` 中进行各种判断，为此我你可以使用
`StreamCompletionEventFacade` 类来将消息流转换为事件流。

这里，我将使用 `TMP_Text` 的拓展方法 `DisplayReasoningStreamWithEvents` 来将聊天结果实时显示到 `TextMeshProUGUI`
上，并将消息流转换为事件流。

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

DeepSeekChat deepSeekChat = builder.Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hi,my name is player!")
        .Base.SetModel(ChatModel.Reasoner)
    .Build(youApiKey);

var asyncEnumerable = deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion, cancellationToken);

// 将消息流转换为事件流
StreamCompletionEventFacade eventFacade = textMeshProUGUI.DisplayReasoningStreamWithEvents(colorHex: ColorToHex(reasoningColor));

// 触发事件流 （不要 await foreach asyncEnumerable）
await eventFacade.Builder().DisplayChatStreamAsync(asyncEnumerable);
```

如果你需要完全自定义事件流，可以直接使用 `StreamCompletionEventFacade` 类，
以下这个例子几乎等价于 `textMeshProUGUI.DisplayReasoningStreamWithEvents` 的实现。

理论上，非深度思考的流式消息也可以转换为事件流，但是我建议你不要这么做，直接进行 `await foreach` 是最佳的方式。
如果转换为事件流会引入额外的开销，而且非深度思考的流式消息相对简单。

```csharp
RequestParameterBuilder builder = DeepSeekChat.Create();

DeepSeekChat deepSeekChat = builder.Message
    .AddSystemMessage(systemPrompt)
    .AddUserMessage("hi,my name is player!")
    .Base.SetModel(ChatModel.Reasoner)
    .Build(youApiKey);


var asyncEnumerable = await deepSeekChat.StreamChatCompletionsEnumerableAsync(destroyCancellationToken);

// 你可以在 Inspector 中设置颜色
// 这个颜色表示深度思考内容的颜色
var colorHex = ColorUtility.ToHtmlStringRGB(new Color(1, 0, 1));

// 你可以可以直接通过构造函数来创建自定义的 StreamCompletionEventFacade

// 如果有需要，你可以通过构造函数传入 `IStreamCompletionConsumer` 接口的实现类以改变工作方式
var eventFacade = StreamCompletionEventFacade.CreateByDefaultConsumer();


eventFacade.ReasoningEvent
    // 开始接收到深度思考内容时的事件
    // Set 开头的方法会覆盖委托，如果需要追加请使用 Append 开头的方法
    .SetEnter(completion =>
    {
        var message = completion.GetMessage();
        chatText.text = string.IsNullOrEmpty(colorHex)
            ? message.ReasoningContent
            : $"<color={(colorHex.StartsWith('#') ? colorHex : $"#{colorHex}")}>{message.ReasoningContent}";
    })
    // 接收到内容时（不包含第一条）
    .SetUpdate(completion => chatText.text += completion.GetMessage().ReasoningContent)
    // 结束接收时（completion与最后一条相同，即最后一次更新）
    .SetExit(_ => chatText.text += "</color>\n\n");


eventFacade.ReasoningEvent.SetEnter(completion => chatText.text += completion.GetMessage().Content)
    .SetUpdate(completion => chatText.text += completion.GetMessage().Content);

// 触发事件流 （不要 await foreach asyncEnumerable）
await eventFacade.Builder().DisplayChatStreamAsync(asyncEnumerable);
```

---

### 2. 多轮聊天需要将部分类声明为这段

#### 2.1 请求配置器

首先你需要创建一个 *请求配置器* ，你可以在其中设置模型的相关参数，如：`MaxToken` 、 `Temperature` 、 `TopK` 等。  
如果你的项目中使用了 `Odin` 插件，你可以使用 `[ShowInInspector]` 特性来显示请求配置器。

```csharp
[Sirenix.OdinInspector.ShowInInspector]
public Xiyu.UniDeepSeek.ChatRequestParameter _setting = new();
```

现在你需要一个 *请求器* ，它负责发送请求并获取响应，初始化时你应该分别传入 ***请求配置器*** 和你的 ***API密钥*** 。  
如果你要复用 `DeepSeekChat` 类，请将其声明为类的字段。

```csharp
private Xiyu.UniDeepSeek.DeepSeekChat _deepSeekChat;
private void Awake()
{
    _deepSeekChat = new Xiyu.UniDeepSeek.DeepSeekChat(_setting, YourApiKey);
}
```

现在你可以开始调用 `DeepSeekChat` 类的 `ChatCompletionAsync`来获取聊天结果，对了别忘记再 `ChatRequestParameter` 中添加消息。

你可以传入一个 `CancellationToken` 来取消请求，当你要求取消时方法会在合适的时机进行取消。

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

`deepSeekChat.ChatCompletionAsync()` 返回一个 `UniTask<(ChatState, ChatCompletion)>`，
其中 `ChatState` 是一个枚举类型，表示聊天的状态，`ChatCompletion` 是一个 `ChatCompletion` 对象，它包含了聊天的结果。

你可以通过 `chatCompletion.GetMessage().Content` 来快速获取聊天的结果，这等价与
`chatCompletion.Choices[0].SourcesMessage.Content`  
值得一提的是当你安装了 `Odin` 插件后返回值 `ChatCompletion` 是可序列化的，你可以将其结果赋值给一个成员变量，并在
`Unity Inspector` 中查看。

---

#### 2.2 流式请求

当你需要实时获取聊天结果时，你应该使用流式方法
`StreamChatCompletionsEnumerableAsync(Action<ChatCompletion> onCompletion, CancellationToken cancellation)`

`onCompletion` 是回调函数，它会将本次请求中所有的流进行合并并且创建一个新的 `ChatCompletion` 对象返回。

***当流式方法被要求取消时将抛出 `OperationCanceledException` 异常，而普通的异步方法不会抛出异常而是返回一个 `ChatState`
枚举类型。***

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

        // 统计你的结果
        Debug.Log($"花费：{completion.Usage.CalculatePrice(ChatModel.Chat):F3}元");
    }
    catch (OperationCanceledException e)
    {
        Debug.LogWarning("取消了聊天请求:" + e.Message);
    }
}
```
