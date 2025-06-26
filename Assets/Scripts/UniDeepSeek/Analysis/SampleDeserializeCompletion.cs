using Newtonsoft.Json;

namespace Xiyu.UniDeepSeek
{
    public sealed class SampleDeserializeCompletion : IAnalysisChatCompletion
    {
        ChatCompletion IAnalysisChatCompletion.AnalysisChatCompletion(ref string jsonData, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<ChatCompletion>(jsonData, settings);
        }
    }
}