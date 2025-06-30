using System;
using Newtonsoft.Json;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    [Serializable]
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    public class FimChoice
    {
        [JsonConstructor]
        public FimChoice(FinishReasonType? finishReason, int index, string text, FimLogprobs logprobs)
        {
            FinishReason = finishReason ?? FinishReasonType.Null;
            Index = index;
            Text = text;
            Logprobs = logprobs;
        }
#if ODIN_INSPECTOR
        [ShowInInspector]
        [UnityEngine.Tooltip("模型停止生成的原因")]
#endif
        public FinishReasonType FinishReason { get; }

#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public int Index { get; }

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("补全结果")]
        [UnityEngine.Tooltip("模型生成的补全结果"), UnityEngine.TextArea(5, 10)]
#endif
        public string Text { get; private set; }


#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        public FimLogprobs Logprobs { get; }

        public void InjectPromptSuffixAsText(string prompt, string suffix)
        {
            Text = prompt + Text + suffix;
        }
    }
}