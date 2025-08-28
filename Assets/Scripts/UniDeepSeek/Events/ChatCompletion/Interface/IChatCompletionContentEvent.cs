namespace Xiyu.UniDeepSeek.Events
{
    public interface IChatCompletionContentEvent : IEventLifecycleHooks<IChatCompletionContentEvent>, IChatCompletionRunning
    {
        /// <summary>
        /// 返回父级以配置其他事件
        /// </summary>
        IChatCompletionEvent Parent { get; }
    }
}