using System;
using System.Collections.Generic;
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
    public partial class ChatCompletion
    {
        public ChatCompletion(List<Choice> choices, string id, int created, ChatModel model, [CanBeNull] string systemFingerprint, string @object, [CanBeNull] Usage usage)
        {
            _id = id;
            _created = created;
            _model = model;
            _systemFingerprint = systemFingerprint;
            _object = @object;
            _usage = usage;
            _choices = choices;
        }

        #region Choices

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("对话结果"), BoxGroup("信息"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("模型生成的 completion 的选择列表。")]
        private List<Choice> _choices;

        /// <summary>
        /// 模型生成的 completion 的选择列表。
        /// </summary>
        public List<Choice> Choices => _choices;

        #endregion

        #region ID

        private string _id;

        /// <summary>
        /// 对话的唯一标识符。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("对话的唯一标识符。")]
        public string ID => _id;

        #endregion

        #region Created

#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("创建聊天完成时的 Unix 时间戳（以秒为单位）。")]
        private int _created;

        /// <summary>
        /// 创建聊天完成时的 Unix 时间戳（以秒为单位）。
        /// </summary>

        public int Created => _created;

        #endregion

        #region Model

#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("创建聊天完成时的 Unix 时间戳（以秒为单位）。")]
        private ChatModel _model;

        /// <summary>
        /// 创建聊天完成时的 Unix 时间戳（以秒为单位）。
        /// </summary>
        public ChatModel Model => _model;

        #endregion


        #region SystemFingerprint

#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("此指纹表示模型运行时使用的后端配置。")]
        private string _systemFingerprint;

        /// <summary>
        /// 此指纹表示模型运行时使用的后端配置。
        /// </summary>

        [CanBeNull]
        public string SystemFingerprint => _systemFingerprint;

        #endregion

        #region Object

#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("对象的类型, 其值为 chat.completion。")]
        private string _object;

        /// <summary>
        /// 对象的类型, 其值为 chat.completion。
        /// </summary>

        public string Object => _object;

        #endregion

        #region Usage

#if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, ReadOnly]
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