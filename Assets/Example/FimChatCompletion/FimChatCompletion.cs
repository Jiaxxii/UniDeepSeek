using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Xiyu.UniDeepSeek.FillInTheMiddle;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Example.FimChatCompletion
{
    public class FimChatCompletion : MonoBehaviour
    {
        [SerializeField] private Text textComponent;

#if ODIN_INSPECTOR
        [ShowInInspector, /*InlineProperty,*/ HideLabel]
        private Xiyu.UniDeepSeek.FillInTheMiddle.FimChatCompletion _fimChatCompletion;
#endif


        private void Start()
        {
#if ODIN_INSPECTOR
            Debug.Log("使用按钮进行测试。");
#else
            FimChatCompletionAsync().Forget();
#endif
        }

        [SerializeField] [TextArea(3, 5)] private string prompt;
        [SerializeField] [TextArea(3, 5)] private string suffix;

#if ODIN_INSPECTOR
        [Button("FimChatCompletionAsync")]
#endif
        private async UniTaskVoid FimChatCompletionAsync()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Please run in play mode.");
                return;
            }

            // 使用你自己的 API Key
            var apiKey = Resources.Load<TextAsset>("DeepSeek-ApiKey").text;
            var fimDeepSeekChat = new FimDeepSeekChat(apiKey)
            {
                Setting =
                {
                    Echo = true
                }
            };


            _fimChatCompletion = await fimDeepSeekChat.ChatCompletionAsync(prompt, suffix, this.GetCancellationTokenOnDestroy());


            textComponent.text = _fimChatCompletion.Choices[0].Text;
        }
    }
}