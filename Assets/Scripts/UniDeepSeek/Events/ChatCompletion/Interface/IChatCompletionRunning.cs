using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events
{
    public interface IChatCompletionRunning
    {
        UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable);
    }
}