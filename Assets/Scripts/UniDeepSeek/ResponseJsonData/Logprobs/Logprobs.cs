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
            this.content = content;
        }

        public LogprobItem this[int index] => Content[index];

#if ODIN_INSPECTOR
        [LabelText("choice 的对数概率信息"), ReadOnly]
#endif
        [UnityEngine.SerializeField]
        private LogprobItem[] content;


        public LogprobItem[] Content => content;
    }
}