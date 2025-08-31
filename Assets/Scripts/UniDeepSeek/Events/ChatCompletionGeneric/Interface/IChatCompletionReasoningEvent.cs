namespace Xiyu.UniDeepSeek.Events.ChatCompletionGeneric
{
    public interface IChatCompletionReasoningEvent<TContext> : IEventLifecycleHooks<IChatCompletionReasoningEvent<TContext>, TContext>, IChatCompletionRunning<TContext>
    {
        /// <summary>
        /// 返回父级以配置其他事件
        /// </summary>
        IChatCompletionEvent<TContext> Parent { get; }
    }
}