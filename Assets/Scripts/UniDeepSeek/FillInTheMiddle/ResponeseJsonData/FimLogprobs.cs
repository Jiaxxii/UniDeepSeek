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
            _textOffsets = textOffset;
            _tokenLogprobs = tokenLogprobs;
            _tokens = tokens;
            _topLogprobs = topLogprobs;
        }

        #region TextOffset

#if ODIN_INSPECTOR
        [ShowInInspector]
#else
        [UnityEngine.SerializeField]
#endif
        private int[] _textOffsets;

        public int[] TextTextOffset => _textOffsets;

        #endregion

        #region TokenLogprobs

#if ODIN_INSPECTOR
        [ShowInInspector]
#else
        [UnityEngine.SerializeField]
#endif
        private float[] _tokenLogprobs;

        public float[] TokenLogprobs => _tokenLogprobs;

        #endregion

        #region Tokens

#if ODIN_INSPECTOR
        [ShowInInspector]
#else
        [UnityEngine.SerializeField]
#endif
        private string[] _tokens;

        public string[] Tokens => _tokens;

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