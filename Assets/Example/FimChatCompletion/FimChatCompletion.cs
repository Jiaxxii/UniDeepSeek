using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.FillInTheMiddle;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Example.FimChatCompletion
{
    public class FimChatCompletion : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textComponent;


        [SerializeField] private FimRequestParameter fimRequestParameter;

#if ODIN_INSPECTOR
        [ShowIf("@fimChatCompletion.Id != \"\"")]
#endif
        [SerializeField]
        private Xiyu.UniDeepSeek.FillInTheMiddle.FimChatCompletion fimChatCompletion;

        private FimDeepSeekChat _fimDeepSeekChat;

        private void Start()
        {
            _fimDeepSeekChat = new FimDeepSeekChat(Resources.Load<TextAsset>("DeepSeek-ApiKey").text);
            _fimDeepSeekChat.Setting = fimRequestParameter;
            StartForget(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid StartForget(CancellationToken cancellationToken)
        {
            const string prefix = "// Swap the i-th and j-th elements of array arr.\r\n" +
                                  "private void Swap<T>(T[] arr, int i, int j)\r\n" +
                                  "{";

            const string suffix = "}";

            textComponent.text = prefix;

            var chatCompletion = await _fimDeepSeekChat.ChatCompletionAsync(prefix, suffix, cancellationToken);

            textComponent.text += chatCompletion.GetMessage().Text + suffix;

            fimChatCompletion = chatCompletion;
        }
    }
}