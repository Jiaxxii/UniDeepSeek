using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Xiyu.UniDeepSeek.Tools;
#if RIDER
using JetBrains.Annotations;

#else
using Xiyu.UniDeepSeek.Annotations;
#endif

namespace Xiyu.UniDeepSeek
{
    public class DeepSeekChat : ChatProcessor
    {
        public DeepSeekChat([NotNull] ChatRequestParameter setting, [NotNull] string apiKey) : base(apiKey)
        {
            Setting = setting;
        }

        public DeepSeekChat([NotNull] ChatRequestParameter setting, IApiKeyConverter apiKeyConverter) : base(apiKeyConverter)
        {
            Setting = setting;
        }

        /// <summary>
        /// 请求配置
        /// </summary>
        [NotNull]
        public ChatRequestParameter Setting { get; set; }

        /// <summary>
        /// 数据解析器
        /// </summary>
        [NotNull]
        public IAnalysisCompletion<ChatCompletion> AnalysisCompletion { get; set; } = new SampleDeserializeCompletion();

        /// <summary>
        /// 异步数据流解析器
        /// </summary>
        [NotNull]
        public IAnalysisCompletionsAsync<ChatCompletion> AnalysisCompletionsAsync { get; set; } = new StreamDeserializeCompletions();

        /// <summary>
        /// 工具函数调用中心
        /// </summary>
        public FunctionCallCenter FunctionCallCenter { get; } = new();

        /// <summary>
        /// 在消息列表发生变化时调用的回调函数
        /// </summary>
        public event Action<List<MessagesType.Message>> OnMessageListChange;

        /// <summary>
        /// 是否使用并发调用工具方法 （默认:true）
        /// </summary>
        public bool UseConcurrency { get; set; } = true;

        /// <summary>
        /// 是否移除无效的函数调用 （默认:true）
        /// </summary>
        public bool RemoveInvalidFunctionCalls { get; set; } = true;

        /// <summary>
        /// 自动删除请求后没用的工具消息以减少网络流量 （默认:true）
        /// </summary>
        public bool AutoRemoveFunctionCallMessage { get; set; } = true;

        public int MaxFunctionCallCount { get; set; } = 10;

        /// <summary>
        /// 消息过滤器，用于过滤掉不需要的消息
        /// </summary>
        [NotNull]
        public Func<int, int, MessagesType.Message, bool> MessageRemoveFilter { get; set; } = DefaultMessageRemoveFilter;


        // private ChatCompletion _lastChatCompletion;

        private readonly JObject _streamIncludedUsage = new() { { "stream_options", new JObject { { "included_usage", true } } } };


        /// <summary>
        /// 注册工具函数
        /// </summary>
        /// <code lang="csharp">
        /// var parameters = JsonConvert.SerializeObject(new
        /// {
        ///     type = "object",
        ///     properties = new
        ///     {
        ///         location = new
        ///         {
        ///             type = "string",
        ///             description = "城市名称"
        ///         }
        ///     }
        /// }, Formatting.None);
        /// // parameters = "{\"type\":\"object\",\"properties\":{\"location\":{\"type\":\"string\",\"description\":\"城市名称\"}}}";
        /// deepSeekChat.RegisterFunction("get_weather", "获取城市天气信息，用户应该提供城市名称。", parameters, fc =>
        /// {
        ///     // fc.Function.Arguments -> 模型参数以JOSN格式提供
        ///     var c = Random.Range(26, 35);
        ///     return UniTask.FromResult((fc.Id /*固定传入ID*/, $"{c}度"));
        /// }, "location");
        /// </code>
        /// <param name="functionName">方法名称</param>
        /// <param name="description">方法描述</param>
        /// <param name="parameters">方法参数描述</param>
        /// <param name="function">方法实现</param>
        /// <param name="requiredParameters">必须出现的参数列表</param>
        public bool RegisterFunction(
            [NotNull] string functionName,
            [NotNull] string description,
            [NotNull] string parameters,
            [NotNull] FunctionCallHandler function,
            [NotNull] params string[] requiredParameters)
        {
            if (FunctionCallCenter.RegisterFunction(functionName, function) == false)
            {
                return false;
            }

            if (Setting.ToolInstances.Any(t => t.FunctionDefine.FunctionName == functionName))
            {
                Debug.LogWarning($"存在同名工具函数名称 \"{functionName}\"，请进行更换。");
                return false;
            }

            var toolInstance = new ToolInstance
            {
                FunctionDefine = new FunctionDefine
                {
                    FunctionName = functionName,
                    Description = description,
                    JsonParameters = parameters,
                    RequiredParameters = requiredParameters.ToList(),
                },
            };
            Setting.ToolInstances.Add(toolInstance);

            return true;
        }


