using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


namespace Xiyu.UniDeepSeek.Tools
{
    [Serializable]
    public class FunctionArg
    {
        public FunctionArg(int index, string name, string arguments)
        {
            FunctionName = name;
            Arguments = arguments;
        }

        /// <summary>
        /// 模型要求调用的函数名称
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("方法名称"), LabelWidth(70)]
        [UnityEngine.Tooltip("模型要求调用的函数名称")]
#endif
        [Newtonsoft.Json.JsonProperty("name")]
        public string FunctionName { get; }


        /// <summary>
        /// 函数索引
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("函数索引"), LabelWidth(70)]
        [UnityEngine.Tooltip("模型函数的索引")]
#endif
        [Newtonsoft.Json.JsonProperty("index")]
        public int FunctionIndex { get; }

        /// <summary>
        /// 函数入参
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("参数")]
        [UnityEngine.TextArea, UnityEngine.Tooltip("函数入参")]
#endif
        public string Arguments { get; private set; }


        public void AppendArgument(string arg)
        {
            Arguments += arg;
        }
    }
}