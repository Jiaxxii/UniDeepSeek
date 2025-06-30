using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    public class FimStreamDeserializeCompletions : IAnalysisCompletionsAsync<FimChatCompletion>
    {
        public UniTaskCancelableAsyncEnumerable<FimChatCompletion> AnalysisChatCompletion(Stream stream, JsonSerializerSettings settings,
            CancellationToken? cancellationToken = null)
        {
            return UniTaskAsyncEnumerable.Create<FimChatCompletion>(Create)
                .WithCancellation(cancellationToken ?? CancellationToken.None);

            async UniTask Create(IAsyncWriter<FimChatCompletion> writer, CancellationToken cts)
            {
                await foreach (var data in UnitySseParser.ParseStreamAsync(stream, cts))
                {
                    cts.ThrowIfCancellationRequested();
                    var trimData = data.Trim();
                    if (trimData.Equals("[DONE]", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    if (trimData.Equals(": keep-alive", StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.LogWarning("Keep-alive message received.");
                        break;
                    }

                    var chatCompletion = JsonConvert.DeserializeObject<FimChatCompletion>(data, settings);
                    await writer.YieldAsync(chatCompletion);
                }
            }
        }
    }
}