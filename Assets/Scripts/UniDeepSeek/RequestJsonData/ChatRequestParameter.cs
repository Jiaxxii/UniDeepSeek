using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.Tools;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek
{
    [Serializable]
    public class ChatRequestParameter : ISerializeParameters
    {
#if ODIN_INSPECTOR
        [ShowInInspector, PropertySpace(10, SpaceAfter = 20), LabelText("模型")]
#endif
        public ChatModel Model { get; set; } = ChatModel.Chat;


#if ODIN_INSPECTOR
        [ShowInInspector, PropertySpace(10, SpaceAfter = 20), LabelText("消息")]
#endif
        public List<MessagesType.Message> Messages { get; set; } = new();

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("重复惩罚度（相同内容）"), PropertySpace, TabGroup("调节", TextColor = "#61AFEF"), HideIf("Model", ChatModel.Reasoner)]
        [Range(-2F, 2F), Tooltip("介于 -2.0 和 2.0 之间的数字。如果该值为正，那么新 token 会根据其在已有文本中的出现频率受到相应的惩罚，降低模型重复相同内容的可能性。")]
#endif
        public float FrequencyPenalty { get; set; }


#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("最大 token 数量"), PropertySpace, TabGroup("调节")]
        [Range(1, 8192), Tooltip("介于 1 到 8192 间的整数，限制一次请求中模型生成 completion 的最大 token 数。输入 token 和输出 token 的总长度受模型的上下文长度的限制。")]
#endif
        public int MaxTokens { get; set; } = 4096;


#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("重复惩罚度（新话题）"), PropertySpace, TabGroup("调节"), HideIf("Model", ChatModel.Reasoner)]
        [Range(-2F, 2F), Tooltip("介于 -2.0 和 2.0 之间的数字。如果该值为正，那么新 token 会根据其是否已在已有文本中出现受到相应的惩罚，从而增加模型谈论新主题的可能性。")]
#endif
        public float PresencePenalty { get; set; }


#if ODIN_INSPECTOR
        [ShowInInspector, EnumToggleButtons, LabelText("输出格式"), PropertySpace, TabGroup("格式", TextColor = "#C678DD")]
        [Tooltip("如果是JSON模式也需要提供JSON格式的示例。")]
#endif
        [JsonIgnore]
        public ResponseFormatType ResponseFormat { get; set; } = ResponseFormatType.Text;

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("停止词"), TabGroup("格式")]
        [Tooltip("一个 string 或最多包含 16 个 string 的 list，在遇到这些词时，API 将停止生成更多的 token。")]
#endif
        public HashSet<string> Stop { get; set; } = new();


        // 调用流式方法将自动开启
        // public bool Stream { get; set; }
        // stream_options : {include_usage: bool}

#if ODIN_INSPECTOR
        [ShowInInspector, PropertySpace, TabGroup("流式控制", TextColor = "#56B6C2")]
        [Tooltip("如果设置为 true，在流式消息最后的 data: [DONE] 之前将会传输一个额外的块。此块上的 usage 字段显示整个请求的 token 使用统计信息，而 choices 字段将始终是一个空数组。所有其他块也将包含一个 usage 字段，但其值为 null。")]
#endif
        [JsonIgnore] // 手动控制序列化格式 - 延迟到请求发送前再确定
        public bool StreamIncludedUsage { get; set; }


#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("温度"), PropertySpace, TabGroup("调节"), HideIf("Model", ChatModel.Reasoner)]
        [Range(0F, 2F), Tooltip("采样温度，介于 0 和 2 之间。更高的值，如 0.8，会使输出更随机，而更低的值，如 0.2，会使其更加集中和确定。 我们通常建议可以更改这个值或者更改 top_p，但不建议同时对两者进行修改。")]
#endif
        public float Temperature { get; set; } = 1;

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("top_p"), PropertySpace, TabGroup("调节"), HideIf("Model", ChatModel.Reasoner)]
        [Range(0F, 1F),
         Tooltip("作为调节采样温度的替代方案，模型会考虑前 top_p 概率的 token 的结果。所以 0.1 就意味着只有包括在最高 10% 概率中的 token 会被考虑。 我们通常建议修改这个值或者更改 temperature，但不建议同时对两者进行修改。")]
#endif
        public float TopP { get; set; } = 1;

#if ODIN_INSPECTOR
        [ShowInInspector, EnumToggleButtons, LabelText("logprobs"), PropertySpace, TabGroup("调节"), HideIf("Model", ChatModel.Reasoner)]
        [Tooltip("是否返回所输出 token 的对数概率。如果为 true，则在 message 的 content 中返回每个输出 token 的对数概率。")]
#endif
        public bool Logprobs { get; set; }

#if ODIN_INSPECTOR
// 修改点：移除单独的 HideIf，使用组合的 ShowIf 条件
        [ShowInInspector, ShowIf("@Logprobs && Model != ChatModel.Reasoner"), LabelText("top_logprobs"), PropertySpace, TabGroup("调节")]
        [Range(0, 20), Tooltip("一个介于 0 到 20 之间的整数 N，指定每个输出位置返回输出概率 top N 的 token，且返回这些 token 的对数概率。指定此参数时，logprobs 必须为 true。")]
