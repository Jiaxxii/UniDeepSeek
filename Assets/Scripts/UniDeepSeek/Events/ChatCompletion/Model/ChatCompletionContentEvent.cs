using System;
using Cysharp.Threading.Tasks;

namespace Xiyu.UniDeepSeek.Events
{
    public class ChatCompletionContentEvent : IChatCompletionContentEvent, IChatCompletionInvoke
    {
        public ChatCompletionContentEvent(IChatCompletionEvent parent)
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

        public IChatCompletionContentEvent SetEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter = onEnter;
            return this;
        }

        public IChatCompletionContentEvent AppendEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public IChatCompletionContentEvent SetExit(Action<ChatCompletion> onExit)
        {
            _onExit = onExit;
            return this;
        }

        public IChatCompletionContentEvent AppendExit(Action<ChatCompletion> onExit)
        {
            _onExit += onExit;
            return this;
        }

        public IChatCompletionContentEvent RemoveEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter -= onEnter;
            return this;
        }

        public IChatCompletionContentEvent RemoveExit(Action<ChatCompletion> exit)
        {
            _onExit -= exit;
            return this;
        }

        public IChatCompletionContentEvent RemoveAllEnter()
        {
            _onEnter = null;
            return this;
        }

        public IChatCompletionContentEvent RemoveAllExit()
        {
            _onExit = null;
            return this;
        }

        public IChatCompletionContentEvent SetUpdate(Action<ChatCompletion> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public IChatCompletionContentEvent AppendUpdate(Action<ChatCompletion> onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public IChatCompletionContentEvent RemoveUpdate(Action<ChatCompletion> update)
        {
            _onUpdate -= update;
            return this;
        }

        public IChatCompletionContentEvent RemoveAllUpdate()
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