        /// <summary>
        /// 【可自动调用函数】 发起一起聊天补全请求
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async UniTask<(ChatState state, ChatCompletion chatCompletion)> ChatCompletionAsync(CancellationToken? cancellation = null)
        {
            CleanUpMessageList();
            var maxFunctionCallCount = MaxFunctionCallCount;
            var usages = new List<Usage>();
            while (maxFunctionCallCount > 0)
            {
                var requestJson = Setting.FromObjectAsToken(GeneralSerializeSettings.SampleJsonSerializer).ToString(Formatting.None);

                string responseJson;
                try
                {
                    responseJson = await GetChatCompletionStringAsync(GetChatCompletionPath(), requestJson, cancellation);
                }
                catch (WebException exception) when (exception.Status == WebExceptionStatus.RequestCanceled)
                {
                    return (ChatState.Cancel, null);
                }

                var chatCompletion = AnalysisCompletion.AnalysisChatCompletion(ref responseJson, GeneralSerializeSettings.SampleJsonSerializerSettings);

                // 记录消息
                RecordMessage(chatCompletion.Choices);

                if (chatCompletion.Choices[0].SourcesMessage.ToolCalls is { Count: > 0 } == false)
                {
                    chatCompletion.Usage!.AppendUsage(usages);
                    return (ChatState.Success, chatCompletion);
                }

                usages.Add(chatCompletion.Usage);

                try
                {
                    var isCancellationRequested = await ProcessFunctionCallsAsync(chatCompletion.Choices[0].SourcesMessage, UseConcurrency, cancellation);
                    if (isCancellationRequested)
                    {
                        return (ChatState.Cancel, null);
                    }
                }
                catch (OperationCanceledException)
                {
                    return (ChatState.Cancel, null);
                }

                maxFunctionCallCount--;
            }

            return (ChatState.MaxFunctionCallCountReached, null);
        }

        /// <summary>
        /// 【不记录消息】【流式】【不调用函数】 发起一起聊天补全请求
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async UniTask<UniTaskCancelableAsyncEnumerable<ChatCompletion>> StreamChatCompletionsEnumerableAsync(CancellationToken? cancellation = null)
        {
            CleanUpMessageList();
            var jsonObject = (JObject)Setting.FromObjectAsToken(GeneralSerializeSettings.SampleJsonSerializer);
            if (Setting.StreamIncludedUsage)
            {
                jsonObject.Add("stream_options", _streamIncludedUsage);
            }

            jsonObject.Add("stream", true);

            var requestJson = jsonObject.ToString(Formatting.None);

            try
            {
                var stream = await SendStreamRequestAsync(GetChatCompletionPath(), requestJson, cancellation);

                return AnalysisCompletionsAsync.AnalysisChatCompletion(stream, GeneralSerializeSettings.SampleJsonSerializerSettings, cancellation);
            }
            catch (WebException exception) when (exception.Status == WebExceptionStatus.RequestCanceled)
            {
                throw new OperationCanceledException(exception.Message, exception);
            }
        }


