namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public interface IBuilding
    {
        DeepSeekChat Build(IApiKeyConverter apiKeyConverter);
        DeepSeekChat Build(string apiKeyConverter);
        
    }
}