using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Xiyu.UniDeepSeek.Tools;

namespace Xiyu.UniDeepSeek
{
    public partial class ChatCompletion
    {
        public static ChatCompletion CombineStreamCompletion(List<ChatCompletion> completions)
        {
            var choices = new List<Choice>();

            for (var i = 0; i < completions[0].Choices.Count; i++)
            {
                var index = i;
                var content = string.Concat(completions.Select(c => c.Choices[index].SourcesMessage.Content));
                var reasoning = string.Concat(completions.Select(c => c.Choices[index].SourcesMessage.ReasoningContent));

                var finishReason = completions[^1].Choices[index].FinishReason;
                var choiceIndex = completions[^1].Choices[index].Index;

                var role = completions[0].Choices[index].SourcesMessage.Role;


                var functionCallList = new List<FunctionCall>();
                var currentFuncCallIndex = -1;

                var functionCalls = completions.Select(c => c.Choices[index].SourcesMessage.ToolCalls)
                    .Where(toos => toos is { Count: > 0 });
                foreach (var functionCall in functionCalls.Select(tools => tools[0]))
                {
                    if (!string.IsNullOrEmpty(functionCall.Function.FunctionName))
                    {
                        var functionArg = new FunctionArg(functionCall.Function.FunctionIndex, functionCall.Function.FunctionName, string.Empty);
                        var call = new FunctionCall(functionCall.Id, functionArg, functionCall.Type);
                        functionCallList.Add(call);
                        currentFuncCallIndex++;
                    }
                    else
                    {
                        var call = functionCallList[currentFuncCallIndex];
                        call.Function.AppendArgument(functionCall.Function.Arguments);
                    }
                }

                var logprobsArray = completions.Select(c => c.Choices[index].Logprobs)
                    .Where(logprobs => logprobs?.Content is { Length: > 0 }).SelectMany(logprobs => logprobs.Content).ToArray();


                var logprobs = new Logprobs(logprobsArray);

                var message = new Message(role, content, reasoning, functionCallList);

                var choice = new Choice(finishReason, choiceIndex, message, null, logprobs);
                choices.Add(choice);
            }

            return new ChatCompletion(choices,
                completions[^1].ID,
                completions[^1].Created,
                completions[^1].Model,
                completions[^1].SystemFingerprint,
                completions[^1].Object,
                completions[^1].Usage);
        }
    }
}