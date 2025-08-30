using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
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
            this.finishReason = finishReason ?? FinishReasonType.Null;
            this.index = index;
            this.text = text;
            this.logprobs = logprobs;
        }

        #region FinishReason

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField, Tooltip("模型停止生成的原因")]
        private FinishReasonType finishReason;

        public FinishReasonType FinishReason => finishReason;

        #endregion

        #region Index

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField]
        private int index;

        public int Index => index;

        #endregion

        #region Text

#if ODIN_INSPECTOR
        [LabelText("补全结果"), ReadOnly]
#endif
        [SerializeField, Tooltip("模型生成的补全结果"), TextArea(5, 10)]
        private string text;

        public string Text
        {
            get => text;
            private set => text = value;
        }

        #endregion

#if ODIN_INSPECTOR
        [ShowInInspector, ShowIf("@Logprobs != null"), ReadOnly]
#endif
        [SerializeReference]
        private FimLogprobs logprobs;

        public FimLogprobs Logprobs => logprobs;

        public void InjectPromptSuffixAsText(string prompt, string suffix)
        {
            Text = prompt + Text + suffix;
        }
    }
}