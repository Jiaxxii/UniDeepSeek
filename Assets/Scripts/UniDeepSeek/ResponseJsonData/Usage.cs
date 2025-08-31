using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek
{
    [Serializable]
    public class Usage
    {
        public Usage(int completionTokens, int promptTokens, int promptCacheHitTokens, int promptCacheMissTokens, int totalTokens,
            CompletionTokensDetails completionTokensDetails, PromptTokensDetails promptTokensDetails)
        {
            CompletionTokens = completionTokens;
            PromptTokens = promptTokens;
            PromptCacheHitTokens = promptCacheHitTokens;
            PromptCacheMissTokens = promptCacheMissTokens;
            TotalTokens = totalTokens;
            CompletionTokensDetails = completionTokensDetails;
            PromptTokensDetails = promptTokensDetails;
        }

        #region CompletionTokens

#if ODIN_INSPECTOR
        [ReadOnly, BoxGroup("Usage")]
#endif
        [SerializeField, Tooltip("模型 completion 产生的 token 数量。")]
        private int completionTokens;

        /// <summary>
        /// 模型 completion 产生的 token 数量。
        /// </summary>

        public int CompletionTokens
        {
            get => completionTokens;
            private set => completionTokens = value;
        }

        #endregion

        #region PromptTokens

#if ODIN_INSPECTOR
        [ReadOnly, BoxGroup("Usage")]
#endif
        [SerializeField, Tooltip("用户 prompt 所包含的 token 数。该值等于 prompt_cache_hit_tokens + prompt_cache_miss_tokens。")]
        private int promptTokens;

        /// <summary>
        /// 用户 prompt 所包含的 token 数。该值等于 prompt_cache_hit_tokens + prompt_cache_miss_tokens。
        /// </summary>
        public int PromptTokens
        {
            get => promptTokens;
            private set => promptTokens = value;
        }

        #endregion

        #region PromptCacheHitTokens

#if ODIN_INSPECTOR
        [ReadOnly, BoxGroup("Usage")]
#endif
        [SerializeField, Tooltip("用户 prompt 中，命中上下文缓存的 token 数。")]
        private int promptCacheHitTokens;

        /// <summary>
        /// 用户 prompt 中，命中上下文缓存的 token 数。
        /// </summary>

        public int PromptCacheHitTokens
        {
            get => promptCacheHitTokens;
            private set => promptCacheHitTokens = value;
        }

        #endregion

        #region PromptCacheMissTokens

#if ODIN_INSPECTOR
        [ReadOnly, BoxGroup("Usage")]
#endif
        [SerializeField, Tooltip("用户 prompt 中，未命中上下文缓存的 token 数。")]
        private int promptCacheMissTokens;

        /// <summary>
        /// 用户 prompt 中，未命中上下文缓存的 token 数。
        /// </summary>
        public int PromptCacheMissTokens
        {
            get => promptCacheMissTokens;
            private set => promptCacheMissTokens = value;
        }

        #endregion

        #region TotalTokens

#if ODIN_INSPECTOR
        [ReadOnly, BoxGroup("Usage")]
#endif
        [SerializeField, Tooltip("该请求中，所有的 token 数量（prompt + completion）。")]
        private int totalTokens;

        /// <summary>
        /// 该请求中，所有的 token 数量（prompt + completion）。
        /// </summary>
        public int TotalTokens
        {
            get => totalTokens;
            private set => totalTokens = value;
        }

        #endregion

#if ODIN_INSPECTOR
        [ReadOnly, InlineProperty, BoxGroup("Usage"), HideLabel, ShowIf("@this.CompletionTokensDetails!= null")]
#endif
        [SerializeField, Tooltip("completion tokens 的详细信息。")]
        private CompletionTokensDetails completionTokensDetails;

        /// <summary>
        /// completion tokens 的详细信息。
        /// </summary>
        public CompletionTokensDetails CompletionTokensDetails
        {
            get => completionTokensDetails;
            private set => completionTokensDetails = value;
        }

#if ODIN_INSPECTOR
        [ReadOnly, InlineProperty, BoxGroup("Usage"), HideLabel, ShowIf("@this.PromptTokensDetails!= null")]
#endif
        [SerializeField, Tooltip("promptTokens details 的详细信息。")]
        private PromptTokensDetails promptTokensDetails;

        public PromptTokensDetails PromptTokensDetails
        {
            get => promptTokensDetails;
            private set => promptTokensDetails = value;
        }


        public void AppendUsage(IEnumerable<Usage> usages)
        {
            foreach (var usage in usages)
            {
                CompletionTokens += usage.CompletionTokens;
                PromptTokens += usage.PromptTokens;
                PromptCacheHitTokens += usage.PromptCacheHitTokens;
                PromptCacheMissTokens += usage.PromptCacheMissTokens;
                TotalTokens += usage.TotalTokens;
                CompletionTokensDetails?.Append(usage.CompletionTokensDetails);
                PromptTokensDetails?.Append(usage.PromptTokensDetails);
            }
        }
    }
}