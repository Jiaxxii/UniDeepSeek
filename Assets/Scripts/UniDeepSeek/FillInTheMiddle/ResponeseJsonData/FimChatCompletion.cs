using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    [Serializable]
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    public partial class FimChatCompletion
    {
        [JsonConstructor]
        public FimChatCompletion(string id, int created, ChatModel model, string systemFingerprint, string @object, FimChoice[] choices, [CanBeNull] Usage usage)
        {
            _id = id;
            _created = created;
            _chatModel = model;
            _systemFingerprint = systemFingerprint;
            _object = @object;
            _choices = choices;
            _usage = usage;
        }

        #region ID

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("补全响应的ID")]
        private string _id;

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        #endregion

        #region CREATED

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("标志补全请求开始时间的 Unix 时间戳（以秒为单位）。")]
        private int _created;

        public int Created => _created;

        #endregion

        #region MODEL

#if ODIN_INSPECTOR
        [ShowInInspector, PropertySpace(10, SpaceAfter = 20), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("补全请求使用的模型。")]
        private ChatModel _chatModel;

        public ChatModel Model => _chatModel;

        #endregion

        #region SYSTEMFINGERPRINT

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("模型运行时的后端配置的指纹。")]
        private string _systemFingerprint;

        public string SystemFingerprint => _systemFingerprint;

        #endregion

        #region OBJECT

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#else
        [SerializeField]
#endif
        private string _object;

        public string Object => _object;

        #endregion

        #region CHOICES

#if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, InlineProperty, ReadOnly]
#else
        [SerializeField]
#endif
        private FimChoice[] _choices;

        public FimChoice[] Choices => _choices;

        #endregion

        #region USAGE

#if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, PropertySpace(10, SpaceAfter = 20), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("该对话补全请求的用量信息。")]
        private Usage _usage;

        /// <summary>
        /// 该对话补全请求的用量信息。
        /// </summary>

        [CanBeNull]
        public Usage Usage => _usage;

        #endregion
    }
}