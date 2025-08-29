namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public interface IAssistantMessage : IThenableMessage
    {
        ISystemMessage AddSystemMessage(string message, string name = null);
        IUserMessage AddUserMessage(string message, string name = null);
    }
}