using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

namespace Xiyu.UniDeepSeek.Tools
{
    [System.Serializable]
    public sealed class FunctionDefine : ISerializeParameters
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.LabelText("方法名称")]
#endif
        public string FunctionName { get; set; }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.LabelText("方法描述")]
        [UnityEngine.TextArea(1, 2)]
#endif
        public string Description { get; set; }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.LabelText("Json参数描述")]
        [UnityEngine.TextArea(5, 10)]
#endif
        public string JsonParameters { get; set; }


        public void ObjectAsJson(object obj, JsonSerializerSettings settings = null)
        {
            JsonParameters = JsonConvert.SerializeObject(obj, settings ?? GeneralSerializeSettings.SampleJsonSerializerSettings);
        }

        public ParamsStandardError VerifyParams()
        {
            try
            {
                return JObject.Parse(JsonParameters).Count > 0 ? ParamsStandardError.Success : ParamsStandardError.JsonMissingProperty;
            }
            catch
            {
                return ParamsStandardError.JsonInvalidFormat;
            }
        }

        public JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            return new JObject
            {
                { "name", FunctionName },
                { "description", Description },
                { "parameters", JToken.Parse(JsonParameters) }
            };
        }
    }
}