using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Xiyu.UniDeepSeek
{
    /// <summary>
    /// 增强版 API 密钥转换器实现
    /// </summary>
    public class EnhancedApiKeyConverter : IApiKeyConverter
    {
        private readonly byte[] _apiKeyBytes;
        private readonly HashSet<char> _invalidChars;
        private readonly Aes _aes;
        private bool _disposed = false;

        /// <summary>
        /// 使用 API 密钥初始化转换器
        /// </summary>
        /// <param name="apiKey">原始 API 密钥</param>
        /// <param name="encryptionKey">可选的加密密钥（32字节）</param>
        public EnhancedApiKeyConverter(string apiKey, byte[] encryptionKey = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API 密钥不能为空", nameof(apiKey));

            if (!ValidateApiKeyFormat(apiKey))
                throw new ArgumentException("API 密钥格式无效", nameof(apiKey));

            _apiKeyBytes = Encoding.UTF8.GetBytes(apiKey);
            _invalidChars = new HashSet<char> { '\r', '\n', ' ', '\t' };

            // 初始化 AES 加密（用于额外功能）
            _aes = Aes.Create();
            _aes.Mode = CipherMode.CBC;
            _aes.Padding = PaddingMode.PKCS7;

            if (encryptionKey != null)
            {
                if (encryptionKey.Length != 32)
                    throw new ArgumentException("加密密钥必须是32字节", nameof(encryptionKey));

                _aes.Key = encryptionKey;
            }
            else
            {
                // 使用默认密钥（在实际应用中应从安全存储获取）
                _aes.Key = Encoding.UTF8.GetBytes("Default32ByteEncryptionKey!!");
            }

            // 生成随机 IV
            _aes.GenerateIV();
        }
        
        public string GetApiKey() => Encoding.UTF8.GetString(_apiKeyBytes);
        
        public bool ValidateApiKey(string apiKey)
        {
            return apiKey.StartsWith("sk-") && !apiKey.Any(_invalidChars.Contains);
        }
        
        
        private static bool ValidateApiKeyFormat(string apiKey)
        {
            // DeepSeek API 密钥通常以 "sk-" 开头，长度在40-60字符之间
            return !string.IsNullOrEmpty(apiKey) &&
                   apiKey.StartsWith("sk-") &&
                   Regex.IsMatch(apiKey, @"^[a-zA-Z0-9\-_]+$");
        }


        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _aes?.Dispose();
                    // 清空敏感数据
                    if (_apiKeyBytes != null)
                        Array.Clear(_apiKeyBytes, 0, _apiKeyBytes.Length);
                }

                _disposed = true;
            }
        }

        ~EnhancedApiKeyConverter()
        {
            Dispose(false);
        }
    }
}