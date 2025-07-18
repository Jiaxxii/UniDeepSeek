using UnityEditor;
using UnityEditor.Build;

namespace Editor
{
    [InitializeOnLoad]
    public class RiderSymbolDefiner
    {
        private const string RiderSymbol = "RIDER";

        static RiderSymbolDefiner()
        {
            // 每当我们回到编辑器时检查
            EditorApplication.delayCall += CheckAndSetSymbol;
        }

        private static void CheckAndSetSymbol()
        {
            // 检查当前编辑器是否为Rider
            var isRider = IsRiderEditor();

            // 获取当前编译符号
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup));
            var hasSymbol = defines.Contains(RiderSymbol);

            // 仅在需要时更新
            if (isRider && !hasSymbol)
            {
                PlayerSettings.SetScriptingDefineSymbols(
                    NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup),
                    string.IsNullOrEmpty(defines) ? RiderSymbol : $"{defines};{RiderSymbol}");
            }
            else if (!isRider && hasSymbol)
            {
                PlayerSettings.SetScriptingDefineSymbols(
                    NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup),
                    defines.Replace($"{RiderSymbol};", "").Replace($";{RiderSymbol}", "").Replace(RiderSymbol, ""));
            }
        }

        private static bool IsRiderEditor()
        {
            // 更可靠的Rider检测方法
            var externalEditor = EditorPrefs.GetString("kScriptsDefaultApp");
            return externalEditor.EndsWith("rider64.exe") ||
                   externalEditor.EndsWith("Rider.exe") ||
                   externalEditor.Contains("Rider");
        }
    }
}