        /// <summary>
        /// 【流式】【自动记录消息】【处理方法回调】 发起一起聊天补全请求
        /// </summary>
        /// <param name="onCompletion">在完全处理完消息后调用的回调函数，返回一个全新的`ChatCompletion`对象</param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public UniTaskCancelableAsyncEnumerable<ChatCompletion> StreamChatCompletionsEnumerableAsync(Action<ChatCompletion> onCompletion = null,
            CancellationToken? cancellation = null)
        {
            return UniTaskAsyncEnumerable.Create<ChatCompletion>(Create).WithCancellation(cancellation ?? CancellationToken.None);

            async UniTask Create(IAsyncWriter<ChatCompletion> writer, CancellationToken cst)
            {
                var maxFunctionCallCount = MaxFunctionCallCount;
                var toolsUsage = new List<Usage>();
                do
                {
                    CleanUpMessageList();
                    var requestJson = GetStreamRequestJson();
                    ChatCompletion usageCompletion;
                    try
                    {
                        usageCompletion = await CombineStreamCompletionAsync(writer, GetChatCompletionPath(), requestJson, cst);
                    }
                    catch (WebException exception) when (exception.Status == WebExceptionStatus.RequestCanceled)
                    {
                        throw new OperationCanceledException(exception.Message, exception);
                    }

                    if (usageCompletion.Choices[0].SourcesMessage.ToolCalls == null || usageCompletion.Choices[0].SourcesMessage.ToolCalls.Count == 0)
                    {
                        usageCompletion.Usage!.AppendUsage(toolsUsage);
                        onCompletion?.Invoke(usageCompletion);
                        RecordMessage(usageCompletion.Choices);
                        return;
                    }

                    toolsUsage.Add(usageCompletion.Usage);
                    var isCancellationRequested = await ProcessFunctionCallsAsync(usageCompletion.Choices[0].SourcesMessage, UseConcurrency, cst);
                    if (isCancellationRequested)
                    {
                        throw new OperationCanceledException("工具函数调用取消。");
                    }
                } while (maxFunctionCallCount-- > 0);

                throw new InvalidOperationException("达到最大函数调用次数。");
            }
        }

        /// <summary>
        /// <para>【对话前缀续写】【自动记录消息】 发起前缀补全请求</para>
        /// <para>
        /// （对话前缀续写，不调用函数，如果`Setting.ToolChoice.FunctionCallModel` 不为 `FunctionCallModel.None`，
        /// 则在请求时会将其甚至为 `FunctionCallModel.None` 这将再请求结束时恢复原设置）</para>
        /// <para>注意*当 `<see cref="think"/>` 不为空时，将会使用 `Reasoner` 模型，否则将使用 `Chat` 模型。直到请求完成时，模型将恢复为原设置。</para>
        /// </summary>
        /// <param name="prefix">前缀内容，模型的回答将以此为前缀（如果已经添加到消息列表中，则不需要再次传入）</param>
        /// <param name="think">思考链，模型将根据此思考链进行回答（如果已经添加到消息列表中，则不需要再次传入）</param>
        /// <param name="name">可以选填的参与者的名称，为模型提供信息以区分相同角色的参与者。</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>对话补全结果</returns>
        public async UniTask<(ChatState state, ChatCompletion chatCompletion)> ChatPrefixCompletionAsync([CanBeNull] string prefix = null, [CanBeNull] string think = null,
            [CanBeNull] string name = null,
            CancellationToken? cancellationToken = null)
        {
            var (state, completion) = await ExecuteWithModelAsync(think, async cancelToken =>
            {
                var requestJson = GetPrefixModelRequestJson(false, prefix, think, name);

                string responseJson;
                try
                {
                    responseJson = await GetChatCompletionStringAsync(GetChatCompletionPath(true), requestJson, cancelToken);
                }
                catch (WebException exception) when (exception.Status == WebExceptionStatus.RequestCanceled)
                {
                    return (ChatState.Cancel, null);
                }

                return (ChatState.Success, AnalysisCompletion.AnalysisChatCompletion(ref responseJson, GeneralSerializeSettings.SampleJsonSerializerSettings));
            }, cancellationToken ?? CancellationToken.None);

            if (state == ChatState.Cancel)
            {
                return (ChatState.Cancel, null);
            }

            if (completion != null)
            {
                // 将前缀添加到消息前面
                InjectPrefixContent(completion, prefix, think);

                // 记录消息
                RecordMessage(completion.Choices);
            }

            return (ChatState.Success, completion);
        }


