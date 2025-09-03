using System;

namespace Xiyu.UniDeepSeek.Events.Generic
{
    public class ContentEvent<TContext> : IStreamCompletionOnlyContentEvent<TContext>
        , IContentStreamHandler<TContext>
    {
        public ContentEvent(IStreamCompletionEvent<TContext> parent)
        {
            Parent = parent;
        }

        private Action<ChatCompletion, TContext> _onEnter;
        private Action<ChatCompletion, TContext> _onUpdate;
        private Action<ChatCompletion, TContext> _onExit;

        public IStreamCompletionEvent<TContext> Parent { get; }

        #region Enter

        public IStreamCompletionOnlyContentEvent<TContext> SetEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter = onEnter;
            return this;
        }

        public IStreamCompletionOnlyContentEvent<TContext> AppendEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public IStreamCompletionOnlyContentEvent<TContext> RemoveEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter -= onEnter;
            return this;
        }

        public IStreamCompletionOnlyContentEvent<TContext> RemoveAllEnter()
        {
            _onEnter = null;
            return this;
        }

        public void ContentEnter(ChatCompletion completion, TContext context)
        {
            _onEnter?.Invoke(completion, context);
        }

        #endregion

        #region Exit

        public IStreamCompletionOnlyContentEvent<TContext> RemoveExit(Action<ChatCompletion, TContext> exit)
        {
            _onExit -= exit;
            return this;
        }


        public IStreamCompletionOnlyContentEvent<TContext> RemoveAllExit()
        {
            _onExit = null;
            return this;
        }

        public IStreamCompletionOnlyContentEvent<TContext> SetExit(Action<ChatCompletion, TContext> onExit)
        {
            _onExit = onExit;
            return this;
        }


        public IStreamCompletionOnlyContentEvent<TContext> AppendExit(Action<ChatCompletion, TContext> onExit)
        {
            _onExit += onExit;
            return this;
        }

        public void ContentExit(ChatCompletion completion, TContext context)
        {
            _onExit?.Invoke(completion, context);
        }

        #endregion

        #region Update

        public IStreamCompletionOnlyContentEvent<TContext> SetUpdate(Action<ChatCompletion, TContext> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public IStreamCompletionOnlyContentEvent<TContext> AppendUpdate(Action<ChatCompletion, TContext> onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public IStreamCompletionOnlyContentEvent<TContext> RemoveUpdate(Action<ChatCompletion, TContext> update)
        {
            _onUpdate -= update;
            return this;
        }

        public IStreamCompletionOnlyContentEvent<TContext> RemoveAllUpdate()
        {
            _onUpdate = null;
            return this;
        }

        public void ContentUpdate(ChatCompletion completion, TContext context)
        {
            _onUpdate?.Invoke(completion, context);
        }

        #endregion
    }
}