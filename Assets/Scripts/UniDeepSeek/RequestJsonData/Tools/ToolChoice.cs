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
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.EnumToggleButtons]
#endif
        public FunctionCallModel FunctionCallModel { get; set; } = FunctionCallModel.None;


#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.ShowIf("FunctionCallModel", FunctionCallModel.Function)]
#endif
        public string Type => "Function";

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.ShowIf("FunctionCallModel", FunctionCallModel.Function)]
#endif
        public string FunctionName { get; set; }

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