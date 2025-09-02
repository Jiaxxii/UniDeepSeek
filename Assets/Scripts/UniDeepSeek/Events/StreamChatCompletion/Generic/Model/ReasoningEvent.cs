using System;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Generic
{
    public class ReasoningEvent<TContext> : IStreamCompletionReasoningEvent<TContext>
        , IReasoningStreamHandler<TContext>
    {
        public ReasoningEvent(IStreamCompletionEvent<TContext> parent)
        {
            Parent = parent;
        }

        private Action<ChatCompletion, TContext> _onEnter;
        private Action<ChatCompletion, TContext> _onUpdate;
        private Action<ChatCompletion, TContext> _onExit;

        public IStreamCompletionEvent<TContext> Parent { get; }

        #region All Actions

        public IStreamCompletionReasoningEvent<TContext> SetAll(Action<ChatCompletion, TContext> action)
        {
            _onEnter = action;
            _onUpdate = action;
            _onExit = action;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> AppendAll(Action<ChatCompletion, TContext> action)
        {
            _onEnter += action;
            _onUpdate += action;
            _onExit += action;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> RemoveAllBy(Action<ChatCompletion, TContext> action)
        {
            _onEnter -= action;
            _onUpdate -= action;
            _onExit -= action;
            return this;
        }

        #endregion

        #region Enter

        public IStreamCompletionReasoningEvent<TContext> SetEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter = onEnter;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> AppendEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> RemoveEnter(Action<ChatCompletion, TContext> onEnter)
        {
            _onEnter -= onEnter;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> RemoveAllEnter()
        {
            _onEnter = null;
            return this;
        }

        public void ReasoningEnter(ChatCompletion completion, TContext context)
        {
            _onEnter?.Invoke(completion, context);
        }

        #endregion

        #region Exit

        public IStreamCompletionReasoningEvent<TContext> RemoveExit(Action<ChatCompletion, TContext> exit)
        {
            _onExit -= exit;
            return this;
        }


        public IStreamCompletionReasoningEvent<TContext> RemoveAllExit()
        {
            _onExit = null;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> SetExit(Action<ChatCompletion, TContext> onExit)
        {
            _onExit = onExit;
            return this;
        }


        public IStreamCompletionReasoningEvent<TContext> AppendExit(Action<ChatCompletion, TContext> onExit)
        {
            _onExit += onExit;
            return this;
        }

        public void ReasoningExit(ChatCompletion completion, TContext context)
        {
            _onExit?.Invoke(completion, context);
        }

        #endregion

        #region Update

        public IStreamCompletionReasoningEvent<TContext> SetUpdate(Action<ChatCompletion, TContext> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> AppendUpdate(Action<ChatCompletion, TContext> onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> RemoveUpdate(Action<ChatCompletion, TContext> update)
        {
            _onUpdate -= update;
            return this;
        }

        public IStreamCompletionReasoningEvent<TContext> RemoveAllUpdate()
        {
            _onUpdate = null;
            return this;
        }

        public void ReasoningUpdate(ChatCompletion chatCompletion, TContext context)
        {
            _onUpdate?.Invoke(chatCompletion, context);
        }

        #endregion
    }
}