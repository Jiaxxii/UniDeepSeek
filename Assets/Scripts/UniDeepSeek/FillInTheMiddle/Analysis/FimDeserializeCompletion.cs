using Newtonsoft.Json;

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    public class FimDeserializeCompletion : IAnalysisCompletion<FimChatCompletion>
    {
        FimChatCompletion IAnalysisCompletion<FimChatCompletion>.AnalysisChatCompletion(ref string jsonData, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<FimChatCompletion>(jsonData, settings);
        }
    }
}