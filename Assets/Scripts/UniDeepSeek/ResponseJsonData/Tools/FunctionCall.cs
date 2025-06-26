using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


namespace Xiyu.UniDeepSeek.Tools
{
    [Serializable]
#if ODIN_INSPECTOR
    [HideReferenceObjectPicker]
#endif
    public class FunctionCall
    {
        public FunctionCall(string id, FunctionArg function = null, string type = "function")
        {
            Id = id;
            Type = type;
            Function = function;
        }

        /// <summary>
        /// tool 调用的 ID。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup("info"), LabelWidth(20)]
        [UnityEngine.Tooltip("tool 调用的 ID。")]
#endif
        public string Id { get; }

        /// <summary>
        /// tool 的类型，目前仅支持 function。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup("info"), LabelWidth(35)]
        [UnityEngine.Tooltip("tool 的类型，目前仅支持 function。")]
#endif
        public string Type { get; }

        /// <summary>
        /// 模型调用的函数参数。
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector, InlineProperty, HideLabel]
        [UnityEngine.Tooltip("模型调用的函数参数。")]
#endif
        public FunctionArg Function { get; }
    }
}