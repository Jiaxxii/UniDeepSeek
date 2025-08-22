using System;
using System.Collections.Generic;
using System.Linq;

namespace Xiyu.UniDeepSeek
{
    public class DefaultApiKeyConverter : IApiKeyConverter
    {
        private readonly HashSet<char> _invalidChars = new() { '\r', '\n', ' ' };

        public DefaultApiKeyConverter(string apiKey)
        {
            _apiKey = Convert.FromBase64String(apiKey);
        }

        private readonly byte[] _apiKey;

        public string GetApiKey() => System.Text.Encoding.UTF8.GetString(_apiKey);

        public bool ValidateApiKey(string apiKey)
        {
            // 适用于DeepSeek API密钥的验证规则
            return apiKey.StartsWith("sk-") && ValidateApiKey(apiKey, _invalidChars);
        }

        private static bool ValidateApiKey(string apiKey, HashSet<char> invalidChars)
        {
            return !apiKey.Any(invalidChars.Contains);
        }

        public void Dispose()
        {
            Array.Clear(_apiKey, 0, _apiKey.Length);
            GC.SuppressFinalize(this);
        }
    }
}