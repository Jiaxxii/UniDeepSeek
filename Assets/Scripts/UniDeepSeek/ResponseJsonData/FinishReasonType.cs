namespace Xiyu.UniDeepSeek
{
    public enum FinishReasonType
    {
        Null,

        /// <summary>
        /// 模型自然停止生成，或遇到 stop 序列中列出的字符串。
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = "stop")]
        Stop,

        /// <summary>
        /// 输出长度达到了模型上下文长度限制，或达到了 max_tokens 的限制。
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = "length")]
        Length,

        /// <summary>
        /// 输出内容因触发过滤策略而被过滤。
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = "content_filter")]
        ContentFilter,


        /// <summary>
        /// 系统推理资源不足，生成被打断。
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = "insufficient_system_resource")]
        InsufficientSystemResource,

        /// <summary>
        /// 工具调用
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = "tool_calls")]
        ToolCalls
    }
}