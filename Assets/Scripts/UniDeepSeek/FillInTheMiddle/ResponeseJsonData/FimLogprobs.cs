using System;
using Newtonsoft.Json;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    [Serializable]
    public class FimLogprobs
    {
        [JsonConstructor]
        public FimLogprobs(int[] textOffset, float[] tokenLogprobs, string[] tokens, FimLogprobs[] topLogprobs)
        {
            textOffsets = textOffset;
            this.tokenLogprobs = tokenLogprobs;
            this.tokens = tokens;
            _topLogprobs = topLogprobs;
        }

        #region TextOffset

        [UnityEngine.SerializeField] private int[] textOffsets;

        public int[] TextTextOffset => textOffsets;

        #endregion

        #region TokenLogprobs

        [UnityEngine.SerializeField] private float[] tokenLogprobs;

        public float[] TokenLogprobs => tokenLogprobs;

        #endregion

        #region Tokens

        [UnityEngine.SerializeField] private string[] tokens;

        public string[] Tokens => tokens;

        #endregion

        #region TopLogprobs

#if ODIN_INSPECTOR
        [ShowInInspector]
#else
        [UnityEngine.SerializeReference]
#endif
        private FimLogprobs[] _topLogprobs;

        public FimLogprobs[] TopLogprobs => _topLogprobs;

        #endregion
    }
}