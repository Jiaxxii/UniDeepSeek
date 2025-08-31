using System;

namespace Xiyu.UniDeepSeek
{
    public static class TokenPricingCalculatorExpand
    {
        private static readonly (TimeSpan beforeDawn, TimeSpan afterDawn) TimeInterval =
            (new TimeSpan(0, 30, 0), new TimeSpan(8, 30, 0));


        /// <summary>
        /// 计算 Usage 的价格 （￥）
        /// </summary>
        /// <param name="usage"></param>
        /// <param name="chatModel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        // [Obsolete("DeepSeek官方 将在 2025 年 09 月 06 日 00:00 起取消夜间优惠 ")]
        public static decimal CalculatePrice(this Usage usage, ChatModel chatModel)
        {
            var utcNow = DateTime.UtcNow;
            var beijingTime = utcNow.AddHours(8); // 转换为北京时间

            // 判断是否在优惠时段（00:30-08:30 北京时间）
            var isDiscountPeriod = IsDiscountPeriod(beijingTime.TimeOfDay);

            // 获取价格配置
            var (inputHit, inputMiss, output) = GetPricePerMillion(chatModel, isDiscountPeriod);

            // 计算输入费用
            var inputCost = (usage.PromptCacheHitTokens / 1_000_000m) * inputHit
                            + (usage.PromptCacheMissTokens / 1_000_000m) * inputMiss;

            // 计算输出费用
            var outputTokens = chatModel switch
            {
                ChatModel.Chat => usage.CompletionTokens,
                ChatModel.Reasoner => usage.CompletionTokens + usage.CompletionTokensDetails.ReasoningTokens,
                _ => throw new ArgumentOutOfRangeException(nameof(chatModel), chatModel, null)
            };

            var outputCost = (outputTokens / 1_000_000m) * output;

            return inputCost + outputCost;
        }

        private static bool IsDiscountPeriod(TimeSpan timeOfDay)
        {
            return timeOfDay >= TimeInterval.beforeDawn && timeOfDay < TimeInterval.afterDawn;
        }


        private static (decimal inputHit, decimal inputMiss, decimal output) GetPricePerMillion(ChatModel chatModel, bool isDiscountPeriod)
        {
            return chatModel switch
            {
                ChatModel.Chat => isDiscountPeriod ? (0.25m, 1m, 4m) : (0.5m, 2m, 8m),
                ChatModel.Reasoner => isDiscountPeriod ? (0.25m, 1m, 4m) : (1m, 4m, 16m),
                _ => throw new ArgumentException("Invalid chat model")
            };
        }
    }
}