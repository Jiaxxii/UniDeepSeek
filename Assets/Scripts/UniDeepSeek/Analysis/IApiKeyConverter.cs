using System;

namespace Xiyu.UniDeepSeek
{
    public interface IApiKeyConverter : IDisposable
    {
        string GetApiKey();
        bool ValidateApiKey(string apiKey);
    }
}