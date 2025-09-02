using System;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public class ContentEvent : IStreamCompletionOnlyContentEvent
        , IContentStreamHandler
    {
        public ContentEvent(IStreamCompletionEvent parent)
        {
            Parent = parent;
        }

        private Action<ChatCompletion> _onEnter;
        private Action<ChatCompletion> _onUpdate;
        private Action<ChatCompletion> _onExit;

        public IStreamCompletionEvent Parent { get; }

        #region Enter

        public IStreamCompletionOnlyContentEvent SetEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter = onEnter;
            return this;
        }

        public IStreamCompletionOnlyContentEvent AppendEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public IStreamCompletionOnlyContentEvent RemoveEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter -= onEnter;
            return this;
        }

        public IStreamCompletionOnlyContentEvent RemoveAllEnter()
        {
            _onEnter = null;
            return this;
        }

        public void ContentEnter(ChatCompletion completion)
        {
            _onEnter?.Invoke(completion);
        }

        #endregion

        #region Exit

        public IStreamCompletionOnlyContentEvent RemoveExit(Action<ChatCompletion> exit)
        {
            _onExit -= exit;
            return this;
        }


        public IStreamCompletionOnlyContentEvent RemoveAllExit()
        {
            _onExit = null;
            return this;
        }

        public IStreamCompletionOnlyContentEvent SetExit(Action<ChatCompletion> onExit)
        {
            _onExit = onExit;
            return this;
        }


        public IStreamCompletionOnlyContentEvent AppendExit(Action<ChatCompletion> onExit)
        {
            _onExit += onExit;
            return this;
        }

        public void ContentExit(ChatCompletion completion)
        {
            _onExit?.Invoke(completion);
        }

        #endregion

        #region Update

        public IStreamCompletionOnlyContentEvent SetUpdate(Action<ChatCompletion> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public IStreamCompletionOnlyContentEvent AppendUpdate(Action<ChatCompletion> onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public IStreamCompletionOnlyContentEvent RemoveUpdate(Action<ChatCompletion> update)
        {
            _onUpdate -= update;
            return this;
        }

        public IStreamCompletionOnlyContentEvent RemoveAllUpdate()
        {
            _onUpdate = null;
            return this;
        }

        public void ContentUpdate(ChatCompletion completion)
        {
            _onUpdate?.Invoke(completion);
        }

        #endregion
    }
}