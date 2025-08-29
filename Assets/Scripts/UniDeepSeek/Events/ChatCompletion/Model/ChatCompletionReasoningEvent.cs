using System;
using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events
{
    public class ChatCompletionReasoningEvent : IChatCompletionReasoningEvent, IChatCompletionInvoke
    {
        public ChatCompletionReasoningEvent(IChatCompletionEvent parent)
        {
            Parent = parent;
        }

        public IChatCompletionEvent Parent { get; }

        private Action<ChatCompletion> _onEnter;
        private Action<ChatCompletion> _onUpdate;
        private Action<ChatCompletion> _onExit;

        public UniTask DisplayChatStreamAsync(UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable)
        {
            return Parent.DisplayChatStreamAsync(asyncEnumerable);
        }

        public IChatCompletionReasoningEvent SetEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter = onEnter;
            return this;
        }

        public IChatCompletionReasoningEvent AppendEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public IChatCompletionReasoningEvent SetExit(Action<ChatCompletion> onExit)
        {
            _onExit = onExit;
            return this;
        }

        public IChatCompletionReasoningEvent AppendExit(Action<ChatCompletion> onExit)
        {
            _onExit += onExit;
            return this;
        }

        public IChatCompletionReasoningEvent RemoveEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter -= onEnter;
            return this;
        }

        public IChatCompletionReasoningEvent RemoveExit(Action<ChatCompletion> exit)
        {
            _onExit -= exit;
            return this;
        }

        public IChatCompletionReasoningEvent RemoveAllEnter()
        {
            _onEnter = null;
            return this;
        }

        public IChatCompletionReasoningEvent RemoveAllExit()
        {
            _onExit = null;
            return this;
        }

        public IChatCompletionReasoningEvent SetUpdate(Action<ChatCompletion> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public IChatCompletionReasoningEvent AppendUpdate(Action<ChatCompletion> onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public IChatCompletionReasoningEvent RemoveUpdate(Action<ChatCompletion> update)
        {
            _onUpdate -= update;
            return this;
        }

        public IChatCompletionReasoningEvent RemoveAllUpdate()
        {
            _onUpdate = null;
            return this;
        }

        public void InvokeEnterEvent(ChatCompletion chatCompletion)
        {
            _onEnter?.Invoke(chatCompletion);
        }

        public void InvokeUpdateEvent(ChatCompletion chatCompletion)
        {
            _onUpdate?.Invoke(chatCompletion);
        }

        public void InvokeExitEvent(ChatCompletion chatCompletion)
        {
            _onExit?.Invoke(chatCompletion);
        }
        
        public override string ToString()
        {
            return $"count-(enter:{_onEnter.GetInvocationList().Length},exit:{_onExit.GetInvocationList().Length},update:{_onUpdate.GetInvocationList().Length})";
        }
    }
}