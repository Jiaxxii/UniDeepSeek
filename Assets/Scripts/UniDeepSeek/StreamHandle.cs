using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using Xiyu.UniDeepSeek.Tools;

namespace Xiyu.UniDeepSeek
{
    [Obsolete("`StreamHandle`处理流式`FunctionCall`数据尝试重新调用方法困难，建议使用请求器中内置的流处理。", false)]
    public class StreamHandle
    {
        private readonly IAnalysisCompletionsAsync<ChatCompletion> _completionParser;

        private readonly Func<CancellationToken, UniTask<Stream>> _streamFunc;

        private readonly JsonSerializerSettings _settings;

        private ChatCompletion _chatCompletion;

        // 新增状态枚举
        private enum StreamState
        {
            NotStarted,
            Running,
            Completed,
            Failed
        }

        public enum TaskCompletionState
        {
            Success,
            FunctionCall
        }

        private StreamState _state = StreamState.NotStarted;

        private readonly Func<ChatCompletion, UniTask<TaskCompletionState>> _onCompletion;

        public StreamHandle([CanBeNull] IAnalysisCompletionsAsync<ChatCompletion> completionParser, [CanBeNull] Func<CancellationToken, UniTask<Stream>> streamFunc,
            JsonSerializerSettings settings = null,
            Func<ChatCompletion, UniTask<TaskCompletionState>> onCompletion = null)
        {
            _completionParser = completionParser;
            _streamFunc = streamFunc;
            _settings = settings ?? GeneralSerializeSettings.SampleJsonSerializerSettings;
            _chatCompletion = null;
            _onCompletion = onCompletion;
        }

        public UniTaskCancelableAsyncEnumerable<ChatCompletion> StreamChatCompletionsEnumerableAsync(CancellationToken? cancellationToken = null)
        {
            return UniTaskAsyncEnumerable.Create<ChatCompletion>(Analysis).WithCancellation(cancellationToken ?? CancellationToken.None);
        }

        public async UniTask<ChatCompletion> WaitChatCompletionAsync(CancellationToken? cancellationToken = null)
        {
            // 等待流结束（无论成功/失败）
            while (_state is StreamState.Running or StreamState.NotStarted)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                await UniTask.Yield();
            }


            if (_state == StreamState.Failed)
                throw new InvalidOperationException("Stream processing failed");

            return _chatCompletion; // 此时 _chatCompletion 已初始化
        }

        private async UniTask Analysis(IAsyncWriter<ChatCompletion> writer, CancellationToken cts)
        {
            if (_state is StreamState.Running or StreamState.Failed)
            {
                Debug.LogError("Analysis already started or completed");
                return;
            }

            _state = StreamState.Running;

            var result = new List<ChatCompletion>();
            try
            {
                var stream = await _streamFunc.Invoke(cts);
                await foreach (var chatCompletion in _completionParser.AnalysisChatCompletion(stream, _settings, cts))
                {
                    await writer.YieldAsync(chatCompletion);
                    result.Add(chatCompletion);
                }


                var choices = MergeChoices(result);

                _chatCompletion = new ChatCompletion(choices, result[^1].ID, result[^1].Created, result[^1].Model, result[^1].SystemFingerprint, result[^1].Object,
                    result[^1].Usage);

                _state = StreamState.Completed;
            }
            catch (Exception ex)
            {
                _state = StreamState.Failed;
                Debug.LogError($"Stream failed: {ex}");
                throw; // 保持异常传播
            }
            finally
            {
                if (_onCompletion != null && _state == StreamState.Completed)
                {
                    var taskCompletionState = await _onCompletion(_chatCompletion);
                    if (taskCompletionState == TaskCompletionState.FunctionCall)
                    {
                        // 重启流
                        await foreach (var _ in StreamChatCompletionsEnumerableAsync(cts))
                        {
                        }
                    }
                }
            }
        }

        private static List<Choice> MergeChoices(List<ChatCompletion> completions)
        {
            var choices = new List<Choice>();

            for (var i = 0; i < completions[0].Choices.Count; i++)
            {
                var index = i;
                var content = string.Concat(completions.Select(c => c.Choices[index].SourcesMessage.Content));
                var reasoning = string.Concat(completions.Select(c => c.Choices[index].SourcesMessage.ReasoningContent));

                var finishReason = completions[^1].Choices[index].FinishReason;
                var choiceIndex = completions[^1].Choices[index].Index;

                var role = completions[0].Choices[index].SourcesMessage.Role;


                var functionCallList = new List<FunctionCall>();
                var currentFuncCallIndex = -1;

                var functionCalls = completions.Select(c => c.Choices[index].SourcesMessage.ToolCalls)
                    .Where(toos => toos is { Count: > 0 });
                foreach (var functionCall in functionCalls.Select(tools => tools[0]))
                {
                    if (!string.IsNullOrEmpty(functionCall.Function.FunctionName))
                    {
                        var functionArg = new FunctionArg(functionCall.Function.FunctionIndex, functionCall.Function.FunctionName, string.Empty);
                        var call = new FunctionCall(functionCall.Id, functionArg, functionCall.Type);
                        functionCallList.Add(call);
                        currentFuncCallIndex++;
                    }
                    else
                    {
                        var call = functionCallList[currentFuncCallIndex];
                        call.Function.AppendArgument(functionCall.Function.Arguments);
                    }
                }

                var logprobsArray = completions.Select(c => c.Choices[index].Logprobs)
                    .Where(logprobs => logprobs?.Content is { Length: > 0 }).SelectMany(logprobs => logprobs.Content).ToArray();


                var logprobs = new Logprobs(logprobsArray);

                var message = new Message(role, content, reasoning, functionCallList);

                var choice = new Choice(finishReason, choiceIndex, message, null, logprobs);
                choices.Add(choice);
            }

            return choices;
        }
    }
}