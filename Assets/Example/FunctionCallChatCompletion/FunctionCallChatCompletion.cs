using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Example.Base;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.Tools;
using Random = UnityEngine.Random;

namespace Example.FunctionCallChatCompletion
{
    public class FunctionCallChatCompletion : ChatBase
    {
        private const string UserMessage =
            // 我叫西，我想知道我目前多少级，还有杭州今天天气怎么样？
            "My name is Xi, and I would like to know my current level and what the weather is like in Hangzhou today?";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("@completion.ID != \"\"")]
#endif
        [SerializeField]
        private ChatCompletion completion;

        protected override async UniTaskVoid StartForget(CancellationToken cancellationToken)
        {
            requestParameter.ToolChoice.FunctionCallModel = FunctionCallModel.Auto;
            AddFunctionCallFirst(_deepSeekChat);
            AddFunctionCallSecond(_deepSeekChat);

            requestParameter.Messages.Add(new UserMessage(UserMessage));
            Debug.Log($"发送消息：{UserMessage}");


            var (state, chatCompletion) = await _deepSeekChat.ChatCompletionAsync(cancellationToken);
            if (state == ChatState.Success)
            {
                textMeshProUGUI.text = chatCompletion.GetMessage().Content;
            }

            completion = chatCompletion;
        }


        // 模拟天气查询函数
        private static void AddFunctionCallFirst(DeepSeekChat deepSeekChat)
        {
            const string description = "Get weather of an location, the user shoud supply a location first.";
            const string parameters = "{\"type\":\"object\",\"properties\":{\"location\":{\"type\":\"string\",\"description\":\"城市名称\"}}}";
            deepSeekChat.RegisterFunction("get_weather", description, parameters, (fc, _) =>
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
                // Try not to throw exceptions, but instead return an error state.
                if (await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token ?? CancellationToken.None)
                        .SuppressCancellationThrow())
                {
                    // 我在这里模拟了一个延迟，如果 `token` 被取消就会返回 `ChatState.Cancel`
                    // I simulated a delay here, and if the `token` is cancelled, it will return `ChatState.Cancel`
                    return new FunctionCallResult(null, null, ChatState.Cancel, "Task was canceled.");
                }

                var arguments = JObject.Parse(functionCall.Function.Arguments);

                // 译：未查询到用户信息
                var query = "don't know this user info";
                // 从参数中获取用户名称
                if (arguments.TryGetValue("user_name", out var jToken))
                {
                    query = $"{jToken.Value<string>()}，level is {Random.Range(0, 100)}.";
                }

                Debug.Log($"调用工具：<color=#E4BF6B>用户等级查询</color> -> <color=#E08E4A>{query}</color>");

                return new FunctionCallResult(functionCall.Id, query, ChatState.Success);
            }
        }
    }
}