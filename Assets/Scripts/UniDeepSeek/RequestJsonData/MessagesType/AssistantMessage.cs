using JetBrains.Annotations;

namespace Xiyu.UniDeepSeek.MessagesType
{
    [System.Serializable]
    public class AssistantMessage : Message
    {
        private AssistantMessage()
        {
        }

        public override RoleType Role => RoleType.Assistant;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        public string Name { get; set; }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        public bool Prefix { get; set; }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        public string ReasoningContent { get; set; }


        public override ParamsStandardError VerifyParams()
        {
            var verifyParams = base.VerifyParams();

            if (verifyParams != ParamsStandardError.Success)
            {
                return verifyParams;
            }

            // 赋值为null以避免JSON序列化无意义的空字符串
            if (string.IsNullOrWhiteSpace(Name)) Name = null;

            if (!Prefix && ReasoningContent != null)
            {
                ReasoningContent = null;
            }

            return ParamsStandardError.Success;
        }

        public static AssistantMessage CreateMessage(string content, string name = null)
        {
            return new AssistantMessage { Content = content, Name = name };
        }

        /// <summary>
        /// 创建带有前缀的消息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssistantMessage CreatePrefixMessage(string content, string name = null)
        {
            return new AssistantMessage { Content = content, Name = name, Prefix = true };
        }

        /// <summary>
        /// 创建带有前缀的消息 (深度思考模型)
        /// </summary>
        /// <param name="reasoningContent"></param>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssistantMessage CreateReasoningPrefixMessage(string reasoningContent, [NotNull] string content = "", string name = null)
        {
            return new AssistantMessage { Content = content, Name = name, Prefix = true, ReasoningContent = reasoningContent };
        }
    }
}