namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public interface IToolChoice
    {
        // 不调用工具
        ITool SetNone();

        // 自动选择工具（可不调用）
        ITool SetAuto();

        // 必须调用一个工具
        IToolInstances SetRequired();

        // 必须调用指定工具
        IToolInstances SetFunction();
    }
}