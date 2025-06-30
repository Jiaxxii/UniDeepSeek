using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
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
            Id = id;
            Created = created;
            Model = model;
            SystemFingerprint = systemFingerprint;
            Object = @object;
            Choices = choices;
            Usage = usage;
        }

#if ODIN_INSPECTOR
        [ShowInInspector]
        [UnityEngine.Tooltip("补全响应的ID")]
#endif
        public string Id { get; }

#if ODIN_INSPECTOR
        [ShowInInspector]
        [UnityEngine.Tooltip("标志补全请求开始时间的 Unix 时间戳（以秒为单位）。")]
#endif
        public int Created { get; }

#if ODIN_INSPECTOR
        [ShowInInspector, PropertySpace(10, SpaceAfter = 20)]
        [UnityEngine.Tooltip("补全请求使用的模型。")]
#endif
        public ChatModel Model { get; }

#if ODIN_INSPECTOR
        [ShowInInspector]
        [UnityEngine.Tooltip("模型运行时的后端配置的指纹。")]
#endif
        public string SystemFingerprint { get; }

#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public string Object { get; }

#if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, InlineProperty]
#endif
        public FimChoice[] Choices { get; }


        /// <summary>
        /// 该对话补全请求的用量信息。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, PropertySpace(10, SpaceAfter = 20)]
        [UnityEngine.Tooltip("该对话补全请求的用量信息。")]
#endif
        [CanBeNull]
        public Usage Usage { get; }
    }
}