        /// <summary>
        /// <para>【对话前缀续写】【自动记录消息】【流式】 发起前缀补全请求</para>
        /// <para>
        /// （对话前缀续写，不调用函数，如果`Setting.ToolChoice.FunctionCallModel` 不为 `FunctionCallModel.None`，
        /// 则在请求时会将其甚至为 `FunctionCallModel.None` 这将再请求结束时恢复原设置）</para>
        /// <para>注意*当 `<see cref="think"/>` 不为空时，将会使用 `Reasoner` 模型，否则将使用 `Chat` 模型。直到请求完成时，模型将恢复为原设置。</para>
        /// </summary>
        /// <param name="prefix">前缀内容，模型的回答将以此为前缀（如果已经添加到消息列表中，则不需要再次传入）</param>
        /// <param name="think">思考链，模型将根据此思考链进行回答（如果已经添加到消息列表中，则不需要再次传入）</param>
        /// <param name="name">可以选填的参与者的名称，为模型提供信息以区分相同角色的参与者。</param>
        /// <param name="onCompletion">在完全处理完消息后调用的回调函数，返回一个全新的`ChatCompletion`对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>对话补全流</returns>
        public UniTaskCancelableAsyncEnumerable<ChatCompletion> ChatPrefixStreamCompletionsEnumerableAsync([CanBeNull] string prefix, [CanBeNull] string think, string name = null,
            Action<ChatCompletion> onCompletion = null,
            CancellationToken? cancellationToken = null)
        {
            return UniTaskAsyncEnumerable.Create<ChatCompletion>(Create).WithCancellation(cancellationToken ?? CancellationToken.None);

            async UniTask Create(IAsyncWriter<ChatCompletion> writer, CancellationToken ct)
            {
                var (_, usageCompletion) = await ExecuteWithModelAsync(think, async cancelToken =>
                {
                    var requestJson = GetPrefixModelRequestJson(true, prefix, think, name);

                    try
                    {
                        return (ChatState.Success, await CombineStreamCompletionAsync(writer, GetChatCompletionPath(true), requestJson, cancelToken));
                    }
                    catch (WebException exception) when (exception.Status == WebExceptionStatus.RequestCanceled)
                    {
                        throw new OperationCanceledException(exception.Message, exception);
                    }
                }, ct);


                if (usageCompletion != null)
                {
                    // 将前缀添加到消息前面
                    InjectPrefixContent(usageCompletion, prefix, think);

                    RecordMessage(usageCompletion.Choices);

                    onCompletion?.Invoke(usageCompletion);
                }
            }
        }


        private async UniTask<(ChatState state, ChatCompletion chatCompletion)> ExecuteWithModelAsync([CanBeNull] string think,
            Func<CancellationToken, UniTask<(ChatState state, ChatCompletion chatCompletion)>> func,
            CancellationToken cancellationToken)
        {
            CleanUpMessageList();
            var currentFunctionCallModel = Setting.ToolChoice.FunctionCallModel;
            var settingModel = Setting.Model = string.IsNullOrEmpty(think) ? ChatModel.Chat : ChatModel.Reasoner;
            try
            {
                return await func(cancellationToken);
            }
            finally
            {
                // 恢复设置
                Setting.ToolChoice.FunctionCallModel = currentFunctionCallModel;
                Setting.Model = settingModel;
            }
        }

