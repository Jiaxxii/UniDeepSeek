using Xiyu.UniDeepSeek.SettingBuilding;

namespace Xiyu.UniDeepSeek
{
    public partial class DeepSeekChat
    {
        public static RequestParameterBuilder Create(IMessage message = null, ITool tool = null)
        {
            return new ChatRequestParameter().CreateSettings(message, tool);
        }
    }
}