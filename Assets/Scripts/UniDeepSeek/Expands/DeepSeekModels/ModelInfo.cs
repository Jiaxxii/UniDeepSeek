using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Xiyu.UniDeepSeek.DeepSeekModels
{
    public readonly struct ModelInfo
    {
        [JsonConstructor]
        public ModelInfo(string modelName, string type, string ownedBy)
        {
            ModelName = modelName;
            Type = type;
            OwnedBy = ownedBy;
        }

        [JsonProperty("id")] public string ModelName { get; }
        [JsonProperty("object")] public string Type { get; }
        [JsonProperty("owned_by")] public string OwnedBy { get; }


        /// <summary>
        /// 找出 `modelInfos` 中使用不包含在 `models` 中的模型
        /// </summary>
        /// <param name="modelInfos"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUnknownModels(IEnumerable<ModelInfo> modelInfos)
        {
            var validModels = Enum.GetNames(typeof(ChatModel))
                .ToHashSet(StringComparer.CurrentCultureIgnoreCase);

            return modelInfos
                .Where(info => !validModels.Contains(info.ModelName))
                .Select(info => info.ModelName);
        }

        public override string ToString()
        {
#if UNITY_EDITOR
            return $"(<color=#F9E235>{Type}</color>) <color=#38B89E>{ModelName}</color> owned by <color=#5396A3>{OwnedBy}</color>";

#else
            return $"({Type}) {ModelName} owned by {OwnedBy}";
#endif
        }
    }
}