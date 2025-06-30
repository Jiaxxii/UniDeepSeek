using Newtonsoft.Json;

namespace Xiyu.UniDeepSeek
{
    public sealed class SampleDeserializeCompletion : IAnalysisCompletion<ChatCompletion>
    {
        ChatCompletion IAnalysisCompletion<ChatCompletion>.AnalysisChatCompletion(ref string jsonData, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<ChatCompletion>(jsonData, settings);
        }
    }
}