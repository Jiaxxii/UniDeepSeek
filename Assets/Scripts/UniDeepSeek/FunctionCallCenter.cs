using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Xiyu.UniDeepSeek.Tools;

namespace Xiyu.UniDeepSeek
{
    public delegate UniTask<(string id, string result)> FunctionCallHandler(FunctionCall functionCall);

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

        public FunctionCallHandler InvalidFunctionCalls { get; set; } = fc => UniTask.FromResult((fc.Id, "Invalid function call."));

        public IEnumerable<FunctionCall> GetInvalidFunctionCalls(IEnumerable<FunctionCall> functionCalls)
        {
            return functionCalls.Where(f => !_functionMap.ContainsKey(f.Function.FunctionName));
        }

        public UniTask<(string id, string result)> InvokeFunction(FunctionCall arg)
        {
            if (!_functionMap.TryGetValue(arg.Function.FunctionName, out var func))
            {
                return InvalidFunctionCalls(arg);
            }

            return func.Invoke(arg);
        }

        public UniTask<(string id, string result)[]> InvokeFunction(IEnumerable<FunctionCall> args)
        {
            var tasks = new List<UniTask<(string id, string result)>>();
            foreach (var functionCall in args)
            {
                if (_functionMap.TryGetValue(functionCall.Function.FunctionName, out var func))
                {
                    tasks.Add(func.Invoke(functionCall));
                }
                else
                {
                    tasks.Add(InvalidFunctionCalls(functionCall));
                }
            }

            return UniTask.WhenAll(tasks);
        }
    }
}