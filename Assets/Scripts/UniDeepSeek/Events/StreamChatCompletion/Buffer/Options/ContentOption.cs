using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Xiyu.UniDeepSeek.Events.Buffer
{
    [Serializable]
    public class ContentOption
    {
#if ODIN_INSPECTOR
        [BoxGroup("universal", LabelText = "通用"), LabelText("刷新阔值"), MinValue(0)]
#endif
        [SerializeField]
        private int flushInterval = 10;

        public int FlushThreshold
        {
            get => flushInterval;
            set => flushInterval = value;
        }

#if ODIN_INSPECTOR
        [BoxGroup("universal"), LabelText("刷新条件"), EnumToggleButtons]
#endif
        [SerializeField]
        private ContentFlushCriteriaOption flushCriteriaOption = ContentFlushCriteriaOption.ByCharacterCount;

        public ContentFlushCriteriaOption FlushCriteriaOption
        {
            get => flushCriteriaOption;
            set => flushCriteriaOption = value;
        }
    }
}