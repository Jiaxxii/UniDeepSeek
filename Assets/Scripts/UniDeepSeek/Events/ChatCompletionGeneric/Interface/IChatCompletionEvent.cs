namespace Xiyu.UniDeepSeek.Events.ChatCompletionGeneric
{
    public interface IChatCompletionEvent<TContext> : IChatCompletionRunning<TContext>
    {
        /// <summary>
        /// 接收到 `Content` 时触发的事件
        /// </summary>
        IChatCompletionContentEvent<TContext> ContentEventSetting { get; }

        /// <summary>
        /// 接收到 `ReasoningContent` 时触发的事件
        /// </summary>
        IChatCompletionReasoningEvent<TContext> ReasoningEventSetting { get; }
    }
}