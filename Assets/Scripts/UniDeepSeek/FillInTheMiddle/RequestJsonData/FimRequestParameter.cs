using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.UniDeepSeek.MessagesType;

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
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    [Serializable]
    public class FimRequestParameter : ISerializeParameters
    {
        /// <summary>
        /// 模型的ID（目前只支持 `deepseek-chat`）
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, PropertySpace(10, SpaceAfter = 20), LabelText("模型"), TabGroup("基本", TextColor = "#82CD86")]
        [Tooltip("模型的ID（目前只支持 `deepseek-chat`）")]
#endif
        public ChatModel Model { get; } = ChatModel.Chat;

        #region ECHO

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), TabGroup("基本", TextColor = "#82CD86")]
#endif
        [SerializeField, Tooltip("在输出中，把 prompt与suffix 的内容也输出出来")]
        private bool echo;

        /// <summary>
        /// 在输出中，把 prompt与suffix 的内容也输出出来。
        /// </summary>
        public bool Echo
        {
            get => echo;
            set => echo = value;
        }

        #endregion

        #region PROMPT

// #if ODIN_INSPECTOR
//         [ShowInInspector, PropertySpace(10, SpaceAfter = 20), LabelText("前缀内容"), TabGroup("基本")]
// #else
//         [SerializeField]
// #endif
//         [Tooltip("被制定内容的前缀。"), TextArea(5, 10)]
//         private string _prompt;

        /// <summary>
        /// 被制定内容的前缀。
        /// </summary>
        public string Prompt { get; set; }

        #endregion

        #region SUFFIX

// #if ODIN_INSPECTOR
//         [ShowInInspector, PropertySpace(10, SpaceAfter = 20), LabelText("后缀内容"), TabGroup("基本")]
// #else
//         [SerializeField]
// #endif
//         [Tooltip("被制定内容的后缀。"), TextArea(5, 10)]
//         private string _suffix;

        /// <summary>
        /// 被制定内容的后缀。
        /// </summary>
        public string Suffix { get; set; }

        #endregion


        #region FREQUENCYPENALTY

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), TabGroup("调节", TextColor = "#61AFEF"), LabelText("重复惩罚度-1")]
#endif
        [SerializeField, Range(-2.0F, 2.0F), Tooltip("介于 -2.0 和 2.0 之间的数字。如果该值为正，那么新 token 会根据其在已有文本中的出现频率受到相应的惩罚，降低模型重复相同内容的可能性。")]
        private float frequencyPenalty;

        /// <summary>
        /// <para>重复惩罚度-1</para>
        /// <para>
        /// 介于 -2.0 和 2.0 之间的数字。如果该值为正，那么新 token 会根据其在已有文本中的出现频率受到相应的惩罚，降低模型重复相同内容的可能性。
        /// </para>
        /// </summary>
        public float FrequencyPenalty
        {
            get => frequencyPenalty;
            set => frequencyPenalty = value;
        }

        #endregion

        #region LOGPROBS

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), TabGroup("调节")]
#endif
        [SerializeField, Range(0, 20), Tooltip("制定输出中包含 logprobs 最可能输出 token 的对数概率，包含采样的 token。例如，如果 logprobs 是 20，API 将返回一个包含 20 个最可能的 token 的列表。" +
                                               "API 将始终返回采样 token 的对数概率，因此响应中可能会有最多 logprobs+1 个元素。logprobs 的最大值是 20。")]
        private int logprobs;

        /// <summary>
        /// <para>调节</para>
        /// <para>
        /// 制定输出中包含 logprobs 最可能输出 token 的对数概率，包含采样的 token。例如，如果 logprobs 是 20，API 将返回一个包含 20 个最可能的 token 的列表。
        /// API 将始终返回采样 token 的对数概率，因此响应中可能会有最多 logprobs+1 个元素。logprobs 的最大值是 20。
        /// </para>
        /// </summary>
        public int Logprobs
        {
            get => logprobs;
            set => logprobs = value;
        }

        #endregion

        #region MAXTOKENS

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), TabGroup("调节"), LabelText("最大 token 数量")]
#endif
        [SerializeField, Range(1, 8192), Tooltip("最大输出 token 数量（不建议取大，尤其是代码生成场景）。")]
        private int maxTokens = 100;

        /// <summary>
        /// <para>最大 token 数量</para>
        /// <para>
        /// 最大输出 token 数量（不建议取大，尤其是代码生成场景）。
        /// </para>
        /// </summary>

        public int MaxTokens
        {
            get => maxTokens;
            set => maxTokens = value;
        }

        #endregion

        #region PRESENCEPENALTY

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), TabGroup("调节"), LabelText("重复惩罚度-2")]
#endif
        [SerializeField, Range(-2, 2), Tooltip("介于 -2.0 和 2.0 之间的数字。如果该值为正，那么新 token 会根据其是否已在已有文本中出现受到相应的惩罚，从而增加模型谈论新主题的可能性。")]
        private float presencePenalty;

        /// <summary>
        /// <para>重复惩罚度-2</para>
        /// <para>介于 -2.0 和 2.0 之间的数字。如果该值为正，那么新 token 会根据其是否已在已有文本中出现受到相应的惩罚，从而增加模型谈论新主题的可能性。</para>
        /// </summary>

        public float PresencePenalty
        {
            get => presencePenalty;
            set => presencePenalty = value;
        }

        #endregion

        #region STOP

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), LabelText("停止词"), TabGroup("格式", TextColor = "#C678DD")]
#endif
        [SerializeField, Tooltip("一个 string 或最多包含 16 个 string 的 list，在遇到这些词时，API 将停止生成更多的 token。")]
        private string[] stop = Array.Empty<string>();

        /// <summary>
        /// <para>停止词</para>
        /// <para>
        /// 一个 string 或最多包含 16 个 string 的 list，在遇到这些词时，API 将停止生成更多的 token。
        /// </para>
        /// </summary>

        [NotNull]
        public HashSet<string> Stop
        {
            get => stop.ToHashSet();
            set => stop = value.ToArray();
        }

        #endregion


        // 调用流式方法将自动开启
        // public bool Stream { get; set; }
        // stream_options : {include_usage: bool}

        #region STREAMINCLUDEDUSAGE

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), TabGroup("流式控制", TextColor = "#56B6C2")]
#endif
        [SerializeField, Tooltip("如果设置为 true，在流式消息最后的 data: [DONE] 之前将会传输一个额外的块。此块上的 usage 字段显示整个请求的 token 使用统计信息，而 choices 字段将始终是一个空数组。所有其他块也将包含一个 usage 字段，但其值为 null。")]
        private bool streamIncludedUsage;

        /// <summary>
        /// <para>流式控制</para>
        /// <para>
        /// 如果设置为 true，在流式消息最后的 data: [DONE] 之前将会传输一个额外的块。此块上的 usage 字段显示整个请求的 token 使用统计信息，而 choices 字段将始终是一个空数组。所有其他块也将包含一个 usage 字段，但其值为 null。
        /// </para>
        /// </summary>

        [JsonIgnore] // 手动控制序列化格式 - 延迟到请求发送前再确定
        public bool StreamIncludedUsage
        {
            get => streamIncludedUsage;
            set => streamIncludedUsage = value;
        }

        #endregion

        #region TEMPERATURE

