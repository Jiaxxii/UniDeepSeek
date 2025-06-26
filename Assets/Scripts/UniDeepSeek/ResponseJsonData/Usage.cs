using System;


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
        public int CompletionTokens { get; }

        /// <summary>
        /// 用户 prompt 所包含的 token 数。该值等于 prompt_cache_hit_tokens + prompt_cache_miss_tokens。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("用户 prompt 所包含的 token 数。该值等于 prompt_cache_hit_tokens + prompt_cache_miss_tokens。")]
#endif
        public int PromptTokens { get; }

        /// <summary>
        /// 用户 prompt 中，命中上下文缓存的 token 数。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("用户 prompt 中，命中上下文缓存的 token 数。")]
#endif
        public int PromptCacheHitTokens { get; }


        /// <summary>
        /// 用户 prompt 中，未命中上下文缓存的 token 数。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("用户 prompt 中，未命中上下文缓存的 token 数。")]
#endif
        public int PromptCacheMissTokens { get; }

        /// <summary>
        /// 该请求中，所有的 token 数量（prompt + completion）。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, BoxGroup("Usage")]
        [UnityEngine.Tooltip("该请求中，所有的 token 数量（prompt + completion）。")]
#endif
        public int TotalTokens { get; }

        /// <summary>
        /// completion tokens 的详细信息。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly, InlineProperty, BoxGroup("Usage"), HideLabel, ShowIf("@this.CompletionTokensDetails!= null")]
        [UnityEngine.Tooltip("completion tokens 的详细信息。")]
#endif
        public CompletionTokensDetails CompletionTokensDetails { get; }
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
        public int ReasoningTokens { get; }
    }
}