        private string GetPrefixModelRequestJson(bool isStream, [CanBeNull] string prefix, [CanBeNull] string think, [CanBeNull] string name)
        {
            if (Setting.Messages[^1] is MessagesType.AssistantMessage { Prefix: true } am)
            {
                if (!string.IsNullOrEmpty(prefix))
                    am.Content = prefix;
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(am.ReasoningContent))
                {
                    Debug.Log("Assistant message has reasoning content, clear it.");
                }
#endif
                am.ReasoningContent = string.Empty;
            }
            else if (!string.IsNullOrWhiteSpace(prefix))
            {
                var message = string.IsNullOrEmpty(think)
                    ? MessagesType.AssistantMessage.CreatePrefixMessage(prefix, name)
                    : MessagesType.AssistantMessage.CreateReasoningPrefixMessage(think, prefix, name);
                Setting.Messages.Add(message);
            }
            else
            {
                throw new ArgumentNullException(nameof(prefix), "The prefix is missing.");
            }

            var jsonObject = (JObject)Setting.FromObjectAsToken(GeneralSerializeSettings.SampleJsonSerializer);

            if (isStream)
            {
                if (Setting.StreamIncludedUsage)
                {
                    jsonObject.Add("stream_options", _streamIncludedUsage);
                }

                jsonObject.Add("stream", true);
            }

            if (Setting.ToolChoice.FunctionCallModel != FunctionCallModel.None)
            {
                Setting.ToolChoice.FunctionCallModel = FunctionCallModel.None;
                jsonObject.Remove("tools");
            }

