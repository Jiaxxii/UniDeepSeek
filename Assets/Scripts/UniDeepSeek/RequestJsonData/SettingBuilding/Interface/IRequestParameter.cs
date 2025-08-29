using System.Collections.Generic;

namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public interface IRequestParameter : IBuilding
    {
        IRequestParameter SetModel(ChatModel model);
        IRequestParameter SetFrequencyPenalty(float penalty);
        IRequestParameter SetLogprobs(bool logprobs);

        IRequestParameter SetMaxTokens(int maxTokens);

        IMessage Message { get; }

        IRequestParameter SetPresencePenalty(float presencePenalty);
        IRequestParameter SetResponseFormat(ResponseFormatType responseFormat);

        IRequestParameter SetStopKeywords(IEnumerable<string> stopSequences);
        IRequestParameter AppendStopKeywords(IEnumerable<string> stopSequences);
        IRequestParameter SetStreamIncludedUsage(bool streamIncludedUsage);

        IRequestParameter SetTemperature(float temperature);

        ITool Tool { get; }

        IRequestParameter SetTopP(float topP);
        IRequestParameter SetTopLogprobs(int topLogprobs);
    }
}