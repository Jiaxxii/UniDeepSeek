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
    //
    // 这里定义可能是没必要的，因为官方文档并没有提到 prompt_tokens_details 这个字段。
    // 它只在响应Json中出现，而且它的值都是0.
    //
    [Serializable]
    public class PromptTokensDetails
    {
        public PromptTokensDetails(int cachedTokens)
        {
            CachedTokens = cachedTokens;
        }

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField, Tooltip("在本次请求的提示（Prompt）中，有 0 个 Token 命中了服务器的缓存，因此这些 Token 不需要被模型重新处理。")]
        private int cachedTokens;

        /// <summary>
        /// 在本次请求的提示（Prompt）中，有 0 个 Token 命中了服务器的缓存，因此这些 Token 不需要被模型重新处理。
        /// </summary>
        public int CachedTokens
        {
            get => cachedTokens;
            private set => cachedTokens = value;
        }

        public void Append([CanBeNull] PromptTokensDetails details)
        {
            if (details == null) return;
            CachedTokens += details.CachedTokens;
        }
    }
}