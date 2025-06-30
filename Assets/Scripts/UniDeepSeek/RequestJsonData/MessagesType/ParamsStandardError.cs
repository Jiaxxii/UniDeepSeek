namespace Xiyu.UniDeepSeek.MessagesType
{
    public enum ParamsStandardError
    {
        None = 0,

        ContentInvalid = 1,
        UserMessagesMissing = 2,
        Success = 10,

        // Tool Errors
        ToolCallInvalid = 100,
        JsonInvalidFormat = 101,
        JsonMissingProperty = 102,

        // Function Errors
        FunctionNameByNull = 200,
        
        // Fim
        PromptEmpty
    }
}