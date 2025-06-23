using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Xiyu.UniDeepSeek
{
    public static class GeneralSerializeSettings
    {
        public static JsonSerializerSettings SampleJsonSerializerSettings { get; } = new()
        {
            // 压缩JSON
            Formatting = Formatting.None,
            // 忽略空值
            NullValueHandling = NullValueHandling.Ignore,
            // 忽略默认值
            DefaultValueHandling = DefaultValueHandling.Ignore,
            // 蛇形命名
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            },
            // 枚举值序列化为字符串
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            }
        };

        public static JsonSerializer SampleJsonSerializer { get; } = JsonSerializer.Create(SampleJsonSerializerSettings);
    }
}