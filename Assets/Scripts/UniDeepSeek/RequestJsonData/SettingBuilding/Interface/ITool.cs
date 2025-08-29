namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public interface ITool : IThenableMessage
    {
        IToolInstances ToolInstances { get; }
        IToolChoice ToolChoice { get; }
    }
}