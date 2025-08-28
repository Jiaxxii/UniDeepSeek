namespace Xiyu.UniDeepSeek.Events
{
    public interface IChatCompletionReasoningEvent : IEventLifecycleHooks<IChatCompletionReasoningEvent>, IChatCompletionRunning
    {
        /// <summary>
        /// 返回父级以配置其他事件
        /// </summary>
        IChatCompletionEvent Parent { get; }
    }
}