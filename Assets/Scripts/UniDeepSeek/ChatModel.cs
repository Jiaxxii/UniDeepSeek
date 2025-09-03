namespace Xiyu.UniDeepSeek
{
    public enum ChatModel
    {
        /// <summary>
        /// 不进行深度思考，使用 v3 模型。
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = "deepseek-chat")]
        Chat,

        /// <summary>
        /// 深度思考 R1 模型，在回答前会给出一段思维链。
        /// </summary>
        [System.Runtime.Serialization.EnumMember(Value = "deepseek-reasoner")]
        Reasoner
    }
}