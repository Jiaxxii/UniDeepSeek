using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.ChatCompletionGeneric
{
    public partial class ChatCompletionEvent<TContext>
    {
        private class DisplayChatStreamProcessor : IChatCompletionRunning<TContext>
        {
            private readonly IChatCompletionInvoke<TContext> _contentEventInvoke;
            private readonly IChatCompletionInvoke<TContext> _reasoningEventInvoke;

            internal DisplayChatStreamProcessor(IChatCompletionInvoke<TContext> contentEventInvoke, IChatCompletionInvoke<TContext> reasoningEventInvoke)
            {
                _contentEventInvoke = contentEventInvoke;
                _reasoningEventInvoke = reasoningEventInvoke;
            }

            public async UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable, TContext context)
            {
                var isThinking = false;

                ChatCompletion last = null;
                await foreach (var completion in asyncEnumerable)
                {
                    last = completion;
                    var message = completion.GetMessage();
                    var hasReasoning = !string.IsNullOrEmpty(message.ReasoningContent);
                    var hasContent = !string.IsNullOrEmpty(message.Content);

                    // 处理思考阶段的开始
                    if (!isThinking && hasReasoning && !hasContent)
                    {
                        isThinking = true;
                        _reasoningEventInvoke.InvokeEnterEvent(completion, context);
                    }
                    // 处理思考阶段的继续
                    else if (isThinking && hasReasoning && !hasContent)
                    {
                        _reasoningEventInvoke.InvokeUpdateEvent(completion, context);
                    }
                    // 处理思考结束，开始回答
                    else if (isThinking && !hasReasoning && hasContent)
                    {
                        isThinking = false;
                        _reasoningEventInvoke.InvokeExitEvent(completion, context);
                        _contentEventInvoke.InvokeEnterEvent(completion, context);
                    }
                    // 处理直接回答（无思考阶段）
                    else if (!isThinking && hasContent)
                    {
                        _contentEventInvoke.InvokeUpdateEvent(completion, context);
                    }
                }

                _contentEventInvoke.InvokeExitEvent(last, context);
            }
        }
    }
}