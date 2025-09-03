using Cysharp.Threading.Tasks;
using Xiyu.UniDeepSeek.Events.StreamChatCompletion.Buffer;
using Xiyu.UniDeepSeek.Events.StreamChatCompletion;

namespace Xiyu.UniDeepSeek.UnityTextMeshProUGUI
{
    /// <summary>
    /// 提供 TextMeshPro 组件的流式聊天内容显示功能
    /// </summary>
    public static class TextMeshProPrintUtility
    {
        /// <summary>
        /// 以流式方式显示聊天完成事件，提供基本的内容刷新控制
        /// </summary>
        /// <param name="textMeshPro">要显示文本的 TextMeshPro 组件</param>
        /// <param name="asyncEnumerable">聊天完成数据流</param>
        /// <param name="flushThreshold">内容刷新阈值，达到此值时将内容推送到 UI</param>
        /// <param name="flushCriteriaOption">内容刷新标准，决定如何计算内容长度</param>
        /// <returns>表示异步操作的 UniTask</returns>
        public static UniTask DisplayChatStreamBasicAsync(
            this TMPro.TMP_Text textMeshPro,
            UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable,
            int flushThreshold = 10,
            ContentFlushCriteriaOption flushCriteriaOption = ContentFlushCriteriaOption.ByCharacterCount)
        {
            var option = new ContentOption()
            {
                FlushThreshold = flushThreshold,
                FlushCriteriaOption = flushCriteriaOption,
            };

            var buffer = new ContentBuffer(text => textMeshPro.text += text, option);

            return StreamCompletionConsumer.ProcessContentOnlyStreamAsync(asyncEnumerable, buffer);
        }

        /// <summary>
        /// 以流式方式显示包含推理内容的聊天完成事件，支持颜色格式化和换行控制
        /// </summary>
        /// <param name="textMeshPro">要显示文本的 TextMeshPro 组件</param>
        /// <param name="asyncEnumerable">聊天完成数据流</param>
        /// <param name="colorHex">推理内容的十六进制颜色代码（如 "#FF5733" 或 "FF5733"，为空时不设置颜色）</param>
        /// <param name="reasoningNewlineCount">推理内容后的换行数量</param>
        /// <param name="flushThreshold">内容刷新阈值，达到此值时将内容推送到 UI</param>
        /// <param name="flushCriteriaOption">内容刷新标准，决定如何计算内容长度</param>
        /// <returns>表示异步操作的 UniTask</returns>
        public static UniTask DisplayReasoningChatStreamBasicAsync(
            this TMPro.TMP_Text textMeshPro,
            UniTaskCancelableAsyncEnumerable<ChatCompletion> asyncEnumerable,
            string colorHex = null,
            int reasoningNewlineCount = 1,
            int flushThreshold = 10,
            ContentFlushCriteriaOption flushCriteriaOption = ContentFlushCriteriaOption.ByCharacterCount)
        {
            var option = new ReasoningOption()
            {
                ColorHex = colorHex,
                ReasoningNewlineCount = reasoningNewlineCount,
                FlushThreshold = flushThreshold,
                FlushCriteriaOption = flushCriteriaOption,
            };

            var buffer = new ReasoningContentBuffer(text => textMeshPro.text += text, option);

            return StreamCompletionConsumer.ProcessStreamWithReasoningAsync(asyncEnumerable, buffer, buffer);
        }

        /// <summary>
        /// 创建支持事件处理的流式聊天显示，提供内容刷新控制
        /// </summary>
        /// <param name="textMeshPro">要显示文本的 TextMeshPro 组件</param>
        /// <param name="flushThreshold">内容刷新阈值，达到此值时将内容推送到 UI</param>
        /// <param name="flushCriteriaOption">内容刷新标准，决定如何计算内容长度</param>
        /// <returns>StreamCompletionEventFacade 实例，用于管理聊天完成事件</returns>
        public static StreamCompletionEventFacade DisplayChatStreamWithEvents(
            this TMPro.TMP_Text textMeshPro,
            int flushThreshold = 10,
            ContentFlushCriteriaOption flushCriteriaOption = ContentFlushCriteriaOption.ByCharacterCount)
        {
            var option = new ContentOption()
            {
                FlushThreshold = flushThreshold,
                FlushCriteriaOption = flushCriteriaOption,
            };

            var buffer = new ContentBuffer(text => textMeshPro.text += text, option);

            return StreamCompletionEventFacade.CreateByDefaultConsumer(buffer);
        }

        /// <summary>
        /// 创建支持事件处理的流式聊天显示，包含推理内容支持，支持颜色格式化和换行控制
        /// </summary>
        /// <param name="textMeshPro">要显示文本的 TextMeshPro 组件</param>
        /// <param name="colorHex">推理内容的十六进制颜色代码（如 "#FF5733" 或 "FF5733"，为空时不设置颜色）</param>
        /// <param name="reasoningNewlineCount">推理内容后的换行数量</param>
        /// <param name="flushThreshold">内容刷新阈值，达到此值时将内容推送到 UI</param>
        /// <param name="flushCriteriaOption">内容刷新标准，决定如何计算内容长度</param>
        /// <returns>StreamCompletionEventFacade 实例，用于管理聊天完成事件</returns>
        public static StreamCompletionEventFacade DisplayReasoningStreamWithEvents(
            this TMPro.TMP_Text textMeshPro,
            string colorHex = null,
            int reasoningNewlineCount = 1,
            int flushThreshold = 10,
            ContentFlushCriteriaOption flushCriteriaOption = ContentFlushCriteriaOption.ByCharacterCount)
        {
            var option = new ReasoningOption()
            {
                ColorHex = colorHex,
                ReasoningNewlineCount = reasoningNewlineCount,
                FlushThreshold = flushThreshold,
                FlushCriteriaOption = flushCriteriaOption,
            };

            var buffer = new ReasoningContentBuffer(text => textMeshPro.text += text, option);

            return StreamCompletionEventFacade.CreateByDefaultConsumer(buffer);
        }
    }
}