using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

namespace Xiyu.UniDeepSeek.Tools
{
    [System.Serializable]
    public sealed class ToolInstance : ISerializeParameters
    {
#if UNITY_EDITOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        public string Type { get; } = "function";

#if UNITY_EDITOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        public FunctionDefine FunctionDefine { get; set; } = new();

        public ParamsStandardError VerifyParams()
        {
            return FunctionDefine.VerifyParams();
        }

        public JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            return new JObject
            {
                { "type", Type },
                { "function", FunctionDefine.FromObjectAsToken(serializer) }
            };
        }
    }
}