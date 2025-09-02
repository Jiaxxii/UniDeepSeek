namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public interface IInjectionExecuteEvent
    {
        void InjectDispatcher(IContentStreamHandler consumer);

        void InjectDispatcher(IReasoningStreamHandler consumer);
    }
}