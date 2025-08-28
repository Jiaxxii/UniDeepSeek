namespace Xiyu.UniDeepSeek.Events
{
    public interface IChatCompletionEvent : IChatCompletionRunning
    {
        /// <summary>
        /// 接收到 `Content` 时触发的事件
        /// </summary>
        IChatCompletionContentEvent ContentEventSetting { get; }

        /// <summary>
        /// 接收到 `ReasoningContent` 时触发的事件
        /// </summary>
        IChatCompletionReasoningEvent ReasoningEventSetting { get; }
    }
}