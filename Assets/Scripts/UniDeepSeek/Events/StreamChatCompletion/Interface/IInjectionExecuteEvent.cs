namespace Xiyu.UniDeepSeek.Events
{
    public interface IInjectionExecuteEvent
    {
        void InjectDispatcher(IContentStreamHandler consumer);

        void InjectDispatcher(IReasoningStreamHandler consumer);
    }
}