using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    // DEEPSEEK-R1 编写
    public class DependencyManager : EditorWindow
    {
        private static readonly (string name, string version) UniTaskManifest = 
            ("com.cysharp.unitask", 
                "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");
    
        private static readonly (string name, string version) NewtonsoftJsonManifest = 
            ("com.unity.nuget.newtonsoft-json", "3.2.1");
    
        private Vector2 scrollPosition;
        private bool uniTaskInstalled;
        private bool newtonsoftJsonInstalled;
        private static bool hasCheckedOnLoad = false;

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            EditorApplication.delayCall += () => 
            {
                // 延迟执行，确保Unity完全加载
                if (!hasCheckedOnLoad)
                {
                    CheckDependenciesAutomatically();
                    hasCheckedOnLoad = true;
                }
            };
        }

        private static void CheckDependenciesAutomatically()
        {
            string manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
            string manifestContent = File.ReadAllText(manifestPath);

            bool uniTaskMissing = !manifestContent.Contains(UniTaskManifest.name);
            bool newtonsoftJsonMissing = !manifestContent.Contains(NewtonsoftJsonManifest.name);

            if (uniTaskMissing || newtonsoftJsonMissing)
            {
                // 如果有缺失的依赖，弹出窗口
                ShowWindow();
            
                // 也可以显示通知
                // ShowNotification("项目缺少必要的依赖包，请查看Dependency Manager窗口");
            }
        }

        [MenuItem("Tools/Dependency Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<DependencyManager>("Dependency Manager");
            window.minSize = new Vector2(400, 200);
            window.CheckDependencies();
        }

        private void OnEnable()
        {
            CheckDependencies();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("依赖包检查", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("此工具会自动检查项目所需的依赖包，缺失的包会显示为红色。", MessageType.Info);
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // UniTask 状态显示
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("UniTask", GUILayout.Width(150));
            GUI.color = uniTaskInstalled ? Color.green : Color.red;
            EditorGUILayout.LabelField(uniTaskInstalled ? "已安装" : "未安装", GUILayout.Width(100));
            GUI.color = Color.white;
        
            if (!uniTaskInstalled)
            {
                if (GUILayout.Button("安装 UniTask"))
                {
                    if (InstallPackage(UniTaskManifest.name, UniTaskManifest.version))
                    {
                        CheckDependencies();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // Newtonsoft Json 状态显示
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Newtonsoft Json", GUILayout.Width(150));
            GUI.color = newtonsoftJsonInstalled ? Color.green : Color.red;
            EditorGUILayout.LabelField(newtonsoftJsonInstalled ? "已安装" : "未安装", GUILayout.Width(100));
            GUI.color = Color.white;
        
            if (!newtonsoftJsonInstalled)
            {
                if (GUILayout.Button("安装 Newtonsoft Json"))
                {
                    if (InstallPackage(NewtonsoftJsonManifest.name, NewtonsoftJsonManifest.version))
                    {
                        CheckDependencies();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        
            // 一键安装所有缺失包
            if (!uniTaskInstalled || !newtonsoftJsonInstalled)
            {
                if (GUILayout.Button("安装所有缺失依赖", GUILayout.Height(30)))
                {
                    bool installedAny = false;
                
                    if (!uniTaskInstalled)
                    {
                        installedAny |= InstallPackage(UniTaskManifest.name, UniTaskManifest.version);
                    }
                
                    if (!newtonsoftJsonInstalled)
                    {
                        installedAny |= InstallPackage(NewtonsoftJsonManifest.name, NewtonsoftJsonManifest.version);
                    }
                
                    if (installedAny)
                    {
                        AssetDatabase.Refresh();
                        CheckDependencies();
                        ShowNotification("依赖包安装完成！");
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("所有依赖包均已安装！", MessageType.Info);
            }

            EditorGUILayout.EndScrollView();
        
            // 刷新按钮
            if (GUILayout.Button("刷新状态"))
            {
                CheckDependencies();
            }
        }

        private void CheckDependencies()
        {
            string manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
            string manifestContent = File.ReadAllText(manifestPath);

            uniTaskInstalled = manifestContent.Contains(UniTaskManifest.name);
            newtonsoftJsonInstalled = manifestContent.Contains(NewtonsoftJsonManifest.name);
        }

        private static bool InstallPackage(string packageName, string versionOrUrl)
        {
            try
            {
                // 使用Unity的PackageManager.Client.Add方法安装包
                UnityEditor.PackageManager.Client.Add($"{packageName}@{versionOrUrl}");
                Debug.Log($"开始安装 {packageName}...");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"安装包 {packageName} 时出错: {e.Message}");
                EditorUtility.DisplayDialog("安装错误", $"安装包 {packageName} 时出错: {e.Message}", "确定");
                return false;
            }
        }

        private void ShowNotification(string message)
        {
            this.ShowNotification(new GUIContent(message));
            EditorApplication.update += () =>
            {
                // 2秒后移除通知
                EditorApplication.delayCall += () =>
                {
                    this.RemoveNotification();
                    EditorApplication.update = null;
                };
            };
        }
    }
}