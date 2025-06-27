using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;
using Message = Xiyu.UniDeepSeek.MessagesType.Message;


#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

public class Test : SerializedMonoBehaviour
#else
public class Test : MonoBehaviour
#endif
{
    [SerializeField]
#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
    private ChatRequestParameter chatRequestParameter = new()
    {
        Messages = new List<Message> { new UserMessage("杭州今天的天气怎么样？") }
    };

    [SerializeField]
#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
    private ChatCompletion chatCompletion;

    private CancellationTokenSource _cancellationTokenSource;
#if ODIN_INSPECTOR
    [Button("创建\\取消令牌", DrawResult = true)]
#endif
    private string CancelTest()
    {
        if (_cancellationTokenSource == null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            return "创建令牌";
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = null;
        return "取消令牌";
    }

#if ODIN_INSPECTOR
    [Button("Chat Completion 取消请求测试", DrawResult = false), PropertySpace(50)]
#endif
    private async UniTaskVoid ChatTest()
    {
        var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
        var deepSeekChat = new DeepSeekChat(chatRequestParameter, apiKey);

        chatRequestParameter.Messages.Clear();
        chatRequestParameter.Messages.Add(new UserMessage("我叫西"));

        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(3));

        var (state, completion) = await deepSeekChat.ChatCompletionAsync(cts.Token);

        Debug.Log($"Chat Completion State: {state}");

        if (state == ChatState.Success)
        {
            Debug.Log(completion.Choices[0].SourcesMessage.Content);
        }
    }

#if ODIN_INSPECTOR
    [Button("Stream Chat Completion 取消请求测试", DrawResult = false), PropertySpace(20)]
#endif
    private async UniTaskVoid StreamChatTest()
    {
        var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
        var deepSeekChat = new DeepSeekChat(chatRequestParameter, apiKey);

        chatRequestParameter.Messages.Clear();
        chatRequestParameter.Messages.Add(new UserMessage("我叫西"));

        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(5));

        try
        {
            await foreach (var completion in await deepSeekChat.StreamChatCompletionsEnumerableAsync(cancellation: cts.Token))
            {
                Debug.Log(completion.Choices[0].SourcesMessage.Content);
            }

            Debug.Log("Stream Chat Test Completed");
        }
        catch (OperationCanceledException e)
        {
            Debug.LogWarning("Stream Chat Test Cancelled: " + e.Message);
        }
    }

#if ODIN_INSPECTOR
    [Button("对话前缀续写 取消请求测试", DrawResult = false), PropertySpace(20)]
#endif
    private async UniTaskVoid PrefixChatTest()
    {
        var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
        var deepSeekChat = new DeepSeekChat(chatRequestParameter, apiKey);

        chatRequestParameter.Messages.Clear();
        chatRequestParameter.Messages.Add(new UserMessage("我叫西"));

        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(3));

        var (state, completion) = await deepSeekChat.ChatPrefixCompletionAsync("你好呀，我叫deepseek，你可以叫我小深度", cancellationToken: cts.Token);

        Debug.Log($"Chat Completion State: {state}");

        if (state == ChatState.Success)
        {
            Debug.Log(completion.Choices[0].SourcesMessage.Content);
        }
    }

#if ODIN_INSPECTOR
    [Button("流式对话前缀续写 取消请求测试", DrawResult = false), PropertySpace(20)]
#endif
    private async UniTaskVoid StreamPrefixChatTest()
    {
        var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
        var deepSeekChat = new DeepSeekChat(chatRequestParameter, apiKey);

        chatRequestParameter.Messages.Clear();
        chatRequestParameter.Messages.Add(new UserMessage("我叫西，你能告诉我`Unity`是什么吗？"));

        var cts = new CancellationTokenSource();
        cts.CancelAfterSlim(TimeSpan.FromSeconds(5));

        try
        {
            await foreach (var completion in deepSeekChat.ChatPrefixStreamCompletionsEnumerableAsync("你好呀，我叫deepseek，你可以叫我小深度", null, cancellationToken: cts.Token))
            {
                Debug.Log(completion.Choices[0].SourcesMessage.Content);
            }

            Debug.Log("<color=green>Stream Chat Test Completed</color>");
        }
        catch (OperationCanceledException exception)
        {
            Debug.LogWarning("Stream Prefix Chat Test Cancelled: " + exception.Message);
        }
    }

#if ODIN_INSPECTOR
    [Button("对话前缀续写测试（流式-深度思考模型）", DrawResult = false)]
#endif
    private async UniTaskVoid StreamThinkConversationPrefixContinuationTest()
    {
        var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
        var deepSeekChat = new DeepSeekChat(chatRequestParameter, apiKey);

        chatRequestParameter.Messages.Clear();
        chatRequestParameter.Messages.Add(new UserMessage("我叫西"));

        ChatCompletion usageCompletion = null;
        var think = "嗯,用户说ta叫西，这听起来像女孩子的名称，我可以用可爱的语气回答ta。女孩子可能都喜欢可爱的动物，比如猫。我可以模仿小猫的语气来回答ta。比如使用“喵”、“肉爪”、“猫耳朵”等词语。";
        await foreach (var completion in deepSeekChat.ChatPrefixStreamCompletionsEnumerableAsync("西，", think: think, onCompletion: c => usageCompletion = c))
        {
            Debug.Log(completion.Choices[0].SourcesMessage.Content);
        }

        Debug.Log("完整消息：" + usageCompletion.Choices[0].SourcesMessage.Content);
        Debug.Log("完整消息(深度思考)：" + usageCompletion.Choices[0].SourcesMessage.ReasoningContent);
    }

#if ODIN_INSPECTOR
    [Button("对话前缀续写测试（流式）", DrawResult = false)]
#endif
    private async UniTaskVoid StreamConversationPrefixContinuationTest()
    {
        var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
        var deepSeekChat = new DeepSeekChat(chatRequestParameter, apiKey);

        chatRequestParameter.Messages.Clear();
        chatRequestParameter.Messages.Add(new UserMessage("我叫西"));

        ChatCompletion usageCompletion = null;
        await foreach (var completion in deepSeekChat.ChatPrefixStreamCompletionsEnumerableAsync("你好呀，我叫deepseek，你可以叫我小深度", null, onCompletion: c => usageCompletion = c))
        {
            Debug.Log(completion.Choices[0].SourcesMessage.Content);
        }

        Debug.Log("完整消息：" + usageCompletion.Choices[0].SourcesMessage.Content);
    }
}