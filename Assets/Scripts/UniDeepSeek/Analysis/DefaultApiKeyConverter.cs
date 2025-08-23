using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xiyu.UniDeepSeek
{
    public class DefaultApiKeyConverter : IApiKeyConverter
    {
        private readonly HashSet<char> _invalidChars = new() { '\r', '\n', ' ' };

        public DefaultApiKeyConverter(string apiKey)
        {
            var bytes = Encoding.UTF8.GetBytes(apiKey);
            var base64String = Convert.ToBase64String(bytes);
            _apiKey = Encoding.UTF8.GetBytes(base64String);
        }

        private readonly byte[] _apiKey;


        public string GetApiKey()
        {
            var base64String = Encoding.UTF8.GetString(_apiKey);
            var bytes = Convert.FromBase64String(base64String);
            return Encoding.UTF8.GetString(bytes);
        }

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