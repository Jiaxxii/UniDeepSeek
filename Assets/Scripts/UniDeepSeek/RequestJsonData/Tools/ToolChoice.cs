using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

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
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.EnumToggleButtons]
#else
[UnityEngine.SerializeField]
#endif

        private FunctionCallModel _functionCallModel = FunctionCallModel.None;

        public FunctionCallModel FunctionCallModel
        {
            get => _functionCallModel;
            set => _functionCallModel = value;
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
        [Sirenix.OdinInspector.ShowInInspector] [Sirenix.OdinInspector.ShowIf("FunctionCallModel", FunctionCallModel.Function)]
#else
[UnityEngine.SerializeField]
#endif
        private string _functionName;

        public string FunctionName
        {
            get => _functionName;
            set => _functionName = value;
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