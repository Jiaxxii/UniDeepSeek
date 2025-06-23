namespace Xiyu.UniDeepSeek
{
    public interface ISerializeParameters
    {
        MessagesType.ParamsStandardError VerifyParams();

        Newtonsoft.Json.Linq.JToken FromObjectAsToken(Newtonsoft.Json.JsonSerializer serializer = null);
    }
}