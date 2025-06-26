using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.Tools;
using Message = Xiyu.UniDeepSeek.MessagesType.Message;
using Random = UnityEngine.Random;


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
        await foreach (var completion in deepSeekChat.ChatStreamCompletionsEnumerableAsync("西，", think: think, onCompletion: c => usageCompletion = c))
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
        await foreach (var completion in deepSeekChat.ChatStreamCompletionsEnumerableAsync("你好呀，我叫deepseek，你可以叫我小深度", null, onCompletion: c => usageCompletion = c))
        {
            Debug.Log(completion.Choices[0].SourcesMessage.Content);
        }

        Debug.Log("完整消息：" + usageCompletion.Choices[0].SourcesMessage.Content);
    }

#if ODIN_INSPECTOR
    [Button("对话前缀续写测试", DrawResult = false)]
#endif
    private async UniTaskVoid ConversationPrefixContinuationTest()
    {
        var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
        var deepSeekChat = new DeepSeekChat(chatRequestParameter, apiKey);

        chatRequestParameter.Messages.Clear();
        chatRequestParameter.Messages.Add(new UserMessage("我叫西"));

        var completion = await deepSeekChat.ChatPrefixCompletionAsync("你好呀，我叫deepseek，你可以叫我小深度");

        Debug.Log(completion.Choices[0].SourcesMessage.Content);
    }

#if ODIN_INSPECTOR
    [Button("初始化")]
#endif
    private void Init()
    {
        // chatRequestParameter.Messages.Add(new UserMessage("杭州天气怎么样？"));
        chatRequestParameter.ToolInstances.Clear();
        chatRequestParameter.ToolChoice.FunctionCallModel = FunctionCallModel.Auto;
        chatRequestParameter.ToolInstances.Add(new ToolInstance
        {
            FunctionDefine = new FunctionDefine
            {
                FunctionName = "get_weather",
                Description = "获取城市天气信息",
                JsonParameters = JsonConvert.SerializeObject(new
                {
                    type = "object",
                    properties = new
                    {
                        location = new
                        {
                            type = "string",
                            description = "城市名称"
                        }
                    }
                }, Formatting.None),
                RequiredParameters = new List<string> { "location" }
            }
        });

        Debug.Assert(chatRequestParameter.ToolInstances.Count == 1);
    }


#if ODIN_INSPECTOR
    [Button("Stream Chat Completion测试", DrawResult = false)]
#endif
    private async UniTask<string> StreamChatCompletion()
    {
        var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
        var deepSeekChat = new DeepSeekChat(chatRequestParameter, apiKey);

        var parameters = JsonConvert.SerializeObject(new
        {
            type = "object",
            properties = new
            {
                location = new
                {
                    type = "string",
                    description = "城市名称"
                }
            }
        }, Formatting.None);

        // parameters = "{\"type\":\"object\",\"properties\":{\"location\":{\"type\":\"string\",\"description\":\"城市名称\"}}}";

        deepSeekChat.RegisterFunction("get_weather", "获取城市天气信息，用户应该提供城市名称。", parameters, fc =>
        {
            // fc.Function.Arguments -> 模型参数以JOSN格式提供
            var c = Random.Range(26, 35);
            return UniTask.FromResult((fc.Id /*固定传入ID*/, $"{c}度"));
        }, "location");

        ChatCompletion usageCompletion = null;
        await foreach (var completion in deepSeekChat.StreamChatCompletionsEnumerableAsync(onCompletion: c => usageCompletion = c))
        {
            var message = completion.Choices[0].SourcesMessage;
            if (!string.IsNullOrEmpty(message.Content))
            {
                Debug.Log(message.Content);
            }
            else if (!string.IsNullOrEmpty(message.ReasoningContent))
            {
                Debug.Log($"<color=#61AFEF>{message.ReasoningContent}</color>");
            }
        }


        Debug.Log("完整消息：" + usageCompletion.Choices[0].SourcesMessage.Content);

        chatCompletion = usageCompletion;

        return chatCompletion.Choices[0].SourcesMessage.Content;
    }


#if ODIN_INSPECTOR
    [Button("SSE Read测试", DrawResult = false)]
#endif
    private async UniTaskVoid SSeReadTest()
    {
        var fileStream = File.Open(@"C:\Users\jiaxx\Desktop\temp.txt", FileMode.Open, FileAccess.Read);

        IAnalysisChatCompletionsAsync analysisChatCompletions = new StreamDeserializeCompletions();

        await foreach (var completion in analysisChatCompletions.AnalysisChatCompletion(fileStream, GeneralSerializeSettings.SampleJsonSerializerSettings))
        {
            Debug.Log(completion.Choices[0].SourcesMessage.Content);
        }
    }

#if ODIN_INSPECTOR
    [Button("反序列化ChatCompletion测试")]
#endif
    private void TestDeserializeChatCompletion()
    {
        var content = File.ReadAllText(@"C:\Users\jiaxx\AppData\Roaming\JetBrains\Rider2025.1\scratches\chat.json");

        var deserializeObject = JsonConvert.DeserializeObject<ChatCompletion>(content, GeneralSerializeSettings.SampleJsonSerializerSettings);

        chatCompletion = deserializeObject;

        Debug.Assert(chatCompletion.Choices != null);
        Debug.Assert(chatCompletion.Choices.Count > 0);
        Debug.Assert(chatCompletion.Choices[0].SourcesMessage != null);
        Debug.Assert(chatCompletion.Choices[0].SourcesMessage.Content != null);
    }


#if ODIN_INSPECTOR
    [Button("测试Chat参数")]
#endif
    private void PrintChatRequestParameter()
    {
        var verifyResult = chatRequestParameter.VerifyParams();

        if (verifyResult != ParamsStandardError.Success)
        {
            Debug.LogError($"【<color=red>{verifyResult}</color>】验证参数失败！(code:{(int)verifyResult})");
            return;
        }

        // 序列化并打印参数
        var json = chatRequestParameter.FromObjectAsToken().ToString(Formatting.None);
        Debug.Log(json);
    }
}