namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic
{
    public interface IInjectionExecuteEvent<out TContext>
    {
        void InjectDispatcher(IContentStreamHandler<TContext> consumer);

        void InjectDispatcher(IReasoningStreamHandler<TContext> consumer);
    }
}