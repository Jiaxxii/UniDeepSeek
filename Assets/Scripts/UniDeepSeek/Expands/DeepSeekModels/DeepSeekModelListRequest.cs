using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Xiyu.UniDeepSeek.DeepSeekModels
{
    public static class DeepSeekModelListRequest
    {
        public static async UniTask<IEnumerable<ModelInfo>> GetModelListAsync(this ChatProcessor processor,
            CancellationToken? cancellationToken = null)
        {
            var jsonContent = await processor.GetStringAsync("/models", cancellationToken);

            var jObject = JObject.Parse(jsonContent);

            if (jObject.TryGetValue("data", out var modelList) && modelList is JArray array)
            {
                return array.ToObject<IEnumerable<ModelInfo>>();
            }

            return Array.Empty<ModelInfo>();
        }
    }
}