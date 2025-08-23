using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.UniDeepSeek.MessagesType;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek.Samples.Tools
{
    [System.Serializable]
    public class FunctionDefine : ISerializeParameters
    {
        #region FUNCTIONNAME

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("方法名称")]
#endif
        public string functionName;

        #endregion

        #region DESCRIPTION

#if ODIN_INSPECTOR
        [LabelText("方法描述")]
#endif
        [SerializeField, TextArea(1, 2)]
        public string description;

        #endregion

        #region JSONPARAMETERS

#if ODIN_INSPECTOR
        [LabelText("Json参数描述")]
#endif
        [SerializeField]
        [TextArea(5, 10)]
        public string jsonParameters;

        #endregion

        #region REQUIREDPARAMETERS

#if ODIN_INSPECTOR
        [LabelText("必填参数")]
#endif
        public List<string> requiredParameters;

        #endregion

        public void ObjectAsJson(object obj, JsonSerializerSettings settings = null)
        {
            jsonParameters = JsonConvert.SerializeObject(obj, settings ?? GeneralSerializeSettings.SampleJsonSerializerSettings);
        }

        public ParamsStandardError VerifyParams()
        {
            try
            {
                return JObject.Parse(jsonParameters).Count > 0 ? ParamsStandardError.Success : ParamsStandardError.JsonMissingProperty;
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
                { "name", functionName },
                { "description", description },
                { "parameters", JObject.Parse(jsonParameters) },
                { "required", JToken.FromObject(requiredParameters) }
            };
        }
    }
}