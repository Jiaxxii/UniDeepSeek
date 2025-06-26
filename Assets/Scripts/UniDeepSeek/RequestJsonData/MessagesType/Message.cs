using Newtonsoft.Json;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Xiyu.UniDeepSeek.MessagesType
{
    public abstract class Message : ISerializeParameters
    {
        protected const string HideContent = "HIDE_CONTENT";
        /// <summary>
        /// 消息的角色。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector]
        [ReadOnly]
        [LabelText("角色")]
#endif
        [JsonIgnore]
        public abstract RoleType Role { get; }


        /// <summary>
        /// 内容。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, HideIf("@Content == \"HIDE_CONTENT\"")]
        [LabelText("内容")]
        [TextArea(5, 10)]
#endif
        public string Content { get; set; }


        public virtual ParamsStandardError VerifyParams()
        {
            return Content == null ? ParamsStandardError.ContentInvalid : ParamsStandardError.Success;
        }

        public virtual Newtonsoft.Json.Linq.JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            var fromObject = Newtonsoft.Json.Linq.JObject.FromObject(this, serializer ?? GeneralSerializeSettings.SampleJsonSerializer);
            fromObject.Add("role", Role.ToString().ToLower());
            return fromObject;
        }
    }
}