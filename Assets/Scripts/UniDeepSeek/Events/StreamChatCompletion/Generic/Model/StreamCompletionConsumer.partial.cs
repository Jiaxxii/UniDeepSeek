using System;
using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.Generic
{
    public partial class StreamCompletionConsumer<TContext>
    {
        public static async UniTask ProcessStreamWithReasoningAsync(
            UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable,
            TContext context,
            IContentStreamHandler<TContext> contentExecuteEvent,
            IReasoningStreamHandler<TContext> reasoningEvent)
        {
            switch (reasoningEvent)
            {
                // 验证事件不为空
                case null when contentExecuteEvent == null:
                    throw new NullReferenceException("Reasoning or Content Execute Event is null");
                // 只有内容事件的情况
                case null:
                    await ProcessContentOnlyStreamAsync(asyncEnumerable, context, contentExecuteEvent);
                    return;
            }

            // 处理有推理事件的情况（内容事件可能存在）
            var isThinking = false;
            ChatCompletion last = null;

            await foreach (var completion in asyncEnumerable)
            {
                last = completion;
                var message = completion.GetMessage();
                var hasReasoning = !string.IsNullOrEmpty(message.ReasoningContent);
                var hasContent = !string.IsNullOrEmpty(message.Content);

                // 使用更清晰的条件判断
                if (isThinking)
                {
                    if (hasReasoning && !hasContent)
                    {
                        // 继续思考
                        reasoningEvent.ReasoningUpdate(completion, context);
                    }
                    else if (!hasReasoning && hasContent)
                    {
                        // 思考结束，开始回答
                        isThinking = false;
                        reasoningEvent.ReasoningExit(completion, context);
                        contentExecuteEvent?.ContentEnter(completion, context);
                    }
                }
                else
                {
                    if (hasReasoning && !hasContent)
                    {
                        // 开始思考
                        isThinking = true;
                        reasoningEvent.ReasoningEnter(completion, context);
                    }
                    else if (hasContent)
                    {
                        // 直接回答
                        contentExecuteEvent?.ContentUpdate(completion, context);
                    }
                }
            }

            // 确保在结束时调用退出事件
            if (isThinking)
            {
                reasoningEvent.ReasoningExit(last, context);
            }
            else
            {
                contentExecuteEvent?.ContentExit(last, context);
            }
        }

        public static async UniTask ProcessContentOnlyStreamAsync(
            UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable,
            TContext context,
            IContentStreamHandler<TContext> contentExecuteEvent)
        {
            ChatCompletion last = null;
            ChatCompletion first = null;

            await foreach (var completion in asyncEnumerable)
            {
                last = completion;

                if (first == null)
                {
                    first = completion;
                    contentExecuteEvent.ContentEnter(first, context);
                }

                contentExecuteEvent.ContentUpdate(completion, context);
            }

            contentExecuteEvent.ContentExit(last, context);
        }
    }
}