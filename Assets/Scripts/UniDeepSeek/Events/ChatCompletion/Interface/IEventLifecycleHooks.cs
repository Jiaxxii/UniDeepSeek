using System;

namespace Xiyu.UniDeepSeek.Events
{
    public interface IEventLifecycleHooks<out T>
    {
        T SetEnter(Action<ChatCompletion> onEnter);
        T AppendEnter(Action<ChatCompletion> onEnter);

        T SetExit(Action<ChatCompletion> onExit);
        T AppendExit(Action<ChatCompletion> onExit);

        T RemoveEnter(Action<ChatCompletion> onEnter);
        T RemoveExit(Action<ChatCompletion> exit);

        T RemoveAllEnter();
        T RemoveAllExit();


        T SetUpdate(Action<ChatCompletion> onUpdate);
        T AppendUpdate(Action<ChatCompletion> onUpdate);
        T RemoveUpdate(Action<ChatCompletion> update);
        T RemoveAllUpdate();
    }
}