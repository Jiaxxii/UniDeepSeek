using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.ChatCompletionGeneric
{
    public interface IChatCompletionRunning<in T>
    {
        UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable, T context);
    }
}