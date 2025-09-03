using System;

namespace Xiyu.UniDeepSeek.Events.Generic
{
    public interface IEventLifecycleHooks<out T, out TContext>
    {
        T SetEnter(Action<ChatCompletion, TContext> onEnter);
        T AppendEnter(Action<ChatCompletion, TContext> onEnter);

        T SetExit(Action<ChatCompletion, TContext> onExit);
        T AppendExit(Action<ChatCompletion, TContext> onExit);

        T RemoveEnter(Action<ChatCompletion, TContext> onEnter);
        T RemoveExit(Action<ChatCompletion, TContext> exit);

        T RemoveAllEnter();
        T RemoveAllExit();


        T SetUpdate(Action<ChatCompletion, TContext> onUpdate);
        T AppendUpdate(Action<ChatCompletion, TContext> onUpdate);
        T RemoveUpdate(Action<ChatCompletion, TContext> update);
        T RemoveAllUpdate();
    }
}