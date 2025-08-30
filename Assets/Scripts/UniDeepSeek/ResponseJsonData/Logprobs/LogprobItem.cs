using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek
{
    [Serializable]
    public class LogprobItem
    {
        public LogprobItem(string token, float logprob, int[] bytes, TopLogprob[] topLogprobs)
        {
            this.token = token;
            this.logprob = logprob;
            this.bytes = bytes;
            this.topLogprobs = topLogprobs;
        }

        #region Token

#if ODIN_INSPECTOR
        [HorizontalGroup, ReadOnly]
#endif
        [SerializeField, Tooltip("输出的 token。")]
        private string token;

        /// <summary>
        /// 输出的 token。
        /// </summary>
        public string Token => token;

        #endregion

        #region Logprob

#if ODIN_INSPECTOR
        [HorizontalGroup, ReadOnly]
#endif
        [SerializeField, Tooltip("该 token 的对数概率。-9999.0 代表该 token 的输出概率极小，不在 top 20 最可能输出的 token 中。")]
        private float logprob;

        /// <summary>
        /// 该 token 的对数概率。-9999.0 代表该 token 的输出概率极小，不在 top 20 最可能输出的 token 中。
        /// </summary>

        public float Logprob => logprob;

        #endregion

        #region Bytes

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField, Tooltip("一个包含该 token UTF-8 字节表示的整数列表。一般在一个 UTF-8 字符被拆分成多个 token 来表示时有用。如果 token 没有对应的字节表示，则该值为 null。")]
        private int[] bytes;

        /// <summary>
        /// 一个包含该 token UTF-8 字节表示的整数列表。一般在一个 UTF-8 字符被拆分成多个 token 来表示时有用。如果 token 没有对应的字节表示，则该值为 null。
        /// </summary>

        public int[] Bytes => bytes;

        #endregion

        #region TopLogprobs

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField, Tooltip("一个包含在该输出位置上，输出概率 top N 的 token 的列表，以及它们的对数概率。在罕见情况下，返回的 token 数量可能少于请求参数中指定的 top_logprobs 值。")]
        private TopLogprob[] topLogprobs;

        /// <summary>
        /// 一个包含在该输出位置上，输出概率 top N 的 token 的列表，以及它们的对数概率。在罕见情况下，返回的 token 数量可能少于请求参数中指定的 top_logprobs 值。
        /// </summary>

        public TopLogprob[] TopLogprobs => topLogprobs;

        #endregion
    }
}