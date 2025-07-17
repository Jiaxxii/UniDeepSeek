using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xiyu.UniDeepSeek.MessagesType;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

#else
using UnityEngine;
#endif

namespace Xiyu.UniDeepSeek.Tools
{
    [System.Serializable]
    public sealed class FunctionDefine : ISerializeParameters
    {
        #region FUNCTIONNAME

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("方法名称")]
#else
        [SerializeField]
#endif
        private string _functionName;

        public string FunctionName
        {
            get => _functionName;
            set => _functionName = value;
        }

        #endregion


        #region DESCRIPTION

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("方法描述")]
#else
        [SerializeField, TextArea(1, 2)]
#endif
        private string _description;

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        #endregion

        #region JSONPARAMETERS

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("Json参数描述")]
#else
        [SerializeField] [TextArea(5, 10)]
#endif
        private string _jsonParameters;

        public string JsonParameters
        {
            get => _jsonParameters;
            set => _jsonParameters = value;
        }

        #endregion

        #region REQUIREDPARAMETERS

#if ODIN_INSPECTOR
        [ShowInInspector] [LabelText("必要参数")]
#else
        [SerializeField]
#endif
        private List<string> _requiredParameters;

        public List<string> RequiredParameters
        {
            get => _requiredParameters;
            set => _requiredParameters = value;
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