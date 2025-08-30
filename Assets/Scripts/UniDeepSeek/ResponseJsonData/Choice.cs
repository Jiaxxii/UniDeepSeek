using System;
using Newtonsoft.Json;
using UnityEngine;
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
    [Serializable]
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    public class Choice
    {
        [JsonConstructor]
        public Choice(FinishReasonType? finishReason, int index, [CanBeNull] Message message, [CanBeNull] Message delta, [CanBeNull] Logprobs logprobs)
        {
            this.finishReason = finishReason ?? FinishReasonType.Null;
            this.index = index;
            if (message != null)
                Message = message;
            if (delta != null)
                Delta = delta;
            this.logprobs = logprobs;
        }

        #region FinishReason

#if ODIN_INSPECTOR
        [HorizontalGroup("info"), LabelText("停止原因"), ReadOnly]
#endif
        [SerializeField, Tooltip("模型停止生成 token 的原因。")]
        private FinishReasonType finishReason;

        /// <summary>
        /// 模型停止生成 token 的原因。
        /// </summary>
        public FinishReasonType FinishReason => finishReason;

        #endregion

        #region Index

#if ODIN_INSPECTOR
        [HorizontalGroup("info"), ReadOnly]
#endif
        [SerializeField, Tooltip("该 completion 在模型生成的 completion 的选择列表中的索引。")]
        private int index;

        /// <summary>
        /// 该 completion 在模型生成的 completion 的选择列表中的索引。
        /// </summary>

        public int Index => index;

        #endregion


        #region Message

#if ODIN_INSPECTOR
        [InlineProperty, HideLabel, ReadOnly]
#endif
        [SerializeField, Tooltip("模型生成的 completion 消息。")]
        private Message sourceMessage;

        /// <summary>
        /// 模型生成的 completion 消息。
        /// </summary>
        [JsonIgnore]
        public Message SourcesMessage
        {
            get => sourceMessage;
            private set => sourceMessage = value;
        }

        #endregion


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

        #region Logprobs

#if ODIN_INSPECTOR
        [ReadOnly, InlineProperty, HideLabel, ShowIf("@this.Logprobs!= null && this.Logprobs.Content != null && this.Logprobs.Content.Length > 0")]
#endif
        [SerializeField, Tooltip("该 choice 的对数概率信息")]
        private Logprobs logprobs;

        /// <summary>
        /// 该 choice 的对数概率信息。
        /// </summary>

        [CanBeNull]
        public Logprobs Logprobs => logprobs;

        #endregion
    }
}