#endif
        [JsonIgnore]
        public int TopLogprobs { get; set; } = 0;

#if ODIN_INSPECTOR
        [ShowInInspector, PropertySpace, TabGroup("工具", TextColor = "#E5C07B")]
#endif
        [JsonIgnore] // 手动控制序列化格式
        public List<ToolInstance> ToolInstances { get; set; } = new();

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("工具选择"), PropertySpace, TabGroup("工具")]
#endif
        [JsonIgnore] // 手动控制序列化格式
        public ToolChoice ToolChoice { get; private set; } = new();


        public ParamsStandardError VerifyParams()
        {
            FrequencyPenalty = Mathf.Clamp(FrequencyPenalty, -2F, 2F);
            MaxTokens = Mathf.Clamp(MaxTokens, 1, 8192);
            PresencePenalty = Mathf.Clamp(PresencePenalty, -2F, 2F);
            TopP = Mathf.Clamp(TopP, 0F, 1F);
            TopLogprobs = Mathf.Clamp(TopLogprobs, 0, 20);

            if (Messages is null || Messages.Count == 0 || Messages.All(m => m.Role != RoleType.User))
            {
                return ParamsStandardError.UserMessagesMissing;
            }


            return ParamsStandardError.Success;
        }

        public JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            var jObject = new JObject
            {
                {
                    "model", Model switch
                    {
                        ChatModel.Chat => "deepseek-chat",
                        ChatModel.Reasoner => "deepseek-reasoner",
                        _ => throw new ArgumentOutOfRangeException()
                    }
                }
            };

            serializer ??= GeneralSerializeSettings.SampleJsonSerializer;

            var messagesArray = new JArray();
            foreach (var message in Messages)
            {
                var verifyParams = message.VerifyParams();
                if (verifyParams != ParamsStandardError.Success)
                {
                    Debug.LogWarningFormat("[<color=red>{0}</color>]参数校验失败 (code: {1})", verifyParams.ToString(), (int)verifyParams);
                }
                else
                {
                    messagesArray.Add(message.FromObjectAsToken(serializer));
                }
            }

            jObject.Add("messages", messagesArray);


            if (Model != ChatModel.Reasoner && FrequencyPenalty != 0)
                jObject.Add("frequency_penalty", FrequencyPenalty);


            if (MaxTokens != 4096)
                jObject.Add("max_tokens", MaxTokens);


            if (Model != ChatModel.Reasoner && PresencePenalty != 0)
                jObject.Add("presence_penalty", PresencePenalty);


            if (Model != ChatModel.Reasoner && !Mathf.Approximately(Temperature, 1))
                jObject.Add("temperature", Temperature);


            if (Model != ChatModel.Reasoner && !Mathf.Approximately(TopP, 1))
                jObject.Add("top_p", TopP);

            if (Model != ChatModel.Reasoner && Logprobs)
            {
                jObject.Add("logprobs", Logprobs);
                jObject.Add("top_logprobs", TopLogprobs);
            }

            if (Stop is not null && Stop.Count > 0)
            {
                if (Stop.Count == 1)
                {
                    jObject.Add("stop", Stop.First());
                }
                else
                {
                    var stopArray = new JArray();
                    foreach (var stopWord in Stop.Take(16))
                    {
                        stopArray.Add(stopWord);
                    }

                    jObject.Add("stop", stopArray);
                }
            }


            if (ResponseFormat != ResponseFormatType.Text)
            {
                jObject.Add("response_format", new JObject
                {
                    {
                        "type", ResponseFormat switch
                        {
                            ResponseFormatType.Text => "text",
                            ResponseFormatType.JsonObject => "json_object",
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    }
                });
            }


            // 没有工具实例 或 没有选择工具
            if (ToolInstances is null || ToolInstances.Count == 0 ||
                (ToolInstances.Count != 0 && ToolChoice is not null && ToolChoice.FunctionCallModel == FunctionCallModel.None))
            {
                return jObject;
            }

            if (ToolChoice is not null && ToolInstances is not null && ToolInstances.Count > 0)
            {
                var tools = new JArray();
                foreach (var toolInstance in ToolInstances)
                {
                    var verifyParams = toolInstance.VerifyParams();
                    if (verifyParams != ParamsStandardError.Success)
                    {
                        Debug.LogWarningFormat("[<color=red>{0}</color>]参数校验失败 (code: {1})", verifyParams.ToString(), (int)verifyParams);
                    }
                    else
                    {
                        tools.Add(toolInstance.FromObjectAsToken(serializer));
                    }
                }

                jObject.Add("tools", tools);
            }


            if (ToolChoice is not null)
            {
                jObject.Add("tool_choice", ToolChoice.FromObjectAsToken(serializer));
            }

            return jObject;
        }
    }
}