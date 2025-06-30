## `UniDeepSeek` 使用说明

---

### 前言

1. 我们强烈推荐您使用插件 `Odin` 插件 ***（提示：二手市场有价格极低的 `Odin` 插件，请谨慎购买！）***
2. `UniDeepSeek` 依赖 `UniTask` 异步库与 `Newtonsoft.Json` 解析库，请确保您的项目中已经添加了这两个库。
3. 使用尽可能高的 `Unity` 版本，最低需支持 `C# 9.0` 版本。

---

### 1. 快速开始

首先你需要创建一个 *请求配置器* ，你可以在其中设置模型的相关参数，如：`MaxToken` 、 `Temperature` 、 `TopK` 等。  
如果你的项目中使用了 `Odin` 插件，你可以使用 `[ShowInInspector]` 特性来显示请求配置器。

```csharp
[Sirenix.OdinInspector.ShowInInspector]
public Xiyu.UniDeepSeek.ChatRequestParameter _setting = new();
```

现在你需要一个 *请求器* ，它负责发送请求并获取响应，初始化时你应该分别传入请求配置器和你的 API 密钥。  
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

你可以通过 `chatCompletion.GetMessage().Content` 来快速获取聊天的结果，这等价与 `chatCompletion.Choices[0].SourcesMessage.Content`  
值得一提的是当你安装了 `Odin` 插件后返回值 `ChatCompletion` 是可序列化的，你可以将其结果赋值给一个成员变量，并在 `Unity Inspector` 中查看。 

---

### 2. 流式请求

当你需要实时获取聊天结果时，你应该使用流式方法 `StreamChatCompletionsEnumerableAsync(Action<ChatCompletion> onCompletion, CancellationToken cancellation)`  

`onCompletion` 是回调函数，它会将本次请求中所有的流进行合并并且创建一个新的 `ChatCompletion` 对象返回。

***当流式方法被要求取消时将抛出 `OperationCanceledException` 异常，而普通的异步方法不会抛出异常而是返回一个 `ChatState` 枚举类型。***

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

## 文档未写完[2025年6月30日17:42]
