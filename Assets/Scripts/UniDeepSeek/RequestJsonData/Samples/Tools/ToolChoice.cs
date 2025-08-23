using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

#if RIDER
using JetBrains.Annotations;

#else
using Xiyu.UniDeepSeek.Annotations;
#endif

namespace Xiyu.UniDeepSeek.Samples.Tools
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

        public FunctionCallModel functionCallModel = FunctionCallModel.None;

        #endregion

        #region TYPE

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("functionCallModel", FunctionCallModel.Function)]
#endif
        public string type = "function";

        #endregion

        #region FUNCTIONNAME

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("functionCallModel", FunctionCallModel.Function)]
#endif
        public string functionName;

        #endregion


        public ParamsStandardError VerifyParams()
        {
            if (functionCallModel == FunctionCallModel.Function && string.IsNullOrEmpty(functionName))
            {
                return ParamsStandardError.FunctionNameByNull;
            }

            return ParamsStandardError.Success;
        }


        [CanBeNull]
        public JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            if (functionCallModel == FunctionCallModel.Function)
            {
                return new JObject { { "function", new JObject { { "name", functionName } } } };
            }

            return functionCallModel.ToString().ToLower();
        }
    }
}