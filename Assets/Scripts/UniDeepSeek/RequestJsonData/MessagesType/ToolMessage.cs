namespace Xiyu.UniDeepSeek.MessagesType
{
    [System.Serializable]
    public class ToolMessage : Message
    {
        public override RoleType Role => RoleType.Tool;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
        [Sirenix.OdinInspector.ReadOnly]
#endif
        public string ToolCallId { get; set; }


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