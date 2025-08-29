using Newtonsoft.Json;

namespace Xiyu.UniDeepSeek.MessagesType
{
    [System.Serializable]
    public class SystemMessage : Message
    {
#if UNITY_EDITOR && ODIN_INSPECTOR // 供序列化使用
        public
#else
        private
#endif
            SystemMessage()
        {
        }

        public SystemMessage(string content, string userName = null)
        {
            Content = content;
            UserName = userName;
        }

#if UNITY_EDITOR && !ODIN_INSPECTOR
        [UnityEngine.SerializeField] private RoleType role = RoleType.System;
        private void ForgetWaring() => _ = role;
#endif
        public override RoleType Role => RoleType.System;

        #region UserName

        [UnityEngine.SerializeField] private string userName;

        [JsonProperty("name")]
        public string UserName
        {
            get => userName;
            set => userName = value;
        }

        #endregion


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