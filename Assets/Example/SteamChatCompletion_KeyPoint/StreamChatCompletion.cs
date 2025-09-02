using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Example.Base;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.SettingBuilding;
using Xiyu.UniDeepSeek.UnityTextMeshProUGUI;

namespace Example.SteamChatCompletion_KeyPoint
{
    public class StreamChatCompletion : ChatBase
    {
        [SerializeField] private ChatCompletion chatCompletion;

        // 标准流式处理
        // This is the standard streaming processing
        protected override async UniTaskVoid StartForget(CancellationToken cancellationToken)
        {
            requestParameter.Messages.Add(new SystemMessage(systemPrompt));
            requestParameter.Messages.Add(new UserMessage("Can you call me 'zako~ zako~'?"));

            textMeshProUGUI.text = string.Empty;
            try
            {
                await foreach (var extract in _deepSeekChat.StreamChatCompletionsEnumerableAsync(
                                   onCompletion: completion => chatCompletion = completion,
                                   cancellation: cancellationToken))
                {
                    textMeshProUGUI.text += extract.GetMessage().Content;
                }

                Debug.Log($"完整消息：{chatCompletion.GetMessage().Content}");
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning($"StreamChatCompletion canceled: {e.Message}");
            }

            // 如果你不关心合并的消息，可以直接使用这个方法，你甚至可以不指定 cancellationToken 与 try-catch，但是我不推荐这样做
            // if you don't care about merged messages, you can use this method directly, you can even not specify the cancellation token and try-catch, but I don't recommend doing so

            // await foreach (var extract in await _deepSeekChat.StreamChatCompletionsEnumerableAsync(cancellationToken))
            // {
            //     textMeshProUGUI.text += extract.GetMessage().Content;
            // }
        }


#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button(DrawResult = false, Name = "流畅接口形式")]
#endif
        // 流畅接口形式
        // This is the fluent interface form
        private async UniTaskVoid Other_A()
        {
            Debug.Log("发送消息：Can you call me 'zako~ zako~'?");

            RequestParameterBuilder builder = DeepSeekChat.Create();

            builder.SetModel(ChatModel.Chat)
                .Message
                .AddSystemMessage(systemPrompt)
                .AddUserMessage("Can you call me 'zako~ zako~'?");

            DeepSeekChat deepSeekChat = builder.Build(ApiKey);

            // 接下来做的事情与上面方法的一样
            try
            {
                await foreach (var extract in deepSeekChat.StreamChatCompletionsEnumerableAsync(
                                   onCompletion: completion => chatCompletion = completion,
                                   cancellation: destroyCancellationToken))
                {
                    Debug.Log(extract.GetMessage().Content);
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.LogWarning($"StreamChatCompletion canceled: {e.Message}");
            }
        }

#if ODIN_INSPECTOR
        private bool IsPlaying => Application.isPlaying;

        [Sirenix.OdinInspector.Button(DrawResult = false, Name = "流畅接口形式 + 拓展方法")]
        [Sirenix.OdinInspector.ShowIf("@IsPlaying == true")]
#endif
        // 流畅接口形式 + 拓展方法
        // This is the fluent interface form with extension methods
        private async UniTaskVoid Other_B()
        {
            Debug.Log("发送消息：Can you call me 'zako~ zako~'?");

            RequestParameterBuilder builder = DeepSeekChat.Create();

            builder.SetModel(ChatModel.Chat)
                .Message
                .AddSystemMessage(systemPrompt)
                .AddUserMessage("Can you call me 'zako~ zako~'?");

            DeepSeekChat deepSeekChat = builder.Build(ApiKey);

            var asyncEnumerable = deepSeekChat.StreamChatCompletionsEnumerableAsync(completion => chatCompletion = completion);

            textMeshProUGUI.text = string.Empty;
            await textMeshProUGUI.DisplayChatStreamBasicAsync(asyncEnumerable);
        }
    }
}