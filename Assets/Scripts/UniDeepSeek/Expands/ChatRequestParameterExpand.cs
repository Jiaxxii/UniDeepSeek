using Xiyu.UniDeepSeek.SettingBuilding;

namespace Xiyu.UniDeepSeek
{
    public static class ChatRequestParameterExpand
    {
        public static RequestParameterBuilder CreateSettings(this ChatRequestParameter param, IMessage message = null, ITool tool = null)
        {
            return new RequestParameterBuilder(param, message, tool);
        }
    }
}