using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek
{
    [Serializable]
    public class Logprobs
    {
        public Logprobs(LogprobItem[] content)
        {
            _content = content;
        }

        public LogprobItem this[int index] => Content[index];

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("choice 的对数概率信息"), ReadOnly]
#else
        [UnityEngine.SerializeField]
#endif
        private LogprobItem[] _content;


        public LogprobItem[] Content => _content;
    }
}