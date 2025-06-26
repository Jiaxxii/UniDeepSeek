using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Xiyu.UniDeepSeek.Tools;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek
{
    [Serializable]
    public partial class ChatCompletion
    {
        public ChatCompletion(List<Choice> choices, string id, int created, ChatModel model, [CanBeNull] string systemFingerprint, string @object, [CanBeNull] Usage usage)
        {
            ID = id;
            Created = created;
            Model = model;
            SystemFingerprint = systemFingerprint;
            Object = @object;
            Usage = usage;
            Choices = choices;
        }

        /// <summary>
        /// 模型生成的 completion 的选择列表。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("对话结果"), BoxGroup("信息")]
        [UnityEngine.Tooltip("模型生成的 completion 的选择列表。")]
#endif
        public List<Choice> Choices { get; }


        /// <summary>
        /// 对话的唯一标识符。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息")]
        [UnityEngine.Tooltip("对话的唯一标识符。")]
#endif
        public string ID { get; }

        /// <summary>
        /// 创建聊天完成时的 Unix 时间戳（以秒为单位）。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息")]
        [UnityEngine.Tooltip("创建聊天完成时的 Unix 时间戳（以秒为单位）。")]
#endif
        public int Created { get; }

        /// <summary>
        /// 创建聊天完成时的 Unix 时间戳（以秒为单位）。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息")]
        [UnityEngine.Tooltip("创建聊天完成时的 Unix 时间戳（以秒为单位）。")]
#endif
        public ChatModel Model { get; }

        /// <summary>
        /// 此指纹表示模型运行时使用的后端配置。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息")]
        [UnityEngine.Tooltip("此指纹表示模型运行时使用的后端配置。")]
#endif
        [CanBeNull]
        public string SystemFingerprint { get; }

        /// <summary>
        /// 对象的类型, 其值为 chat.completion。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, BoxGroup("信息")]
        [UnityEngine.Tooltip("对象的类型, 其值为 chat.completion。")]
#endif
        public string Object { get; }

        /// <summary>
        /// 该对话补全请求的用量信息。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, HideLabel]
        [UnityEngine.Tooltip("该对话补全请求的用量信息。")]
#endif
        [CanBeNull]
        public Usage Usage { get; }
    }
}