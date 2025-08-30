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
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    public partial class ChatCompletion
    {
        public ChatCompletion(List<Choice> choices, string id, int created, ChatModel model, [CanBeNull] string systemFingerprint, string @object, [CanBeNull] Usage usage)
        {
            this.id = id;
            this.created = created;
            this.model = model;
            this.systemFingerprint = systemFingerprint;
            this.@object = @object;
            this.usage = usage;
            this.choices = choices;
        }

        #region Choices

#if ODIN_INSPECTOR
        [LabelText("对话结果"), BoxGroup("信息"), ReadOnly]
#endif
        [SerializeField, Tooltip("模型生成的 completion 的选择列表。")]
        private List<Choice> choices;

        /// <summary>
        /// 模型生成的 completion 的选择列表。
        /// </summary>
        public List<Choice> Choices => choices;

        #endregion

        #region ID

#if ODIN_INSPECTOR
        [BoxGroup("信息"), ReadOnly]
#endif
        [SerializeField, Tooltip("对话的唯一标识符。")]
        private string id;

        /// <summary>
        /// 对话的唯一标识符。
        /// </summary>

        public string ID => id;

        #endregion

        #region Created

#if ODIN_INSPECTOR
        [BoxGroup("信息"), ReadOnly]
#endif
        [SerializeField, Tooltip("创建聊天完成时的 Unix 时间戳（以秒为单位）。")]
        private int created;

        /// <summary>
        /// 创建聊天完成时的 Unix 时间戳（以秒为单位）。
        /// </summary>

        public int Created => created;

        #endregion

        #region Model

#if ODIN_INSPECTOR
        [BoxGroup("信息"), ReadOnly]
#endif
        [SerializeField, Tooltip("创建聊天完成时的 Unix 时间戳（以秒为单位）。")]
        private ChatModel model;

        /// <summary>
        /// 创建聊天完成时的 Unix 时间戳（以秒为单位）。
        /// </summary>
        public ChatModel Model => model;

        #endregion

        #region SystemFingerprint

#if ODIN_INSPECTOR
        [BoxGroup("信息"), ReadOnly]
#endif
        [SerializeField, Tooltip("此指纹表示模型运行时使用的后端配置。")]
        private string systemFingerprint;

        /// <summary>
        /// 此指纹表示模型运行时使用的后端配置。
        /// </summary>

        [CanBeNull]
        public string SystemFingerprint => systemFingerprint;

        #endregion

        #region Object

#if ODIN_INSPECTOR
        [BoxGroup("信息"), ReadOnly]
#endif
        [SerializeField, Tooltip("对象的类型, 其值为 chat.completion。")]
        private string @object;

        /// <summary>
        /// 对象的类型, 其值为 chat.completion。
        /// </summary>

        public string Object => @object;

        #endregion

        #region Usage

#if ODIN_INSPECTOR
        [HideLabel, ReadOnly]
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