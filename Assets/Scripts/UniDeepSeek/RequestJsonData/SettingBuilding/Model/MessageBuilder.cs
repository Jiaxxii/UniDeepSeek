using System.Collections.Generic;
using Xiyu.UniDeepSeek.MessagesType;

namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public class MessageBuilder : IMessage, IAssistantMessage, IUserMessage, ISystemMessage
    {
        public MessageBuilder(IRequestParameter requestParameter, IList<Xiyu.UniDeepSeek.MessagesType.Message> messages)
        {
            _messages = messages;
            Base = requestParameter;
        }

        private readonly IList<Xiyu.UniDeepSeek.MessagesType.Message> _messages;

        public IRequestParameter Base { get; }


        public ISystemMessage AddSystemMessage(string message, string name = null)
        {
            _messages.Add(new SystemMessage(message, name));
            return this;
        }

        public IUserMessage AddUserMessage(string message, string name = null)
        {
            _messages.Add(new UserMessage(message, name));
            return this;
        }

        public IAssistantMessage AddAssistantMessage(string message, string name = null)
        {
            _messages.Add(AssistantMessage.CreateMessage(message, name));
            return this;
        }

        public IAssistantMessage AddAssistantPrefixMessage(string message, bool prefix, string name = null)
        {
            _messages.Add(AssistantMessage.CreatePrefixMessage(message, name));
            return this;
        }

        public IAssistantMessage AddReasoningPrefixMessage(string reasoningContent, string content, string name = null)
        {
            _messages.Add(AssistantMessage.CreateReasoningPrefixMessage(reasoningContent, content, name));
            return this;
        }
    }
}