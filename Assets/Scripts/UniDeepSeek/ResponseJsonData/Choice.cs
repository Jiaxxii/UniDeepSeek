using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
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
            _finishReason = finishReason ?? FinishReasonType.Null;
            _index = index;
            if (message != null)
                Message = message;
            if (delta != null)
                Delta = delta;
            _logprobs = logprobs;
        }

        #region FinishReason

#if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup("info"), LabelText("停止原因"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("模型停止生成 token 的原因。")]
        private FinishReasonType _finishReason;

        /// <summary>
        /// 模型停止生成 token 的原因。
        /// </summary>
        public FinishReasonType FinishReason => _finishReason;

        #endregion

        #region Index

#if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup("info"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("该 completion 在模型生成的 completion 的选择列表中的索引。")]
        private int _index;

        /// <summary>
        /// 该 completion 在模型生成的 completion 的选择列表中的索引。
        /// </summary>

        public int Index => _index;

        #endregion


        #region Message

#if ODIN_INSPECTOR
        [ShowInInspector, InlineProperty, HideLabel, ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("模型生成的 completion 消息。")]
        private Message _sourceMessage;

        /// <summary>
        /// 模型生成的 completion 消息。
        /// </summary>
        [JsonIgnore]
        public Message SourcesMessage
        {
            get => _sourceMessage;
            private set => _sourceMessage = value;
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
        [ShowInInspector, ReadOnly, InlineProperty, HideLabel, ShowIf("@this.Logprobs!= null && this.Logprobs.Content != null && this.Logprobs.Content.Length > 0")]
#else
        [SerializeField]
#endif
        [Tooltip("该 choice 的对数概率信息")]
        private Logprobs _logprobs;

        /// <summary>
        /// 该 choice 的对数概率信息。
        /// </summary>

        [CanBeNull]
        public Logprobs Logprobs => _logprobs;

        #endregion
    }
}