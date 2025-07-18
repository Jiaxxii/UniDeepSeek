using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


#if RIDER
using JetBrains.Annotations;
#else
using Xiyu.UniDeepSeek.Annotations;
#endif

namespace Xiyu.UniDeepSeek.MessagesType
{
    [System.Serializable]
    public class FunctionCallMessage : Message
    {
        public FunctionCallMessage([NotNull] UniDeepSeek.Message callMessage)
        {
            _callMessage = callMessage;
            Content = HideContent;
        }

#if UNITY_EDITOR && !ODIN_INSPECTOR
        [UnityEngine.SerializeField] private RoleType role = RoleType.Tool;
        private void ForgetWaring() => _ = role;
#endif
        [JsonIgnore] public override RoleType Role => RoleType.Tool;

        #region CallMessage

#if ODIN_INSPECTOR
        [ShowInInspector, InlineProperty, ReadOnly]
#else
        [UnityEngine.SerializeField]
#endif
        private UniDeepSeek.Message _callMessage;

        [JsonIgnore] public UniDeepSeek.Message CallMessage => _callMessage;

        #endregion

        public override JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            var fromObject = JObject.FromObject(CallMessage, serializer ?? GeneralSerializeSettings.SampleJsonSerializer);
            return fromObject;
        }
    }
}