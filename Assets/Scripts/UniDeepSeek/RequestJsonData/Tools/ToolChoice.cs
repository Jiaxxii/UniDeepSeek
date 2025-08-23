using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

#if RIDER
using JetBrains.Annotations;

#else
using Xiyu.UniDeepSeek.Annotations;
#endif

namespace Xiyu.UniDeepSeek.Tools
{
    [System.Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    public sealed class ToolChoice : ISerializeParameters
    {
        #region FUNCTIONCALLMODEL

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.EnumToggleButtons]
#endif
        [UnityEngine.SerializeField]
        private FunctionCallModel functionCallModel = FunctionCallModel.None;

        public FunctionCallModel FunctionCallModel
        {
            get => functionCallModel;
            set => functionCallModel = value;
        }

        #endregion

        #region TYPE

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.ShowIf("FunctionCallModel", FunctionCallModel.Function)]
#endif

        public string Type { get; } = "Function";

        #endregion

        #region FUNCTIONNAME

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("FunctionCallModel", FunctionCallModel.Function)]
#endif
        [UnityEngine.SerializeField]
        private string functionName;

        public string FunctionName
        {
            get => functionName;
            set => functionName = value;
        }

        #endregion

        public ParamsStandardError VerifyParams()
        {
            if (FunctionCallModel == FunctionCallModel.Function && string.IsNullOrEmpty(FunctionName))
            {
                return ParamsStandardError.FunctionNameByNull;
            }

            return ParamsStandardError.Success;
        }


        [CanBeNull]
        public JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            if (FunctionCallModel == FunctionCallModel.Function)
            {
                return new JObject { { "function", new JObject { { "name", FunctionName } } } };
            }

            return FunctionCallModel.ToString().ToLower();
        }
    }
}