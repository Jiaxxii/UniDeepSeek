using Xiyu.UniDeepSeek.MessagesType;

namespace Xiyu.UniDeepSeek
{
    public static class Expand
    {
        public static AssistantMessage ToAssistantMessage(this Message message, string name = null)
        {
            return string.IsNullOrEmpty(message.ReasoningContent)
                ? AssistantMessage.CreateMessage(message.Content, null, name)
                : AssistantMessage.CreateMessage(message.Content, message.ReasoningContent, name);
        }
    }
}