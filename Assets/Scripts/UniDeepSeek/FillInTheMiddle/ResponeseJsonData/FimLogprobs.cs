using System;
using Newtonsoft.Json;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    [Serializable]
    public class FimLogprobs
    {
        [JsonConstructor]
        public FimLogprobs(int[] textOffset, float[] tokenLogprobs, string[] tokens, FimLogprobs[] topLogprobs)
        {
            TextOffset = textOffset;
            TokenLogprobs = tokenLogprobs;
            Tokens = tokens;
            TopLogprobs = topLogprobs;
        }
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public int[] TextOffset { get; }
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public float[] TokenLogprobs { get; }
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public string[] Tokens { get; }
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public FimLogprobs[] TopLogprobs { get; }
    }
}