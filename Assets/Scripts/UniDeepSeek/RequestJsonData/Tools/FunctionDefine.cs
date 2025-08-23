using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.UniDeepSeek.MessagesType;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek.Tools
{
    [System.Serializable]
    public sealed class FunctionDefine : ISerializeParameters
    {
        #region FUNCTIONNAME

#if ODIN_INSPECTOR
        [LabelText("方法名称")]
#endif
        [SerializeField]
        private string functionName;

        public string FunctionName
        {
            get => functionName;
            set => functionName = value;
        }

        #endregion


        #region DESCRIPTION

#if ODIN_INSPECTOR
        [LabelText("方法描述")]
#endif
        [SerializeField, TextArea(1, 2)]
        private string description;

        public string Description
        {
            get => description;
            set => description = value;
        }

        #endregion

        #region JSONPARAMETERS

#if ODIN_INSPECTOR
        [LabelText("Json参数描述")]
#endif
        [SerializeField]
        [TextArea(5, 10)]
        private string jsonParameters;

        public string JsonParameters
        {
            get => jsonParameters;
            set => jsonParameters = value;
        }

        #endregion

        #region REQUIREDPARAMETERS

#if ODIN_INSPECTOR
        [LabelText("必要参数")]
#endif
        [SerializeField]
        private List<string> requiredParameters;

        public List<string> RequiredParameters
        {
            get => requiredParameters;
            set => requiredParameters = value;
        }

        #endregion


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
                { "parameters", JObject.Parse(JsonParameters) },
                { "required", JToken.FromObject(RequiredParameters) },
            };
        }
    }
}