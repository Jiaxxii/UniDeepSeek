using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.Tools;
using Random = UnityEngine.Random;

namespace Example.FunctionCallChatCompletion
{
    public class FunctionCallChatCompletion : MonoBehaviour
    {
        [SerializeField] private Text chatText;

        private bool _isRunning;

        private void Start()
        {
            FunctionCallChatCompletionAsync().Forget();
        }


#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("启动", DrawResult = false)]
#endif
        private async UniTaskVoid FunctionCallChatCompletionAsync()
        {
            if (_isRunning)
            {
                Debug.Log("正在运行中，请勿重复运行。");
                return;
            }

            _isRunning = true;

            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

            var requestParameter = new ChatRequestParameter();
            var deepSeekChat = new DeepSeekChat(requestParameter, apiKey);

            // 设置工具模型为自动
            requestParameter.ToolChoice.FunctionCallModel = FunctionCallModel.Auto;
            AddFunctionCallFirst(deepSeekChat);
            AddFunctionCallSecond(deepSeekChat);

            requestParameter.Messages.Add(new UserMessage("我叫西，我想知道我目前多少级，还有杭州今天天气怎么样？"));
            Debug.Log($"发送消息：{requestParameter.Messages[^1].Content}");

            // 使用 流式方法调用 同样会自动处理 FunctionCall
            var (state, chatCompletion) = await deepSeekChat.ChatCompletionAsync();

            if (Application.isPlaying)
            {
                chatText.text = $"request state: <color=#E08E4A>{state}</color>";

                if (state == ChatState.Success)
                    chatText.text = $"{chatCompletion.Choices[0].SourcesMessage.Content}";
            }
            else
            {
                Debug.Log($"request state: <color=#E08E4A>{state}</color>");
                if (state == ChatState.Success)
                    Debug.Log($"AI回复: {chatCompletion.Choices[0].SourcesMessage.Content}");
            }

            _isRunning = false;
        }


        // 模拟天气查询函数
        private static void AddFunctionCallFirst(DeepSeekChat deepSeekChat)
        {
            const string description = "Get weather of an location, the user shoud supply a location first.";
            const string parameters = "{\"type\":\"object\",\"properties\":{\"location\":{\"type\":\"string\",\"description\":\"城市名称\"}}}";
            deepSeekChat.RegisterFunction("get_weather", description, parameters, (fc, token) =>
            {
                var t = Random.Range(28, 35);
                Debug.Log("调用工具：<color=#E4BF6B>天气查询</color> -> <color=#DB5548>" + t + "°C</color>");
                return UniTask.FromResult(new FunctionCallResult(fc.Id, $"{t}°C", ChatState.Success));
            }, "location");
        }

        // 模拟用户等级查询函数
        private static void AddFunctionCallSecond(DeepSeekChat deepSeekChat)
        {
            // 查询用户的等级，用户应当提供一个名称
            const string description = "To query the user's level, the user should provide a name.";
            var parameters = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                type = "object",
                properties = new
                {
                    user_name = new
                    {
                        type = "string",
                        description = "is user name"
                    }
                }
            }, Newtonsoft.Json.Formatting.None);
            deepSeekChat.RegisterFunction("get_user_level", description, parameters, GetUserLevel, "user_name");

            return;

            async UniTask<FunctionCallResult> GetUserLevel(FunctionCall functionCall, CancellationToken? token)
            {
                // 模拟延迟
                // 尽量不要抛出异常，而是返回错误状态
                if (await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token ?? CancellationToken.None)
                        .SuppressCancellationThrow())
                {
                    return new FunctionCallResult(null, null, ChatState.Cancel, "Task was canceled.");
                }

                var arguments = JObject.Parse(functionCall.Function.Arguments);
                var query = "未查询到用户信息";
                if (arguments.TryGetValue("user_name", out var jToken))
                {
                    query = $"你好，{jToken.Value<string>()}，你目前的等级是{Random.Range(0, 100)}级。";
                }

                Debug.Log($"调用工具：<color=#E4BF6B>用户等级查询</color> -> <color=#E08E4A>{query}</color>");

                return new FunctionCallResult(functionCall.Id, query, ChatState.Success);
            }
        }
    }
}