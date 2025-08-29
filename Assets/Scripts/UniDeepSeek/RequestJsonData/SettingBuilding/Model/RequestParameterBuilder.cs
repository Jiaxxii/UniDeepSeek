using System.Collections.Generic;

namespace Xiyu.UniDeepSeek.SettingBuilding
{
    public class RequestParameterBuilder : IRequestParameter
    {
        private readonly ChatRequestParameter _requestParameter;

        public RequestParameterBuilder(ChatRequestParameter requestParameter = null, IMessage message = null, ITool tool = null)
        {
            _requestParameter = requestParameter ?? new ChatRequestParameter();
            Message = message ?? new MessageBuilder(this, _requestParameter.Messages);
            Tool = tool ?? new ToolConfigurator(this, _requestParameter.ToolInstances, _requestParameter.ToolChoice);
        }
  

        public IRequestParameter SetModel(ChatModel model)
        {
            _requestParameter.Model = model;
            return this;
        }

        public IRequestParameter SetFrequencyPenalty(float penalty)
        {
            _requestParameter.FrequencyPenalty = penalty;
            return this;
        }

        public IRequestParameter SetLogprobs(bool logprobs)
        {
            _requestParameter.Logprobs = logprobs;
            return this;
        }

        public IRequestParameter SetMaxTokens(int maxTokens)
        {
            _requestParameter.MaxTokens = maxTokens;
            return this;
        }

        public IMessage Message { get; set; }

        public IRequestParameter SetPresencePenalty(float presencePenalty)
        {
            _requestParameter.PresencePenalty = presencePenalty;
            return this;
        }

        public IRequestParameter SetResponseFormat(ResponseFormatType responseFormat)
        {
            _requestParameter.ResponseFormat = responseFormat;
            return this;
        }

        public IRequestParameter SetStopKeywords(IEnumerable<string> stopSequences)
        {
            _requestParameter.Stop.Clear();
            foreach (var keyword in stopSequences)
            {
                _requestParameter.Stop.Add(keyword);
            }

            return this;
        }

        public IRequestParameter AppendStopKeywords(IEnumerable<string> stopSequences)
        {
            foreach (var keyword in stopSequences)
            {
                _requestParameter.Stop.Add(keyword);
            }

            return this;
        }

        public IRequestParameter SetStreamIncludedUsage(bool streamIncludedUsage)
        {
            _requestParameter.StreamIncludedUsage = streamIncludedUsage;
            return this;
        }

        public IRequestParameter SetTemperature(float temperature)
        {
            _requestParameter.Temperature = temperature;
            return this;
        }

        public ITool Tool { get; }

        public IRequestParameter SetTopP(float topP)
        {
            _requestParameter.TopP = topP;
            return this;
        }

        public IRequestParameter SetTopLogprobs(int topLogprobs)
        {
            _requestParameter.TopLogprobs = topLogprobs;
            return this;
        }

        public DeepSeekChat Build(string apiKey)
        {
            return new DeepSeekChat(_requestParameter, apiKey);
        }

        public DeepSeekChat Build(IApiKeyConverter apiKeyConverter)
        {
            return new DeepSeekChat(_requestParameter, apiKeyConverter);
        }
    }
}