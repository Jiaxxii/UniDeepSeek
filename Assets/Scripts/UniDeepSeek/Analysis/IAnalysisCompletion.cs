namespace Xiyu.UniDeepSeek
{
    public interface IAnalysisCompletion<out T>
    {
        T AnalysisChatCompletion(ref string jsonData, Newtonsoft.Json.JsonSerializerSettings settings);
    }
}