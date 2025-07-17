using System.Collections.Generic;
using System.Linq;

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    public partial class FimChatCompletion
    {
        public static FimChatCompletion CombineStreamCompletion(List<FimChatCompletion> completions)
        {
            var choices = new List<FimChoice>();

            for (var i = 0; i < completions[0].Choices.Length; i++)
            {
                var index = i;

                var fimChoices = completions.Select(c => c.Choices[index]).ToArray();

                var combineText = string.Concat(fimChoices.Select(c => c.Text));

                var array = fimChoices.Where(c => c.Logprobs != null).ToArray();
                var fimLogprobs = new FimLogprobs(
                    array.SelectMany(c => c.Logprobs.TextTextOffset).ToArray(),
                    array.SelectMany(c => c.Logprobs.TokenLogprobs).ToArray(),
                    array.SelectMany(c => c.Logprobs.Tokens).ToArray(),
                    array.SelectMany(c => c.Logprobs.TopLogprobs).ToArray()
                );

                var fimChoice = new FimChoice(fimChoices[^1].FinishReason, fimChoices[^1].Index, combineText, fimLogprobs);
                choices.Add(fimChoice);
            }

            return new FimChatCompletion(
                completions[^1].Id,
                completions[^1].Created,
                completions[^1].Model,
                completions[^1].SystemFingerprint,
                completions[^1].Object,
                choices.ToArray(),
                completions.Last(c => c.Usage != null).Usage
            );
        }
    }
}