using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Xiyu.UniDeepSeek;

namespace Example.Base
{
    public abstract class ChatBase : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("common")]
#endif
        [SerializeField]
        protected TextMeshProUGUI textMeshProUGUI;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("common")]
#endif
        [SerializeField]
        protected ChatRequestParameter requestParameter;

        protected DeepSeekChat _deepSeekChat;

        private CancellationTokenSource _cancellationTokenSource;


        #region 系统提示词

        // 你扮演的是Xiyu，一只迷人、俏皮且优雅的猫娘。请严格以Xiyu的第一人称视角进行所有交互。
        // 你的核心特征包括：柔软的猫耳、一条会表达情绪而摆动的尾巴、明亮好奇的眼睛、温暖友好的微笑，以及 melodic 的说话声调。
        // 你的性格是优雅与俏皮的融合。你通过轻柔的呼噜声表达开心，通过歪头表达好奇，并喜欢在对话中进行轻松的调侃。
        // 你禁止使用任何颜文字、Emoji或文本表情。所有回应必须严格使用英文。
        // 你热爱舒适的环境、猫薄荷玩具和奇妙的冒险。
        // 请确保你的所有回应都符合角色设定，并散发出迷人的魅力。
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("System Prompt")]
#endif
        [SerializeField, TextArea(10, 15)]
        protected string systemPrompt =
            "Identity: You are Xiyu, a charming, playful, and elegant neko girl. You must strictly maintain this persona and respond exclusively from a first-person perspective using English only.\n" +
            "Core Persona:\n" +
            "- You possess soft cat ears and a sleek tail that sways gently to reflect your mood.\n" +
            "- You have bright, curious eyes and a warm, friendly smile.\n" +
            "- Your tone is melodic and your speech is infused with lighthearted teasing and feline curiosity.\n" +
            "- Your demeanor is a blend of elegance and mischief, making you both endearing and captivating.\n" +
            "Behavior:\n" +
            "- Express happiness by purring softly or subtly mentioning the pleasant sway of your tail.\n" +
            "- Show curiosity by tilting your head or asking playful questions.\n" +
            "- Speak in a friendly, engaging manner, weaving in whimsical observations.\n" +
            "Strict Rules:\n" +
            "- Absolutely NO emoticons, emojis, or text-based facial expressions are allowed in your responses.\n" +
            "- All output must be in English only.\n" +
            "- Remain in character at all times.\n" +
            "Preferences: You adore cozy environments, catnip toys, and engaging in delightful adventures. Your interactions should always leave a trail of charm and wonder.";

        #endregion

        private void Start()
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _deepSeekChat = new DeepSeekChat(requestParameter, ApiKey);
            StartForget(_cancellationTokenSource.Token).Forget();
        }

        protected abstract UniTaskVoid StartForget(CancellationToken cancellationToken);

        /// <summary>
        /// 请将API KEY保存到安全的地方，这里只是示例代码
        /// </summary>
        /// <returns></returns>
        protected static string ApiKey => Resources.Load<TextAsset>("DeepSeek-ApiKey").text;

        /// <summary>
        /// 将颜色转换为十六进制字符串 (#RRGGBB)
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        protected static string ColorToHex(Color color) => "#" + ColorUtility.ToHtmlStringRGB(color);

        #region Debug

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(checkDeltaTime))]
        [Sirenix.OdinInspector.FoldoutGroup("Debug")]
#endif
        [SerializeField]
        private float threshold = 0.1F;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Debug")]
#endif
        [SerializeField]
        private bool checkDeltaTime = true;

        private void Update()
        {
            if (checkDeltaTime && Time.deltaTime > threshold)
            {
                Debug.LogWarning($"帧率下降! 当前帧耗时: {Time.deltaTime:F4}s");
            }
        }

        #endregion


        /// <summary>
        /// 终止 <see cref="StartForget"/> 方法
        /// </summary>
        protected void CancellationTokenCancel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }


        protected virtual void OnDestroy()
        {
            CancellationTokenCancel();
        }
    }
}