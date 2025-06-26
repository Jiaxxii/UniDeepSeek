using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xiyu.UniDeepSeek.MessagesType
{
    public class FunctionCallMessage : Message
    {
        public FunctionCallMessage([NotNull] UniDeepSeek.Message callMessage)
        {
            CallMessage = callMessage;
            Content = HideContent;
        }

        [JsonIgnore] public override RoleType Role => RoleType.Tool;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.InlineProperty]
#endif
        [JsonIgnore]
        public UniDeepSeek.Message CallMessage { get; }

        public override JToken FromObjectAsToken(JsonSerializer serializer = null)
        {
            var fromObject = JObject.FromObject(CallMessage, serializer ?? GeneralSerializeSettings.SampleJsonSerializer);
            return fromObject;
        }
    }
}