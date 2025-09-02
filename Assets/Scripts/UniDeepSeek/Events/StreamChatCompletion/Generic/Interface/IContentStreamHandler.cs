namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic
{
    public interface IContentStreamHandler<in TContext>
    {
        void ContentEnter(ChatCompletion chatCompletion, TContext context);

        void ContentExit(ChatCompletion chatCompletion, TContext context);

        void ContentUpdate(ChatCompletion chatCompletion, TContext context);
    }

    public interface IReasoningStreamHandler<in TContext>
    {
        void ReasoningEnter(ChatCompletion chatCompletion, TContext context);

        void ReasoningExit(ChatCompletion chatCompletion, TContext context);

        void ReasoningUpdate(ChatCompletion chatCompletion, TContext context);
    }
}