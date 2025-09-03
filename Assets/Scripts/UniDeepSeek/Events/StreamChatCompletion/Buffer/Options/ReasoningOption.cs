using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Xiyu.UniDeepSeek.Events.Buffer
{
    [Serializable]
    public class ReasoningOption : ContentOption
    {
#if ODIN_INSPECTOR
        [BoxGroup("reasoning", LabelText = "仅深度思考"), LabelText("忽略颜色")]
#endif
        [SerializeField]
        private bool ignoreColor = true;

#if ODIN_INSPECTOR
        [ShowIf("@!ignoreColor"), BoxGroup("reasoning"), LabelText("深度思考颜色")]
#endif

        [SerializeField]
        private Color thickColor = Color.white;


        public string ColorHex
        {
            get => ignoreColor ? null : '#' + ColorUtility.ToHtmlStringRGB(thickColor);
            set
            {
                if (!ColorUtility.TryParseHtmlString(value, out thickColor))
                {
                    Debug.LogWarning("Invalid color hex code: " + value);
                }
            }
        }

#if ODIN_INSPECTOR
        [BoxGroup("reasoning", LabelText = "仅深度思考"), LabelText("深度思考后换行数"), MinValue(0)]
#endif
        [SerializeField]
        private int reasoningNewlineCount = 1;

        public int ReasoningNewlineCount
        {
            get => reasoningNewlineCount;
            set => reasoningNewlineCount = value;
        }
    }
}