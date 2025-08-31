namespace Xiyu.UniDeepSeek.Events.ChatCompletionGeneric
{
    public interface IChatCompletionInvoke<in TContext>
    {
        void InvokeEnterEvent(ChatCompletion chatCompletion, TContext context);
        void InvokeUpdateEvent(ChatCompletion chatCompletion, TContext context);
        void InvokeExitEvent(ChatCompletion chatCompletion, TContext context);
    }
}