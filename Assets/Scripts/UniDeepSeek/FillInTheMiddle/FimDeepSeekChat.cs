using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if RIDER
using JetBrains.Annotations;
#else
using Xiyu.UniDeepSeek.Annotations;
#endif

namespace Xiyu.UniDeepSeek.FillInTheMiddle
{
    public class FimDeepSeekChat : ChatProcessor
    {
        private const string RequestPath = "beta/completions";

        private readonly JObject _streamIncludedUsage = new() { { "stream_options", new JObject { { "included_usage", true } } } };

        [NotNull] public FimRequestParameter Setting { get; set; } = new();

        public FimDeepSeekChat(string apiKey) : base(apiKey)
        {
        }

        /// <summary>
        /// 数据解析器
        /// </summary>
        [NotNull]
        public IAnalysisCompletion<FimChatCompletion> AnalysisCompletion { get; set; } = new FimDeserializeCompletion();

        /// <summary>
        /// 数据解析器
        /// </summary>
        [NotNull]
        public IAnalysisCompletionsAsync<FimChatCompletion> AnalysisCompletionsAsync { get; } = new FimStreamDeserializeCompletions();

        public event Action<FimChatCompletion> OnChatCompletion;

        /// <summary>
        /// 发起一次聊天补全请求
        /// </summary>
        /// <param name="prompt">指定补全内容前缀</param>
        /// <param name="suffix">指定补全内容后缀</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public async UniTask<FimChatCompletion> ChatCompletionAsync([CanBeNull] string prompt, [CanBeNull] string suffix = null, CancellationToken? cancellationToken = null)
        {
            var requestJson = GetRequestJson(false, prompt, suffix);
            var responseJson = await GetChatCompletionStringAsync(RequestPath, requestJson, cancellationToken);

            var chatCompletion = AnalysisCompletion.AnalysisChatCompletion(ref responseJson, GeneralSerializeSettings.SampleJsonSerializerSettings);
            TryCombineResultText(chatCompletion);

            OnChatCompletion?.Invoke(chatCompletion);
            return chatCompletion;
        }

        /// <summary>
        /// 【流式】发起一次聊天补全请求
        /// </summary>
        /// <param name="prompt">指定补全内容前缀</param>
        /// <param name="suffix">指定补全内容后缀</param>
        /// <param name="onCompletion">完成时会合并所有流式<see cref="FimChatCompletion"/>将其合并成一个</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public UniTaskCancelableAsyncEnumerable<FimChatCompletion> StreamChatCompletionsEnumerableAsync(
            [CanBeNull] string prompt,
            [CanBeNull] string suffix = null,
            Action<FimChatCompletion> onCompletion = null,
            CancellationToken? cancellationToken = null)
        {
            return UniTaskAsyncEnumerable.Create<FimChatCompletion>(async (writer, cts) =>
            {
                var requestJson = GetRequestJson(true, prompt, suffix);
                var stream = await SendStreamRequestAsync(RequestPath, requestJson, cancellationToken);

                var chatCompletions = new List<FimChatCompletion>();
                await foreach (var fcc in AnalysisCompletionsAsync.AnalysisChatCompletion(stream, GeneralSerializeSettings.SampleJsonSerializerSettings, cts))
                {
                    await writer.YieldAsync(fcc);
                    chatCompletions.Add(fcc);
                }

                var combineStreamCompletion = FimChatCompletion.CombineStreamCompletion(chatCompletions);
                TryCombineResultText(combineStreamCompletion);

                onCompletion?.Invoke(combineStreamCompletion);
                OnChatCompletion?.Invoke(combineStreamCompletion);
            }).WithCancellation(cancellationToken ?? CancellationToken.None);
        }


        private string GetRequestJson(bool isStream, [CanBeNull] string prompt, [CanBeNull] string suffix = null)
        {
            if (!string.IsNullOrEmpty(prompt)) Setting.Prompt = prompt;
            else if (string.IsNullOrEmpty(Setting.Prompt)) throw new ArgumentNullException(nameof(prompt), "Prompt cannot be null or empty.");

            if (!string.IsNullOrEmpty(suffix)) Setting.Suffix = suffix;

            var jsonObject = (JObject)Setting.FromObjectAsToken(GeneralSerializeSettings.SampleJsonSerializer);

            if (!isStream) return jsonObject.ToString(Formatting.None);

            if (Setting.StreamIncludedUsage)
            {
                jsonObject.Add("stream_options", _streamIncludedUsage);
            }

            jsonObject.Add("stream", true);

            return jsonObject.ToString(Formatting.None);
        }

        private FimChatCompletion TryCombineResultText(FimChatCompletion chatCompletion)
        {
            if (!Setting.Echo) return chatCompletion;

            foreach (var fimChoices in chatCompletion.Choices)
            {
                fimChoices.InjectPromptSuffixAsText(Setting.Prompt, Setting.Suffix);
            }

            return chatCompletion;
        }
    }
}