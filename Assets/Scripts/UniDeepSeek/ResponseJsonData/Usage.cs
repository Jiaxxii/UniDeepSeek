using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;


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

        /// <summary>
        /// 模型 completion 产生的 token 数量。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("模型 completion 产生的 token 数量。")]
#endif
        public int CompletionTokens { get; private set; }

        /// <summary>
        /// 用户 prompt 所包含的 token 数。该值等于 prompt_cache_hit_tokens + prompt_cache_miss_tokens。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("用户 prompt 所包含的 token 数。该值等于 prompt_cache_hit_tokens + prompt_cache_miss_tokens。")]
#endif
        public int PromptTokens { get; private set; }

        /// <summary>
        /// 用户 prompt 中，命中上下文缓存的 token 数。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("用户 prompt 中，命中上下文缓存的 token 数。")]
#endif
        public int PromptCacheHitTokens { get; private set; }


        /// <summary>
        /// 用户 prompt 中，未命中上下文缓存的 token 数。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("用户 prompt 中，未命中上下文缓存的 token 数。")]
#endif
        public int PromptCacheMissTokens { get; private set; }

        /// <summary>
        /// 该请求中，所有的 token 数量（prompt + completion）。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("该请求中，所有的 token 数量（prompt + completion）。")]
#endif
        public int TotalTokens { get; private set; }

        /// <summary>
        /// completion tokens 的详细信息。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, InlineProperty, BoxGroup("Usage"), HideLabel, ShowIf("@this.CompletionTokensDetails!= null")]
        [UnityEngine.Tooltip("completion tokens 的详细信息。")]
#endif
        public CompletionTokensDetails CompletionTokensDetails { get; private set; }


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

        /// <summary>
        /// 推理模型所产生的思维链 token 数量。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
        [UnityEngine.Tooltip("推理模型所产生的思维链 token 数量。")]
#endif
        public int ReasoningTokens { get; private set; }

        public void Append([CanBeNull] CompletionTokensDetails details)
        {
            if (details == null) return;
            ReasoningTokens += details.ReasoningTokens;
        }
    }
}