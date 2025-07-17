using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

namespace Xiyu.UniDeepSeek.Tools
{
    [System.Serializable]
    public sealed class ToolInstance : ISerializeParameters
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        public string Type { get; } = "function";

        #region FUNCTIONDEFINE

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#else
[UnityEngine.SerializeField]
#endif
        private FunctionDefine _functionDefine = new();

        public FunctionDefine FunctionDefine
        {
            get => _functionDefine;
            set => _functionDefine = value;
        }

        #endregion

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