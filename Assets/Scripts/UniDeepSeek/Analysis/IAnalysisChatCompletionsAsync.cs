using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Xiyu.UniDeepSeek
{
    public interface IAnalysisChatCompletionsAsync
    {
        UniTaskCancelableAsyncEnumerable<ChatCompletion> AnalysisChatCompletion(Stream stream, JsonSerializerSettings settings, CancellationToken? cancellationToken = null);
    }
}