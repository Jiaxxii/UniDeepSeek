﻿using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if RIDER
using JetBrains.Annotations;
#else
using Xiyu.UniDeepSeek.Annotations;
#endif

namespace Xiyu.UniDeepSeek.MessagesType
{
    [System.Serializable]
    public class AssistantMessage : Message
    {
#if UNITY_EDITOR && ODIN_INSPECTOR // 供序列化使用
        public
#else
        private
#endif
            AssistantMessage()
        {
        }

#if UNITY_EDITOR && !ODIN_INSPECTOR
        [UnityEngine.SerializeField] private RoleType role = RoleType.Assistant;
        private void ForgetWaring() => _ = role;
#endif
        public override RoleType Role => RoleType.Assistant;

        #region NAME

#if ODIN_INSPECTOR
        [ShowInInspector, ReadOnly]
#else
        [SerializeField]
#endif
        private string _name;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        #endregion

        #region Prefix

#if ODIN_INSPECTOR
        [ShowInInspector]
#else
        [SerializeField]
#endif
        private bool _prefix;

        public bool Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }

        #endregion

        #region ReasoningContent

#if ODIN_INSPECTOR
        [ShowInInspector]
#else
        [SerializeField]
#endif
        [TextArea(10, 20)]
        private string _reasoningContent;

        public string ReasoningContent
        {
            get => _reasoningContent;
            set => _reasoningContent = value;
        }

        #endregion


        public override ParamsStandardError VerifyParams()
        {
            var verifyParams = base.VerifyParams();

            if (verifyParams != ParamsStandardError.Success)
            {
                return verifyParams;
            }

            // 赋值为null以避免JSON序列化无意义的空字符串
            if (string.IsNullOrWhiteSpace(Name)) Name = null;

            if (!Prefix && ReasoningContent != null)
            {
                ReasoningContent = null;
            }

            return ParamsStandardError.Success;
        }

        public static AssistantMessage CreateMessage(string content, string name = null)
        {
            return new AssistantMessage { Content = content, Name = name };
        }

        public static AssistantMessage CreateMessage(string content, string reasoningContent, string name)
        {
            return new AssistantMessage { Content = content, ReasoningContent = reasoningContent, Name = name };
        }

        /// <summary>
        /// 创建带有前缀的消息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssistantMessage CreatePrefixMessage(string content, string name = null)
        {
            return new AssistantMessage { Content = content, Name = name, Prefix = true };
        }

        /// <summary>
        /// 创建带有前缀的消息 (深度思考模型)
        /// </summary>
        /// <param name="reasoningContent"></param>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssistantMessage CreateReasoningPrefixMessage(string reasoningContent, [NotNull] string content = "", string name = null)
        {
            return new AssistantMessage { Content = content, Name = name, Prefix = true, ReasoningContent = reasoningContent };
        }
    }
}