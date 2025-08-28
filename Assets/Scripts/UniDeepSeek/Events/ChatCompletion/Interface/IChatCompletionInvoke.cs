namespace Xiyu.UniDeepSeek.Events
{
    public interface IChatCompletionInvoke
    {
        void InvokeEnterEvent(ChatCompletion chatCompletion);
        void InvokeUpdateEvent(ChatCompletion chatCompletion);
        void InvokeExitEvent(ChatCompletion chatCompletion);
    }
}