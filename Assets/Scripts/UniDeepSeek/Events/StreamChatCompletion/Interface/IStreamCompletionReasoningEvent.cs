using System;

namespace Xiyu.UniDeepSeek.Events
{
    public interface IStreamCompletionReasoningEvent :
        IEventLifecycleHooks<IStreamCompletionReasoningEvent>
    {
        IStreamCompletionEvent Parent { get; }


        IStreamCompletionReasoningEvent SetAll(Action<ChatCompletion> action);
        IStreamCompletionReasoningEvent AppendAll(Action<ChatCompletion> action);
        IStreamCompletionReasoningEvent RemoveAllBy(Action<ChatCompletion> action);
    }
}