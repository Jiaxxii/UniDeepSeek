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
            Content = content;
        }

        public LogprobItem this[int index] => Content[index];

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("choice 的对数概率信息")]
#endif
        public LogprobItem[] Content { get; }
        
        
        
    }
}