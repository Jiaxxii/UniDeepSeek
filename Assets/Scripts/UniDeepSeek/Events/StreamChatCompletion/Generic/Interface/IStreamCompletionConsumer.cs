using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic
{
    public interface IStreamCompletionConsumer<TContext> : IInjectionExecuteEvent<TContext>
    {
        UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable, TContext context);
    }
}