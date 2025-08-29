using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
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
            functionName = name;
            this.arguments = arguments;
        }

        #region FunctionName

#if ODIN_INSPECTOR
        [LabelText("方法名称"), LabelWidth(70), ReadOnly]
#endif
        [SerializeField, Tooltip("模型要求调用的函数名称")]
        private string functionName;

        /// <summary>
        /// 模型要求调用的函数名称
        /// </summary>

        [JsonProperty("name")]
        public string FunctionName => functionName;

        #endregion

        #region FunctionIndex

#if ODIN_INSPECTOR
        [LabelText("函数索引"), LabelWidth(70), ReadOnly]
#endif
        [SerializeField, Tooltip("模型函数的索引")]
        private int functionIndex;

        /// <summary>
        /// 函数索引
        /// </summary>

        [JsonProperty("index")]
        public int FunctionIndex => functionIndex;

        #endregion

        #region Arguments

#if ODIN_INSPECTOR
        [LabelText("参数"), ReadOnly]
#endif
        [SerializeField, TextArea, Tooltip("函数入参")]
        private string arguments;

        /// <summary>
        /// 函数入参
        /// </summary>

        public string Arguments
        {
            get => arguments;
            private set => arguments = value;
        }

        #endregion

        public void AppendArgument(string arg)
        {
            Arguments += arg;
        }
    }
}