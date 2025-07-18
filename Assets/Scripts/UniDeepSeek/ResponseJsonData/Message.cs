using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.Tools;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if RIDER
using JetBrains.Annotations;
#else
using Xiyu.UniDeepSeek.Annotations;
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
            _role = role;
            _content = content;
            _reasoningContent = reasoningContent;
            _toolCalls = toolCalls;
        }

        #region Role

#if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, LabelText("角色"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("生成这条消息的角色。")]
        private RoleType _role;

        // [JsonProperty("role")]
        public RoleType Role => _role;

        #endregion

        #region Content

        private string _content;
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("消息"), HideLabel, ShowIf("@this.Content != null && this.Content.Length > 0")]
#else
        [SerializeField]
#endif
        [Tooltip("该 completion 的内容。"), TextArea(2, 5)]
        public string Content
        {
            get => _content;
            private set => _content = value;
        }

        #endregion

        #region ReasoningContent

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("消息"), LabelText("推理内容"), ShowIf("@this.ReasoningContent != null && this.ReasoningContent.Length > 0")]
#else
        [SerializeField]
#endif
        [Tooltip("仅适用于 deepseek-reasoner 模型。内容为 assistant 消息中在最终答案之前的推理内容。"), TextArea(3, 10)]
        private string _reasoningContent;

        [CanBeNull]
        [JsonIgnore]
        public string ReasoningContent
        {
            get => _reasoningContent;
            private set => _reasoningContent = value;
        }

        #endregion

        #region ToolCalls

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("工具调用"), ShowIf("@this.ToolCalls != null && this.ToolCalls.Count > 0")]
#else
        [SerializeField]
#endif
        [Tooltip("模型生成的 tool 调用，例如 function 调用。")]
        private List<FunctionCall> _toolCalls;

        [CanBeNull] public List<FunctionCall> ToolCalls => _toolCalls;

        #endregion

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