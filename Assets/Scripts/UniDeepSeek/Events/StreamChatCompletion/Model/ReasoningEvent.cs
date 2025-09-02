using System;

namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion
{
    public class ReasoningEvent : IStreamCompletionReasoningEvent
        , IReasoningStreamHandler
    {
        public ReasoningEvent(IStreamCompletionEvent parent)
        {
            Parent = parent;
        }

        public IStreamCompletionEvent Parent { get; }


        private Action<ChatCompletion> _onEnter;
        private Action<ChatCompletion> _onExit;
        private Action<ChatCompletion> _onUpdate;

        #region All Actions

        public IStreamCompletionReasoningEvent SetAll(Action<ChatCompletion> action)
        {
            _onEnter = action;
            _onUpdate = action;
            _onExit = action;
            return this;
        }

        public IStreamCompletionReasoningEvent AppendAll(Action<ChatCompletion> action)
        {
            _onEnter += action;
            _onUpdate += action;
            _onExit += action;
            return this;
        }

        public IStreamCompletionReasoningEvent RemoveAllBy(Action<ChatCompletion> action)
        {
            _onEnter -= action;
            _onUpdate -= action;
            _onExit -= action;
            return this;
        }

        #endregion

        #region Enter

        public IStreamCompletionReasoningEvent SetEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter = onEnter;
            return this;
        }

        public IStreamCompletionReasoningEvent AppendEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter += onEnter;
            return this;
        }

        public IStreamCompletionReasoningEvent RemoveEnter(Action<ChatCompletion> onEnter)
        {
            _onEnter -= onEnter;
            return this;
        }

        public IStreamCompletionReasoningEvent RemoveAllEnter()
        {
            _onEnter = null;
            return this;
        }

        public void ReasoningEnter(ChatCompletion completion)
        {
            _onEnter?.Invoke(completion);
        }

        #endregion

        #region Exit

        public IStreamCompletionReasoningEvent SetExit(Action<ChatCompletion> onExit)
        {
            _onExit = onExit;
            return this;
        }

        public IStreamCompletionReasoningEvent AppendExit(Action<ChatCompletion> onExit)
        {
            _onExit += onExit;
            return this;
        }


        public IStreamCompletionReasoningEvent RemoveExit(Action<ChatCompletion> exit)
        {
            _onExit -= exit;
            return this;
        }


        public IStreamCompletionReasoningEvent RemoveAllExit()
        {
            _onExit = null;
            return this;
        }

        public void ReasoningExit(ChatCompletion completion)
        {
            _onExit?.Invoke(completion);
        }

        #endregion

        #region Update

        public IStreamCompletionReasoningEvent SetUpdate(Action<ChatCompletion> onUpdate)
        {
            _onUpdate = onUpdate;
            return this;
        }

        public IStreamCompletionReasoningEvent AppendUpdate(Action<ChatCompletion> onUpdate)
        {
            _onUpdate += onUpdate;
            return this;
        }

        public IStreamCompletionReasoningEvent RemoveUpdate(Action<ChatCompletion> update)
        {
            _onUpdate -= update;
            return this;
        }

        public IStreamCompletionReasoningEvent RemoveAllUpdate()
        {
            _onUpdate = null;
            return this;
        }


        public void ReasoningUpdate(ChatCompletion completion)
        {
            _onUpdate?.Invoke(completion);
        }

        #endregion
    }
}