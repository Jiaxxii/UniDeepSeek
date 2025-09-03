using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.Generic
{
    public interface IStreamCompletionConsumer<TContext> : IInjectionExecuteEvent<TContext>
    {
        UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable, TContext context);
    }
}