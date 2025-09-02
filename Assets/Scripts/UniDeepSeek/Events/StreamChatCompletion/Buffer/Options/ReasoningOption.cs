namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Buffer
{
    public readonly struct ReasoningOption
    {
        public ReasoningOption(string colorHex, int reasoningNewlineCount, int flushThreshold, ContentFlushCriteriaOption flushCriteriaOption)
        {
            ColorHex = colorHex;
            ReasoningNewlineCount = reasoningNewlineCount;
            FlushThreshold = flushThreshold;
            FlushCriteriaOption = flushCriteriaOption;
        }

        public string ColorHex { get; }
        public int ReasoningNewlineCount { get; }
        public int FlushThreshold { get; }
        public ContentFlushCriteriaOption FlushCriteriaOption { get; }

        public static ReasoningOption Default { get; } = new(null, 1, 10, ContentFlushCriteriaOption.ByCharacterCount);


        public static implicit operator ContentOption(ReasoningOption option)
        {
            return new ContentOption(option.FlushThreshold, option.FlushCriteriaOption);
        }
    }
}