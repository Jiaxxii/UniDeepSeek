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
#endif
        private Xiyu.UniDeepSeek.FillInTheMiddle.FimChatCompletion _fimChatCompletion;

        private bool _isRunning;

        private void Start()
        {
            FimChatCompletionAsync().Forget();
        }

        [SerializeField] [TextArea(3, 5)] private string prompt;
        [SerializeField] [TextArea(3, 5)] private string suffix;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button("启动", DrawResult = false)]
#endif
        private async UniTaskVoid FimChatCompletionAsync()
        {
            if (_isRunning)
            {
                Debug.LogWarning("正在运行中，请等待。");
                return;
            }

            _isRunning = true;

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

            if (Application.isPlaying)
                textComponent.text = _fimChatCompletion.Choices[0].Text;
            else Debug.Log(_fimChatCompletion.Choices[0].Text);

            _isRunning = false;
        }
    }
}