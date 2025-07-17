using System;
using Newtonsoft.Json;
using UnityEngine;
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
            _finishReason = finishReason ?? FinishReasonType.Null;
            _index = index;
            _text = text;
            _logprobs = logprobs;
        }

        #region FinishReason

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("模型停止生成的原因")]
        private FinishReasonType _finishReason;

        public FinishReasonType FinishReason => _finishReason;

        #endregion

        #region Index

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#else
        [SerializeField]
#endif
        private int _index;

        public int Index => _index;

        #endregion

        #region Text

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("补全结果"), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("模型生成的补全结果"), TextArea(5, 10)]
        private string _text;

        public string Text
        {
            get => _text;
            private set => _text = value;
        }

        #endregion

#if ODIN_INSPECTOR
        [ShowInInspector, ShowIf("@Logprobs != null"), ReadOnly]
#else
        [SerializeField]
#endif
        private FimLogprobs _logprobs;

        public FimLogprobs Logprobs => _logprobs;

        public void InjectPromptSuffixAsText(string prompt, string suffix)
        {
            Text = prompt + Text + suffix;
        }
    }
}