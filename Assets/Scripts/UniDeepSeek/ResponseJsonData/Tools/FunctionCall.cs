using System;
using UnityEngine;
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
            _id = id;
            _type = type;
            _functionArg = function;
        }

        #region Id

#if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup("info"), LabelWidth(20), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("tool 调用的 ID。")]
        private string _id;

        /// <summary>
        /// tool 调用的 ID。
        /// </summary>

        public string Id => _id;

        #endregion

        #region Type

#if ODIN_INSPECTOR
        [ShowInInspector, HorizontalGroup("info"), LabelWidth(35), ReadOnly]
#else
        [SerializeField]
#endif
        [Tooltip("tool 的类型，目前仅支持 function。")]
        private string _type;

        /// <summary>
        /// tool 的类型，目前仅支持 function。
        /// </summary>

        public string Type => _type;

        #endregion

        #region Function

#if ODIN_INSPECTOR
        [ShowInInspector, InlineProperty, HideLabel]
#else
        [SerializeField]
#endif
        [Tooltip("模型调用的函数参数。")]
        private FunctionArg _functionArg;

        /// <summary>
        /// 模型调用的函数参数。
        /// </summary>

        public FunctionArg Function => _functionArg;

        #endregion
    }
}