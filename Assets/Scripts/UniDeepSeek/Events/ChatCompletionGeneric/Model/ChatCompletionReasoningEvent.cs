using System;
using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events.ChatCompletionGeneric
{
    public class ChatCompletionReasoningEvent<TContext> : IChatCompletionReasoningEvent<TContext>, IChatCompletionInvoke<TContext>
    {
        public ChatCompletionReasoningEvent(IChatCompletionEvent<TContext> parent)
        {
            Parent = parent;
        }

        public IChatCompletionEvent<TContext> Parent { get; }

        private Action<ChatCompletion, TContext> _onEnter;
        private Action<ChatCompletion, TContext> _onUpdate;
        private Action<ChatCompletion, TContext> _onExit;

        public UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable, TContext context)
        {
            return Parent.DisplayChatStreamAsync(asyncEnumerable, context);
        }

        public IChatCompletionReasoningEvent<TContext> SetEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter = onEnter;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> AppendEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> SetExit(Action<ChatCompletion, TContext> onExit)
        {
            _onExit = onExit;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> AppendExit(Action<ChatCompletion, TContext> onExit)
        {
            _onExit += onExit;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> RemoveEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter -= onEnter;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> RemoveExit(Action<ChatCompletion, TContext> exit)
        {
            _onExit -= exit;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> RemoveAllEnter()
        {
            _onEnter = null;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> RemoveAllExit()
        {
            _onExit = null;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> SetUpdate(Action<ChatCompletion, TContext> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> AppendUpdate(Action<ChatCompletion, TContext> onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> RemoveUpdate(Action<ChatCompletion, TContext> update)
        {
            _onUpdate -= update;
            return this;
        }

        public IChatCompletionReasoningEvent<TContext> RemoveAllUpdate()
        {
            _onUpdate = null;
            return this;
        }

        public void InvokeEnterEvent(ChatCompletion chatCompletion, TContext context)
        {
            _onEnter?.Invoke(chatCompletion, context);
        }

        public void InvokeUpdateEvent(ChatCompletion chatCompletion, TContext context)
        {
            _onUpdate?.Invoke(chatCompletion, context);
        }

        public void InvokeExitEvent(ChatCompletion chatCompletion, TContext context)
        {
            _onExit?.Invoke(chatCompletion, context);
        }
    }
}