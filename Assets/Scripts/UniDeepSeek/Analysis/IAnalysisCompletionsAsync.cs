using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Xiyu.UniDeepSeek
{
    public interface IAnalysisCompletionsAsync<T>
    {
        UniTaskCancelableAsyncEnumerable<T> AnalysisChatCompletion(Stream stream, JsonSerializerSettings settings, CancellationToken? cancellationToken = null);
    }
}