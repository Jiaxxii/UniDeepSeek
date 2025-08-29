namespace Xiyu.UniDeepSeek.MessagesType
{
    [System.Serializable]
    public class UserMessage : Message
    {
        public UserMessage(string content, string userName = null)
        {
            Content = content;
            UserName = userName;
        }

#if UNITY_EDITOR && ODIN_INSPECTOR // 供序列化使用
        public
#else
        private
#endif
            UserMessage()
        {
        }

#if UNITY_EDITOR && !ODIN_INSPECTOR
        [UnityEngine.SerializeField] private RoleType role = RoleType.User;
        private void ForgetWaring() => _ = role;
#endif

        public override RoleType Role => RoleType.User;

        [UnityEngine.SerializeField] private string userName;

        [Newtonsoft.Json.JsonProperty("name")] public string UserName { get; set; }


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