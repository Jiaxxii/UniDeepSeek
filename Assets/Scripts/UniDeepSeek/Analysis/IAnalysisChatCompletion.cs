namespace Xiyu.UniDeepSeek
{
    public interface IAnalysisChatCompletion
    {
        ChatCompletion AnalysisChatCompletion(ref string jsonData, Newtonsoft.Json.JsonSerializerSettings settings);
    }
}