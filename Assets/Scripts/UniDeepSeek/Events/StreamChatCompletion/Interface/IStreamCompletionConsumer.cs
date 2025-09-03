using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events
{
    public interface IStreamCompletionConsumer : IInjectionExecuteEvent
    {
        UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable);
    }
}