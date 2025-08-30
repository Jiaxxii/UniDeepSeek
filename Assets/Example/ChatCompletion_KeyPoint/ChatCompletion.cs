using System.Threading;
using Cysharp.Threading.Tasks;
using Example.Base;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.SettingBuilding;

namespace Example.ChatCompletion_KeyPoint
{
    // 最基础的聊天功能，适用与不需要实时返回结果的场景
    // this is the basic chat function, suitable for scenarios that do not require real-time results
    public class ChatCompletion : ChatBase
    {
        // 默认用法
        protected override async UniTaskVoid StartForget(CancellationToken cancellationToken)
        {
            requestParameter.Messages.Add(new SystemMessage(systemPrompt));
            requestParameter.Messages.Add(new UserMessage("What's your name?"));

            textMeshProUGUI.text = "send message:" + requestParameter.Messages[^1].Content;

            var (state, chatCompletion) = await _deepSeekChat.ChatCompletionAsync(cancellationToken);
            if (state == ChatState.Success)
            {
                textMeshProUGUI.text = chatCompletion.GetMessage().Content;
            }
        }
        

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button(DrawResult = false, Name = "流畅接口形式")]
#endif
        // 流畅接口形式
        private async UniTaskVoid StartForget()
        {
            Debug.Log("发送消“hello Xiyu.”");

            RequestParameterBuilder builder = DeepSeekChat.Create();

            builder.SetModel(ChatModel.Chat)
                .Message
                .AddSystemMessage(systemPrompt)
                .AddUserMessage("hello Xiyu.");

            DeepSeekChat deepSeekChat = builder.Build(ApiKey);

            var (state, chatCompletion) = await deepSeekChat.ChatCompletionAsync(destroyCancellationToken);
            if (state == ChatState.Success)
            {
                Debug.Log(chatCompletion.GetMessage().Content);
            }
        }
    }
}