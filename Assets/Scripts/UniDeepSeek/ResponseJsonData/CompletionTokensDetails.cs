using System;
using UnityEngine;


#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if RIDER
using JetBrains.Annotations;

#else
using Xiyu.UniDeepSeek.Annotations;
#endif
namespace Xiyu.UniDeepSeek
{
    [Serializable]
    public class CompletionTokensDetails
    {
        public CompletionTokensDetails(int reasoningTokens)
        {
            ReasoningTokens = reasoningTokens;
        }

        #region ReasoningTokens

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField, Tooltip("推理模型所产生的思维链 token 数量。")]
        private int reasoningTokens;

        /// <summary>
        /// 推理模型所产生的思维链 token 数量。
        /// </summary>
        public int ReasoningTokens
        {
            get => reasoningTokens;
            private set => reasoningTokens = value;
        }

        #endregion

        public void Append([CanBeNull] CompletionTokensDetails details)
        {
            if (details == null) return;
            ReasoningTokens += details.ReasoningTokens;
        }
    }
}