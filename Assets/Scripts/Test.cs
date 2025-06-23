using Newtonsoft.Json;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;
using Xiyu.UniDeepSeek;
using Xiyu.UniDeepSeek.MessagesType;


#if ODIN_INSPECTOR
public class Test : SerializedMonoBehaviour
#else
public class Test : MonoBehaviour
#endif
{
    [SerializeField]
#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
    private ChatRequestParameter chatRequestParameter;

#if ODIN_INSPECTOR
    [Button("测试Chat参数")]
#endif
    private void PrintChatRequestParameter()
    {
        var verifyResult = chatRequestParameter.VerifyParams();

        if (verifyResult != ParamsStandardError.Success)
        {
            Debug.LogError($"【<color=red>{verifyResult}</color>】验证参数失败！(code:{(int)verifyResult})");
            return;
        }

        // 序列化并打印参数
        var json = chatRequestParameter.FromObjectAsToken().ToString(Formatting.None);
        Debug.Log(json);
    }
}