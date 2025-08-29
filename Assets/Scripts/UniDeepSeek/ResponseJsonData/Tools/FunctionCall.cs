using System;
using UnityEngine;
using UnityEngine.Serialization;
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
            this.id = id;
            this.type = type;
            functionArg = function;
        }

        #region Id

#if ODIN_INSPECTOR
        [HorizontalGroup("info"), LabelWidth(20), ReadOnly]
#endif
        [SerializeField, Tooltip("tool 调用的 ID。")]
        private string id;

        /// <summary>
        /// tool 调用的 ID。
        /// </summary>

        public string Id => id;

        #endregion

        #region Type

#if ODIN_INSPECTOR
        [HorizontalGroup("info"), LabelWidth(35), ReadOnly]
#endif
        [SerializeField, Tooltip("tool 的类型，目前仅支持 function。")]
        private string type;

        /// <summary>
        /// tool 的类型，目前仅支持 function。
        /// </summary>

        public string Type => type;

        #endregion

        #region Function

#if ODIN_INSPECTOR
        [InlineProperty, HideLabel]
#endif
        [SerializeField, Tooltip("模型调用的函数参数。")]
        private FunctionArg functionArg;

        /// <summary>
        /// 模型调用的函数参数。
        /// </summary>

        public FunctionArg Function => functionArg;

        #endregion
    }
}