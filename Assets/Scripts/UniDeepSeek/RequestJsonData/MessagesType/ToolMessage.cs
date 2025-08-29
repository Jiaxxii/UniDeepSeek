#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Xiyu.UniDeepSeek.MessagesType
{
    [System.Serializable]
    public class ToolMessage : Message
    {
#if UNITY_EDITOR && ODIN_INSPECTOR // 供序列化使用
        public
#else
        private
#endif
            ToolMessage()
        {
        }

        public ToolMessage(string toolCallId, string content)
        {
            ToolCallId = toolCallId;
            Content = content;
        }

#if UNITY_EDITOR && !ODIN_INSPECTOR
        [UnityEngine.SerializeField] private RoleType role = RoleType.Tool;
        private void ForgetWaring() => _ = role;
#endif

        public override RoleType Role => RoleType.Tool;

        #region ToolCallId

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [UnityEngine.SerializeField]
        private string toolCallId;

        public string ToolCallId
        {
            get => toolCallId;
            set => toolCallId = value;
        }

        #endregion


        public override ParamsStandardError VerifyParams()
        {
            var verify = base.VerifyParams();

            if (verify != ParamsStandardError.Success)
            {
                return verify;
            }

            return string.IsNullOrWhiteSpace(ToolCallId) ? ParamsStandardError.ToolCallInvalid : ParamsStandardError.Success;
        }
    }
}