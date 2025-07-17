using System;
using Newtonsoft.Json;
using UnityEngine;
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
            _functionName = name;
            _arguments = arguments;
        }

        #region FunctionName

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("方法名称"), LabelWidth(70), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("模型要求调用的函数名称")]
        private string _functionName;

        /// <summary>
        /// 模型要求调用的函数名称
        /// </summary>

        [JsonProperty("name")]
        public string FunctionName => _functionName;

        #endregion

        #region FunctionIndex

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("函数索引"), LabelWidth(70), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("模型函数的索引")]
        private int _functionIndex;

        /// <summary>
        /// 函数索引
        /// </summary>

        [JsonProperty("index")]
        public int FunctionIndex => _functionIndex;

        #endregion

        #region Arguments

#if ODIN_INSPECTOR
        [ShowInInspector, LabelText("参数"), ReadOnly]
#else
        [SerializeField]
#endif
        [TextArea, Tooltip("函数入参")]
        private string _arguments;

        /// <summary>
        /// 函数入参
        /// </summary>

        public string Arguments
        {
            get => _arguments;
            private set => _arguments = value;
        }

        #endregion

        public void AppendArgument(string arg)
        {
            Arguments += arg;
        }
    }
}