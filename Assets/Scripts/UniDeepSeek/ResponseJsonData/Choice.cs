using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek
{
    [Serializable]
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    public class Choice
    {
        [JsonConstructor]
        public Choice(FinishReasonType? finishReason, int index, [CanBeNull] Message message, [CanBeNull] Message delta, [CanBeNull] Logprobs logprobs)
        {
            FinishReason = finishReason ?? FinishReasonType.Null;
            Index = index;
            if (message != null)
                Message = message;
            if (delta != null)
                Delta = delta;
            Logprobs = logprobs;
        }

        /// <summary>
        /// 模型停止生成 token 的原因。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup("info"), LabelText("停止原因")]
        [UnityEngine.Tooltip("模型停止生成 token 的原因。")]
#endif
        public FinishReasonType FinishReason { get; }

        /// <summary>
        /// 该 completion 在模型生成的 completion 的选择列表中的索引。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup("info")]
        [UnityEngine.Tooltip("该 completion 在模型生成的 completion 的选择列表中的索引。")]
#endif
        public int Index { get; }

        /// <summary>
        /// 模型生成的 completion 消息。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, InlineProperty, HideLabel]
        [UnityEngine.Tooltip("模型生成的 completion 消息。")]
#endif
        [JsonIgnore]
        public Message SourcesMessage { get; private set; }


        [CanBeNull]
        public Message Message
        {
            get => SourcesMessage;
            private set => SourcesMessage = value;
        }

        [CanBeNull]
        public Message Delta
        {
            get => SourcesMessage;
            private set => SourcesMessage = value;
        }

        /// <summary>
        /// 该 choice 的对数概率信息。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, InlineProperty, HideLabel, ShowIf("@this.Logprobs!= null && this.Logprobs.Content != null && this.Logprobs.Content.Length > 0")]
        [UnityEngine.Tooltip("该 choice 的对数概率信息")]
#endif
        [CanBeNull]
        public Logprobs Logprobs { get; }
    }
}