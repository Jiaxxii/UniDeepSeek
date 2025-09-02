using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public interface IStreamCompletionConsumer : IInjectionExecuteEvent
    {
        UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable);
    }
}