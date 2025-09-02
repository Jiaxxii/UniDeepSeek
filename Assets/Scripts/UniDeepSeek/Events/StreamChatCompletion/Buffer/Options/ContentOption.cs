namespace Xiyu.UniDeepSeek.Events.StreamChatCompletion.Buffer
{
    public readonly struct ContentOption
    {
        public ContentOption(int flushThreshold, ContentFlushCriteriaOption flushCriteriaOption)
        {
            FlushThreshold = flushThreshold;
            FlushCriteriaOption = flushCriteriaOption;
        }

        public int FlushThreshold { get; }
        public ContentFlushCriteriaOption FlushCriteriaOption { get; }


        public static ContentOption Default => new(10, ContentFlushCriteriaOption.ByCharacterCount);
    }
}