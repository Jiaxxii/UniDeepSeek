using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

namespace Xiyu.UniDeepSeek.Samples.Tools
{
    [System.Serializable]
    public class ToolInstance
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ReadOnly]
#endif
        public string type = "function";

        #region FUNCTIONDEFINE

        [UnityEngine.SerializeField] public FunctionDefine functionDefine = new();

        #endregion

        public ParamsStandardError VerifyParams()
        {
            return functionDefine.VerifyParams();
        }

        public JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            return new JObject
            {
                { "type", type },
                { "function", functionDefine.FromObjectAsToken(serializer) }
            };
        }
    }
}