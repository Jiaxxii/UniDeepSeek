using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.Serialization;
using UnityEngine;
using Xiyu.UniDeepSeek.MessagesType;
using Xiyu.UniDeepSeek.Tools;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek.Samples
{
    [Serializable]
    public class ChatRequestBody : ISerializeParameters
    {
        #region CHAT_MODEL

#if ODIN_INSPECTOR
        [PropertySpace(10, SpaceAfter = 20), LabelText("模型")]
#endif
        public ChatModel model = ChatModel.Chat;

        #endregion

        #region MESSAGES

#if ODIN_INSPECTOR
        [ShowInInspector, OdinSerialize, PropertySpace(10, SpaceAfter = 20), LabelText("消息")]
#endif
        // Unity 没办法序列化抽象类
        public List<MessagesType.Message> Messages;

        #endregion

        #region FREQUENCY_PENALTY

#if ODIN_INSPECTOR
        [LabelText("重复惩罚度（相同内容）"), PropertySpace, TabGroup("调节", TextColor = "#61AFEF"), HideIf("model", ChatModel.Reasoner)]
#endif
        [Range(-2F, 2F), Tooltip("介于 -2.0 和 2.0 之间的数字。如果该值为正，那么新 token 会根据其在已有文本中的出现频率受到相应的惩罚，降低模型重复相同内容的可能性。")]
        public float frequencyPenalty;

        #endregion

        #region MAX_TOKENS

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("最大 token 数量"), PropertySpace, TabGroup("调节")]
#endif

        [Range(1, 8192), Tooltip("介于 1 到 8192 间的整数，限制一次请求中模型生成 completion 的最大 token 数。输入 token 和输出 token 的总长度受模型的上下文长度的限制。")]
        public int maxTokens = 4096;

        #endregion

        #region PRESENCE_PENALTY

#if ODIN_INSPECTOR
        [LabelText("重复惩罚度（新话题）"), PropertySpace, TabGroup("调节"), HideIf("model", ChatModel.Reasoner)]
#endif

        [Range(-2F, 2F), Tooltip("介于 -2.0 和 2.0 之间的数字。如果该值为正，那么新 token 会根据其是否已在已有文本中出现受到相应的惩罚，从而增加模型谈论新主题的可能性。")]
        public float presencePenalty;

        #endregion

        #region RESPONSEFORMAT

#if ODIN_INSPECTOR
        [EnumToggleButtons, LabelText("输出格式"), PropertySpace, TabGroup("格式", TextColor = "#C678DD")]
#endif

        [Tooltip("如果是JSON模式也需要提供JSON格式的示例。")]
        public ResponseFormatType responseFormat = ResponseFormatType.Text;

        #endregion

        #region STOP_TOKENS

#if ODIN_INSPECTOR
        [LabelText("停止词"), TabGroup("格式")]
#endif
        [Tooltip("一个 string 或最多包含 16 个 string 的 list，在遇到这些词时，API 将停止生成更多的 token。")]
        public string[] stop;

        #endregion

        #region STREAM

#if UNITY_EDITOR
        [Obsolete("调用流式方法将自动开启", true)] [JsonIgnore] [HideInInspector]
        public bool stream;
#endif

        #endregion

        #region STREAM_INCLUDED_USAGE

#if ODIN_INSPECTOR
        [PropertySpace, TabGroup("流式控制", TextColor = "#56B6C2")]
#endif
        [Tooltip("如果设置为 true，在流式消息最后的 data: [DONE] 之前将会传输一个额外的块。此块上的 usage 字段显示整个请求的 token 使用统计信息，而 choices 字段将始终是一个空数组。所有其他块也将包含一个 usage 字段，但其值为 null。")]
        public bool streamIncludedUsage;

        #endregion

        #region TEMPERATURE

#if ODIN_INSPECTOR
        [LabelText("温度"), PropertySpace, TabGroup("调节"), HideIf("model", ChatModel.Reasoner)]
#endif
        [Range(0F, 2F), Tooltip("采样温度，介于 0 和 2 之间。更高的值，如 0.8，会使输出更随机，而更低的值，如 0.2，会使其更加集中和确定。 我们通常建议可以更改这个值或者更改 top_p，但不建议同时对两者进行修改。")]
        public float temperature = 1F;

        #endregion

        #region TOP_P

#if ODIN_INSPECTOR
        [LabelText("top_p"), PropertySpace, TabGroup("调节"), HideIf("model", ChatModel.Reasoner)]
#endif

        [Range(0F, 1F), Tooltip("作为调节采样温度的替代方案，模型会考虑前 top_p 概率的 token 的结果。所以 0.1 就意味着只有包括在最高 10% 概率中的 token 会被考虑。 我们通常建议修改这个值或者更改 temperature，但不建议同时对两者进行修改。")]
        public float topP = 1F;

        #endregion

        #region LOGPROBS

#if ODIN_INSPECTOR
        [EnumToggleButtons, LabelText("logprobs"), PropertySpace, TabGroup("调节"), HideIf("model", ChatModel.Reasoner)]
#endif
        [Tooltip("是否返回所输出 token 的对数概率。如果为 true，则在 message 的 content 中返回每个输出 token 的对数概率。")]
        public bool loggers;

        #endregion

        #region TOPLOGPROBS

#if ODIN_INSPECTOR
        [ShowIf("@Logprobs && Model != ChatModel.Reasoner"), LabelText("top_logprobs"), PropertySpace, TabGroup("调节")]
#endif

        [Range(0, 20), Tooltip("一个介于 0 到 20 之间的整数 N，指定每个输出位置返回输出概率 top N 的 token，且返回这些 token 的对数概率。指定此参数时，logprobs 必须为 true。")]
        public int topLogprobs;

        #endregion

        #region TOOLINSTANCES

#if ODIN_INSPECTOR
        [PropertySpace, TabGroup("工具", TextColor = "#E5C07B")]
#endif
        public List<Tools.ToolInstance> toolInstances = new();

        #endregion

        #region TOOLCHOICE

#if ODIN_INSPECTOR
        [LabelText("工具选择"), PropertySpace, TabGroup("工具")]
#endif

        public Tools.ToolChoice toolChoice = new();

        #endregion

        #region As Json

        public ParamsStandardError VerifyParams()
        {
            frequencyPenalty = Mathf.Clamp(frequencyPenalty, -2F, 2F);
            maxTokens = Mathf.Clamp(maxTokens, 1, 8192);
            presencePenalty = Mathf.Clamp(presencePenalty, -2F, 2F);
            topP = Mathf.Clamp(topP, 0F, 1F);
            topLogprobs = Mathf.Clamp(topLogprobs, 0, 20);

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
                    "model", model switch
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


            if (model != ChatModel.Reasoner && frequencyPenalty != 0)
                jObject.Add("frequency_penalty", frequencyPenalty);


            if (maxTokens != 4096)
                jObject.Add("max_tokens", maxTokens);


            if (model != ChatModel.Reasoner && presencePenalty != 0)
                jObject.Add("presence_penalty", presencePenalty);


            if (model != ChatModel.Reasoner && !Mathf.Approximately(temperature, 1))
                jObject.Add("temperature", temperature);


            if (model != ChatModel.Reasoner && !Mathf.Approximately(topP, 1))
                jObject.Add("top_p", topP);

            if (model != ChatModel.Reasoner && loggers)
            {
                jObject.Add("logprobs", loggers);
                jObject.Add("top_logprobs", topLogprobs);
            }

            if (stop is not null && stop.Length > 0)
            {
                if (stop.Length == 1)
                {
                    jObject.Add("stop", stop[0]);
                }
                else
                {
                    var stopArray = new JArray();
                    foreach (var stopWord in stop.Take(16))
                    {
                        stopArray.Add(stopWord);
                    }

                    jObject.Add("stop", stopArray);
                }
            }


            if (responseFormat != ResponseFormatType.Text)
            {
                jObject.Add("response_format", new JObject
                {
                    {
                        "type", responseFormat switch
                        {
                            ResponseFormatType.Text => "text",
                            ResponseFormatType.JsonObject => "json_object",
                            _ => throw new ArgumentOutOfRangeException()
                        }
                    }
                });
            }


            // 没有工具实例 或 没有选择工具
            if (toolInstances is null || toolInstances.Count == 0 ||
                (toolInstances.Count != 0 && toolChoice is not null && toolChoice.functionCallModel == FunctionCallModel.None))
            {
                return jObject;
            }

            if (toolChoice is not null && toolInstances is not null && toolInstances.Count > 0)
            {
                var tools = new JArray();
                foreach (var toolInstance in toolInstances)
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


            if (toolChoice is not null)
            {
                jObject.Add("tool_choice", toolChoice.FromObjectAsToken(serializer));
            }

            return jObject;
        }

        #endregion


        public static implicit operator Xiyu.UniDeepSeek.ChatRequestParameter(ChatRequestBody requestBody)
        {
            return new Xiyu.UniDeepSeek.ChatRequestParameter
            {
                Model = requestBody.model,
                Messages = requestBody.Messages,
                FrequencyPenalty = requestBody.frequencyPenalty,
                MaxTokens = requestBody.maxTokens,
                PresencePenalty = requestBody.presencePenalty,
                Temperature = requestBody.temperature,
                TopP = requestBody.topP,
                Logprobs = requestBody.loggers,
                TopLogprobs = requestBody.topLogprobs,
                Stop = requestBody.stop.ToHashSet(),
                ResponseFormat = requestBody.responseFormat,
                StreamIncludedUsage = requestBody.streamIncludedUsage,
                ToolInstances = requestBody.toolInstances.Select(ins => new UniDeepSeek.Tools.ToolInstance
                {
                    FunctionDefine = new UniDeepSeek.Tools.FunctionDefine
                    {
                        FunctionName = ins.functionDefine.functionName,
                        Description = ins.functionDefine.description,
                        JsonParameters = ins.functionDefine.jsonParameters,
                        RequiredParameters = ins.functionDefine.requiredParameters.ToList(),
                    }
                }).ToList(),
                ToolChoice = new ToolChoice
                {
                    FunctionCallModel = requestBody.toolChoice.functionCallModel,
                    FunctionName = requestBody.toolChoice.functionName
                }
            };
        }
    }
}