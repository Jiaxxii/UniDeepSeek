namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public interface IUserMessage : IThenableMessage
    {
        ISystemMessage AddSystemMessage(string message, string name = null);
        IAssistantMessage AddAssistantMessage(string message, string name = null);
        IAssistantMessage AddAssistantPrefixMessage(string message, bool prefix, string name = null);
        IAssistantMessage AddReasoningPrefixMessage(string reasoningContent, string content, string name = null);
    }
}