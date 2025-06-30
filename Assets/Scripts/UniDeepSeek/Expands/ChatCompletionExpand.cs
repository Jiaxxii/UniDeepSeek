using System.Collections.Generic;

namespace Xiyu.UniDeepSeek
{
    public static class ChatCompletionExpand
    {
        public static void BindTo(this FillInTheMiddle.FimDeepSeekChat fimDeepSeekChat, List<MessagesType.Message> messages, int choiceIndex = 0)
        {
            fimDeepSeekChat.OnChatCompletion += Message;
            return;

            void Message(FillInTheMiddle.FimChatCompletion chatCompletion)
            {
                if (messages == null)
                {
                    fimDeepSeekChat.OnChatCompletion -= Message;
                    
                    return;
                }

                var message = MessagesType.AssistantMessage.CreateMessage(chatCompletion.GetMessage(choiceIndex).Text);
                messages.Add(message);
            }
        }


        public static FillInTheMiddle.FimChoice GetMessage(this FillInTheMiddle.FimChatCompletion chatCompletion, int choiceIndex = 0)
        {
            return chatCompletion.Choices[choiceIndex];
        }

        public static Message GetMessage(this ChatCompletion chatCompletion, int choiceIndex = 0)
        {
            return chatCompletion.Choices[choiceIndex].SourcesMessage;
        }

        public static bool HasFunction(this ChatCompletion chatCompletion, int choiceIndex = 0)
        {
            var sourcesMessage = chatCompletion.Choices[choiceIndex].SourcesMessage;
            return sourcesMessage is { ToolCalls: { Count: > 0 } };
        }
    }
}