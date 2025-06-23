using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

namespace Xiyu.UniDeepSeek.Tools
{
    [System.Serializable]
#if UNITY_EDITOR
    [Sirenix.OdinInspector.InlineProperty]
#endif
    public sealed class ToolChoice : ISerializeParameters
    {
#if UNITY_EDITOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.EnumToggleButtons]
#endif
        public FunctionCallModel FunctionCallModel { get; set; } = FunctionCallModel.None;


#if UNITY_EDITOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.ShowIf("FunctionCallModel", FunctionCallModel.Required)]
#endif
        public string Type => "Function";

#if UNITY_EDITOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.ShowIf("FunctionCallModel", FunctionCallModel.Required)]
#endif
        public string FunctionName { get; set; }

        public ParamsStandardError VerifyParams()
        {
            if (FunctionCallModel == FunctionCallModel.Required && string.IsNullOrEmpty(FunctionName))
            {
                return ParamsStandardError.FunctionNameByNull;
            }

            return ParamsStandardError.Success;
        }


        [CanBeNull]
        public JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            if (FunctionCallModel == FunctionCallModel.None) return null;

            var obj = new JObject
            {
                { "tool_choice", FunctionCallModel.ToString().ToLower() }
            };

            if (FunctionCallModel == FunctionCallModel.Auto)
            {
                return obj;
            }

            obj.Add("function", new JObject { { "name", FunctionName } });

            return obj;
        }
    }
}