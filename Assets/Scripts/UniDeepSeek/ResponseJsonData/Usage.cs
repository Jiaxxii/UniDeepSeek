using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;


#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek
{
    [Serializable]
    public class Usage
    {
        public Usage(int completionTokens, int promptTokens, int promptCacheHitTokens, int promptCacheMissTokens, int totalTokens)
        {
            CompletionTokens = completionTokens;
            PromptTokens = promptTokens;
            PromptCacheHitTokens = promptCacheHitTokens;
            PromptCacheMissTokens = promptCacheMissTokens;
            TotalTokens = totalTokens;
        }

        #region CompletionTokens

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
#else
        [SerializeField]
#endif
        [Tooltip("模型 completion 产生的 token 数量。")]
        private int _completionTokens;

        /// <summary>
        /// 模型 completion 产生的 token 数量。
        /// </summary>

        public int CompletionTokens
        {
            get => _completionTokens;
            private set => _completionTokens = value;
        }

        #endregion

        #region PromptTokens

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
#else
        [SerializeField]
#endif
        [Tooltip("用户 prompt 所包含的 token 数。该值等于 prompt_cache_hit_tokens + prompt_cache_miss_tokens。")]
        private int _promptTokens;

        /// <summary>
        /// 用户 prompt 所包含的 token 数。该值等于 prompt_cache_hit_tokens + prompt_cache_miss_tokens。
        /// </summary>
        public int PromptTokens
        {
            get => _promptTokens;
            private set => _promptTokens = value;
        }

        #endregion

        #region PromptCacheHitTokens

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
#else
        [SerializeField]
#endif
        [Tooltip("用户 prompt 中，命中上下文缓存的 token 数。")]
        private int _promptCacheHitTokens;

        /// <summary>
        /// 用户 prompt 中，命中上下文缓存的 token 数。
        /// </summary>

        public int PromptCacheHitTokens
        {
            get => _promptCacheHitTokens;
            private set => _promptCacheHitTokens = value;
        }

        #endregion

        #region PromptCacheMissTokens

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
#else
        [SerializeField]
#endif
        [Tooltip("用户 prompt 中，未命中上下文缓存的 token 数。")]
        private int _promptCacheMissTokens;

        /// <summary>
        /// 用户 prompt 中，未命中上下文缓存的 token 数。
        /// </summary>
        public int PromptCacheMissTokens
        {
            get => _promptCacheMissTokens;
            private set => _promptCacheMissTokens = value;
        }

        #endregion

        #region TotalTokens

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
#else
        [SerializeField]
#endif
        [Tooltip("该请求中，所有的 token 数量（prompt + completion）。")]
        private int _totalTokens;

        /// <summary>
        /// 该请求中，所有的 token 数量（prompt + completion）。
        /// </summary>
        public int TotalTokens
        {
            get => _totalTokens;
            private set => _totalTokens = value;
        }

        #endregion

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, InlineProperty, BoxGroup("Usage"), HideLabel, ShowIf("@this.CompletionTokensDetails!= null")]
#else
        [SerializeField]
#endif
        [Tooltip("completion tokens 的详细信息。")]
        private CompletionTokensDetails _completionTokensDetails;

        /// <summary>
        /// completion tokens 的详细信息。
        /// </summary>
        public CompletionTokensDetails CompletionTokensDetails
        {
            get => _completionTokensDetails;
            private set => _completionTokensDetails = value;
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
            }
        }
    }

    [Serializable]
    public class CompletionTokensDetails
    {
        public CompletionTokensDetails(int reasoningTokens)
        {
            ReasoningTokens = reasoningTokens;
        }

        #region ReasoningTokens

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("推理模型所产生的思维链 token 数量。")]
        private int _reasoningTokens;

        /// <summary>
        /// 推理模型所产生的思维链 token 数量。
        /// </summary>
        public int ReasoningTokens
        {
            get => _reasoningTokens;
            private set => _reasoningTokens = value;
        }

        #endregion

        public void Append([CanBeNull] CompletionTokensDetails details)
        {
            if (details == null) return;
            ReasoningTokens += details.ReasoningTokens;
        }
    }
}