namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public interface IContentStreamHandler
    {
        void ContentEnter(ChatCompletion chatCompletion);

        void ContentExit(ChatCompletion chatCompletion);

        void ContentUpdate(ChatCompletion chatCompletion);
    }

    public interface IReasoningStreamHandler
    {
        void ReasoningEnter(ChatCompletion chatCompletion);

        void ReasoningExit(ChatCompletion chatCompletion);

        void ReasoningUpdate(ChatCompletion chatCompletion);
    }
}