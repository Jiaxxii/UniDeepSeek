using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.Tools;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek
{
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    [System.Serializable]
    public class Message
    {
        [JsonConstructor]
        public Message(RoleType role, string content, [CanBeNull] string reasoningContent, [CanBeNull] List<FunctionCall> toolCalls)
        {
            Role = role;
            Content = content;
            ReasoningContent = reasoningContent;
            ToolCalls = toolCalls;
        }
#if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, LabelText("角色")]
        [UnityEngine.Tooltip("生成这条消息的角色。")]
#endif
        // [JsonProperty("role")]
        public RoleType Role { get; }


#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("消息"), HideLabel, ShowIf("@this.Content != null && this.Content.Length > 0")]
        [UnityEngine.Tooltip("该 completion 的内容。"), UnityEngine.TextArea(2, 5)]
#endif
        public string Content { get; private set; }

#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("消息"), LabelText("推理内容"), ShowIf("@this.ReasoningContent != null && this.ReasoningContent.Length > 0")]
        [UnityEngine.Tooltip("仅适用于 deepseek-reasoner 模型。内容为 assistant 消息中在最终答案之前的推理内容。"), UnityEngine.TextArea(3, 10)]
#endif
        [CanBeNull]
        [JsonIgnore]
        public string ReasoningContent { get; private set; }

#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("工具调用"), ShowIf("@this.ToolCalls != null && this.ToolCalls.Count > 0")]
        [UnityEngine.Tooltip("模型生成的 tool 调用，例如 function 调用。")]
#endif
        [CanBeNull]
        public List<FunctionCall> ToolCalls { get; }

        public void InjectPrefixAsContent(string prefix)
        {
            Content = prefix + Content;
        }

        public void InjectPrefixAsReasoningContent(string prefix, string reasoningContent)
        {
            ReasoningContent = reasoningContent;
            InjectPrefixAsContent(prefix);
        }


        public static AssistantMessage ToAssistantMessage(string content, string reasoningContent = null, string name = null)
        {
            return AssistantMessage.CreateMessage(content, reasoningContent, name);
        }

        public static implicit operator AssistantMessage(Message message)
        {
            return string.IsNullOrEmpty(message.ReasoningContent)
                ? AssistantMessage.CreateMessage(message.Content)
                : AssistantMessage.CreateMessage(message.Content, message.ReasoningContent, null);
        }
    }
}