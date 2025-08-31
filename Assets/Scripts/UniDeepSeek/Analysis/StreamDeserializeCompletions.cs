using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Xiyu.UniDeepSeek
{
    public class StreamDeserializeCompletions : IAnalysisCompletionsAsync<ChatCompletion>
    {
        UniTaskCancelableAsyncEnumerable<ChatCompletion> IAnalysisCompletionsAsync<ChatCompletion>.AnalysisChatCompletion(Stream stream, JsonSerializerSettings settings,
            CancellationToken? cancellationToken)
        {
            return UniTaskAsyncEnumerable.Create<ChatCompletion>(Create)
                .WithCancellation(cancellationToken ?? CancellationToken.None);

            async UniTask Create(IAsyncWriter<ChatCompletion> writer, CancellationToken cts)
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
                    
                    var chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(data, settings);
                    await writer.YieldAsync(chatCompletion);
                }
            }
        }


        // private static async UniTask ReadStreamSseDataAsync(Stream stream, JsonSerializerSettings settings, IAsyncWriter<ChatCompletion> writer,
        //     CancellationToken cancellationToken)
        // {
        //     await foreach (var data in UnitySseParser.ParseStreamAsync(stream, cancellationToken))
        //     {
        //         var chatCompletion = JsonConvert.DeserializeObject<ChatCompletion>(data, settings);
        //         await writer.YieldAsync(chatCompletion);
        //     }
        // }
    }
}