using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Example.Base;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;

namespace Example.StreamPrefixChatCompletion
{
    public class StreamPrefixChatCompletion : ChatBase
    {
        [SerializeField] private ChatCompletion chatCompletion;

        protected override async UniTaskVoid StartForget(CancellationToken cancellationToken)
        {
            requestParameter.Messages.Add(new SystemMessage(systemPrompt));
            requestParameter.Messages.Add(new UserMessage("I hate you~baby~"));

            // 哈？你这么说的时候，我
            // 如果你的前缀本身就是一句完整的话，那么AI可能不会进行续写，或者直接添加句号
            const string prefix = "ha? When you say that, my tail";

            // 接下来Ai将会以prefix为开头，尝试完成剩余的句子
            textMeshProUGUI.text = $"wait for 3 seconds,chat assistance with chat completion start with prefix: <color=#D8A0DF>{prefix}</color>";

            await UniTask.WaitForSeconds(3, cancellationToken: cancellationToken);

            textMeshProUGUI.text = prefix;

            try
            {
                await foreach (var extract in _deepSeekChat.ChatPrefixStreamCompletionsEnumerableAsync(prefix, null,
                                   onCompletion: completion => chatCompletion = completion,
                                   cancellationToken: cancellationToken))
                {
                    textMeshProUGUI.text += extract.GetMessage().Content;
                }

                Debug.Log($"完整消息:{chatCompletion.GetMessage().Content}");
            }
            catch (OperationCanceledException e)
            {
                Debug.LogError($"stream cancelled: {e.Message}");
            }
        }
    }
}