#if ODIN_INSPECTOR
        [LabelText("温度"), PropertySpace(10, SpaceAfter = 20), TabGroup("调节")]
#endif
        [SerializeField, Range(0F, 2F), Tooltip("采样温度，介于 0 和 2 之间。更高的值，如 0.8，会使输出更随机，而更低的值，如 0.2，会使其更加集中和确定。 我们通常建议可以更改这个值或者更改 top_p，但不建议同时对两者进行修改。")]
        private float temperature = 1F;

        /// <summary>
        /// <para>温度</para>
        /// <para>
        /// 采样温度，介于 0 和 2 之间。更高的值，如 0.8，会使输出更随机，而更低的值，如 0.2，会使其更加集中和确定。 我们通常建议可以更改这个值或者更改 top_p，但不建议同时对两者进行修改。
        /// </para>
        /// </summary>

        public float Temperature
        {
            get => temperature;
            set => temperature = value;
        }

        #endregion

        #region TOP_P

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("top_p"), PropertySpace(10, SpaceAfter = 20), TabGroup("调节")]
#endif
        [SerializeField, Range(0F, 1F), Tooltip("作为调节采样温度的替代方案，模型会考虑前 top_p 概率的 token 的结果。所以 0.1 就意味着只有包括在最高 10% 概率中的 token 会被考虑。 我们通常建议修改这个值或者更改 temperature，但不建议同时对两者进行修改。")]
        private float topP;

        /// <summary>
        /// <para>top_p</para>
        /// <para>
        /// 作为调节采样温度的替代方案，模型会考虑前 top_p 概率的 token 的结果。所以 0.1 就意味着只有包括在最高 10% 概率中的 token 会被考虑。 我们通常建议修改这个值或者更改 temperature，但不建议同时对两者进行修改。
        /// </para>
        /// </summary>

        public float TopP
        {
            get => topP;
            set => topP = value;
        }

        #endregion


        public ParamsStandardError VerifyParams()
        {
            if (string.IsNullOrEmpty(Prompt))
            {
                Debug.LogError("请求参数 Prompt 不能为空。");
                return ParamsStandardError.PromptEmpty;
            }

            FrequencyPenalty = Mathf.Clamp(FrequencyPenalty, -2F, 2F);
            Logprobs = Mathf.Clamp(Logprobs, 0, 20);
            MaxTokens = Mathf.Clamp(MaxTokens, 1, 8192);
            PresencePenalty = Mathf.Clamp(PresencePenalty, -2F, 2F);
            Temperature = Mathf.Clamp(Temperature, 0F, 2F);
            TopP = Mathf.Clamp(TopP, 0F, 1F);

            var outs = Stop.Skip(16).ToArray();
            if (outs.Length > 0)
            {
                Debug.LogWarning($"停止词数量超过 16 个，将只保留前 16 个，其中 [{string.Join(", ", outs)}] 被忽略。");
            }

            return ParamsStandardError.Success;
        }

        public JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            var obj = new JObject
            {
                // 必填项
                { "model", "deepseek-chat" },
                { "prompt", Prompt }
            };

            if (!string.IsNullOrEmpty(Suffix)) obj.Add("suffix", Suffix);
            if (FrequencyPenalty != 0) obj.Add("frequency_penalty", FrequencyPenalty);
            if (Logprobs != 0) obj.Add("logprobs", Logprobs);
            if (MaxTokens != 0) obj.Add("max_tokens", MaxTokens);
            if (PresencePenalty != 0) obj.Add("presence_penalty", PresencePenalty);
            if (Stop.Count > 0) obj.Add("stop", new JArray(Stop.Take(16)));
            if (TopP != 0) obj.Add("top_p", TopP);

            return obj;
        }
    }
}