namespace Xiyu.UniDeepSeek.MessagesType
{
    [System.Serializable]
    public class UserMessage : Message
    {
        public override RoleType Role => RoleType.User;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        [Newtonsoft.Json.JsonProperty("name")]
        public string UserName { get; set; }


        public override ParamsStandardError VerifyParams()
        {
            var verify = base.VerifyParams();

            if (verify != ParamsStandardError.Success)
            {
                return verify;
            }

            // 赋值为null以避免JSON序列化无意义的空字符串
            if (string.IsNullOrWhiteSpace(UserName)) UserName = null;
            return ParamsStandardError.Success;
        }
    }
}