            return jsonObject.ToString(Formatting.None);
        }

        private string GetStreamRequestJson()
        {
            var jsonObject = (JObject)Setting.FromObjectAsToken(GeneralSerializeSettings.SampleJsonSerializer);
            if (Setting.StreamIncludedUsage)
            {
                jsonObject.Add("stream_options", _streamIncludedUsage);
            }

            jsonObject.Add("stream", true);

            return jsonObject.ToString(Formatting.None);
        }


        // 处理工具调用获取调用后的消息
        private async UniTask<bool> ProcessFunctionCallsAsync(Message message, bool useConcurrency = true, CancellationToken? cancellationToken = null)
        {
            RecordMessage(new MessagesType.FunctionCallMessage(message));

            var functionCalls = message.ToolCalls!;
#if UNITY_EDITOR
            Debug.Log($"<color=#408bee>请求工具调用</color>：[{string.Join(',', functionCalls.Select(f => f.Function.FunctionName))}]");
#endif
            foreach (var invalidFunctionCall in FunctionCallCenter.GetInvalidFunctionCalls(message.ToolCalls))
            {
                if (RemoveInvalidFunctionCalls)
                {
                    Debug.Log($"Invalid function call: {invalidFunctionCall.Function.FunctionName}");
                    FunctionCallCenter.RemoveFunction(invalidFunctionCall.Function.FunctionName);
                    functionCalls.Remove(invalidFunctionCall);
                }
                else
                {
                    Debug.LogWarning($"未实现方法\" {invalidFunctionCall.Function.FunctionName}\"，将采用默认实现。");
                }
            }

            if (useConcurrency)
            {
                foreach (var result in await FunctionCallCenter.InvokeFunction(functionCalls, cancellationToken))
                {
                    switch (result.State)
                    {
                        case ChatState.Cancel:
                            return true;
                        case ChatState.Success or ChatState.InvalidFunctionCall:
                        {
                            var userMessage = new MessagesType.ToolMessage(result.FunctionId, result.Result);
                            RecordMessage(userMessage);
                            break;
                        }
                        default:
                            Debug.LogWarning($"工具函数\"{result.FunctionId}\"调用失败，状态码：{result.State}。信息：{result.Message}");
                            break;
                    }
                }
            }
            else
            {
                foreach (var functionCall in functionCalls)
                {
                    var result = await FunctionCallCenter.InvokeFunction(functionCall, cancellationToken);
                    switch (result.State)
                    {
                        case ChatState.Cancel:
                            return true;
                        case ChatState.Success or ChatState.InvalidFunctionCall:
                        {
                            var userMessage = new MessagesType.ToolMessage(functionCall.Id, result.Result);
                            RecordMessage(userMessage);
                            break;
                        }
                        default:
                            Debug.LogWarning($"工具函数\"{functionCall.Id}\"调用失败，状态码：{result.State}。信息：{result.Message}");
                            break;
                    }
                }
            }

            return false;
        }


        // 过滤消息，删除无效消息以减少网络传输量
        private void CleanUpMessageList()
        {
            if (!AutoRemoveFunctionCallMessage || Setting.Messages.Count < 3) return;

            if (Setting.Messages[^1].Role == MessagesType.RoleType.Tool || Setting.Messages[^1] is MessagesType.AssistantMessage { Prefix: false }) return;

            for (var i = Setting.Messages.Count - 1; i >= 0; i--)
            {
                if (MessageRemoveFilter(i, Setting.Messages.Count - 1, Setting.Messages[i]))
                {
                    Setting.Messages.RemoveAt(i);
                }
            }

            OnMessageListChange?.Invoke(Setting.Messages);
        }


        // 记录方法 （使用与流式对话补全）
        private void RecordMessage(List<Choice> choices)
        {
            Setting.Messages.AddRange(choices.Select(choice => (MessagesType.AssistantMessage)choice.SourcesMessage));
            OnMessageListChange?.Invoke(Setting.Messages);
        }


        // 记录消息
        private void RecordMessage(MessagesType.Message message)
        {
            Setting.Messages.Add(message);
            OnMessageListChange?.Invoke(Setting.Messages);
        }

        private async UniTask<ChatCompletion> CombineStreamCompletionAsync(IAsyncWriter<ChatCompletion> writer, string requestUrl, string requestJson, CancellationToken cst)
        {
            var stream = await SendStreamRequestAsync(requestUrl, requestJson, cst);

            var chatCompletions = new List<ChatCompletion>();
            await foreach (var chatCompletion in AnalysisCompletionsAsync.AnalysisChatCompletion(stream, GeneralSerializeSettings.SampleJsonSerializerSettings, cst))
            {
                chatCompletions.Add(chatCompletion);
                await writer.YieldAsync(chatCompletion);
            }

            var usageCompletion = ChatCompletion.CombineStreamCompletion(chatCompletions);
            return /*_lastChatCompletion = */usageCompletion;
        }

        private static void InjectPrefixContent(ChatCompletion chatCompletion, [CanBeNull] string prefix, [CanBeNull] string think)
        {
            foreach (var choice in chatCompletion.Choices)
            {
                if (string.IsNullOrEmpty(think))
                    choice.SourcesMessage.InjectPrefixAsContent(prefix);
                else choice.SourcesMessage.InjectPrefixAsReasoningContent(prefix, think);
            }
        }

        private static string GetChatCompletionPath(bool beta = false) => beta ? "beta/chat/completions" : "/chat/completions";

        /// <summary>
        /// 默认消息过滤器
        /// </summary>
        /// <param name="currentIndex">当前索引数</param>
        /// <param name="count">循环次数（固定为`Setting.Messages.Count - 1`）</param>
        /// <param name="message">当前消息</param>
        /// <returns></returns>
        private static bool DefaultMessageRemoveFilter(int currentIndex, int count, MessagesType.Message message)
        {
            if (message.Role == MessagesType.RoleType.Tool || message is MessagesType.FunctionCallMessage)
            {
                return true;
            }

            if (currentIndex != count && message is MessagesType.AssistantMessage assistant)
            {
                if (assistant.Prefix)
                {
                    return true;
                }

                assistant.ReasoningContent = null;
            }

            return false;
        }
    }
}