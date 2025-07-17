using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Xiyu.UniDeepSeek.Tools;

namespace Xiyu.UniDeepSeek
{
    public class FunctionCallResult
    {
        public string FunctionId { get; }
        public string Result { get; }
        public ChatState State { get; }
        public string Message { get; }

        public FunctionCallResult(string functionId, string result, ChatState state, string message = "Success")
        {
            FunctionId = functionId;
            Result = result;
            State = state;
            Message = message;
        }
    }

    public delegate UniTask<FunctionCallResult> FunctionCallHandler(FunctionCall functionCall, CancellationToken? cancellationToken = null);

    public class FunctionCallCenter
    {
        private readonly ConcurrentDictionary<string, FunctionCallHandler> _functionMap = new();

        public bool RegisterFunction(string functionName, FunctionCallHandler function)
        {
            if (_functionMap.ContainsKey(functionName))
            {
                return false;
            }

            _functionMap.TryAdd(functionName, function);
            return true;
        }

        public bool RemoveFunction(string functionName)
        {
            return _functionMap.TryRemove(functionName, out _);
        }

        public bool HasFunction(string functionName) => _functionMap.ContainsKey(functionName);

        public FunctionCallHandler InvalidFunctionCalls { get; set; } = (fc, _) => UniTask.FromResult(
            new FunctionCallResult(fc.Id, $"\"{fc.Function.FunctionName}\"工具停止服务。", ChatState.InvalidFunctionCall));

        public IEnumerable<FunctionCall> GetInvalidFunctionCalls(IEnumerable<FunctionCall> functionCalls)
        {
            return functionCalls.Where(f => !_functionMap.ContainsKey(f.Function.FunctionName));
        }

        public UniTask<FunctionCallResult> InvokeFunction(FunctionCall functionCall, CancellationToken? cancellationToken = null)
        {
            if (!_functionMap.TryGetValue(functionCall.Function.FunctionName, out var func))
            {
                return InvalidFunctionCalls(functionCall, cancellationToken ?? CancellationToken.None);
            }

            return func.Invoke(functionCall);
        }

        public UniTask<FunctionCallResult[]> InvokeFunction(IEnumerable<FunctionCall> args, CancellationToken? cancellationToken = null)
        {
            var tasks = new List<UniTask<FunctionCallResult>>();
            foreach (var functionCall in args)
            {
                if (_functionMap.TryGetValue(functionCall.Function.FunctionName, out var func))
                {
                    tasks.Add(func.Invoke(functionCall, cancellationToken));
                }
                else
                {
                    tasks.Add(InvalidFunctionCalls(functionCall, cancellationToken));
                }
            }

            return UniTask.WhenAll(tasks);
        }
    }
}