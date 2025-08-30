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

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    [Serializable]
    public partial class FimChatCompletion
    {
        [JsonConstructor]
        public FimChatCompletion(string id, int created, ChatModel model, string systemFingerprint, string @object, FimChoice[] choices, [CanBeNull] Usage usage)
        {
            this.id = id;
            this.created = created;
            chatModel = model;
            this.systemFingerprint = systemFingerprint;
            this.@object = @object;
            this.choices = choices;
            this.usage = usage;
        }

        #region ID

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField]
        [Tooltip("补全响应的ID")]
        private string id;

        public string Id
        {
            get => id;
            set => id = value;
        }

        #endregion

        #region CREATED

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField, Tooltip("标志补全请求开始时间的 Unix 时间戳（以秒为单位）。")]
        private int created;

        public int Created => created;

        #endregion

        #region MODEL

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), ReadOnly]
#endif
        [SerializeField, Tooltip("补全请求使用的模型。")]
        private ChatModel chatModel;

        public ChatModel Model => chatModel;

        #endregion

        #region SYSTEMFINGERPRINT

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField, Tooltip("模型运行时的后端配置的指纹。")]
        private string systemFingerprint;

        public string SystemFingerprint => systemFingerprint;

        #endregion

        #region OBJECT

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField]
        private string @object;

        public string Object => @object;

        #endregion

        #region CHOICES

#if ODIN_INSPECTOR
        [HideLabel, InlineProperty, ReadOnly]
#endif
        [SerializeField]
        private FimChoice[] choices;

        public FimChoice[] Choices => choices;

        #endregion

        #region USAGE

#if ODIN_INSPECTOR
        [HideLabel, PropertySpace(10, SpaceAfter = 20), ReadOnly]
#endif
        [SerializeField, Tooltip("该对话补全请求的用量信息。")]
        private Usage usage;

        /// <summary>
        /// 该对话补全请求的用量信息。
        /// </summary>

        [CanBeNull]
        public Usage Usage => usage;

        #endregion
    }
}