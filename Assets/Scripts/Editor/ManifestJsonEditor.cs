using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class ManifestJsonEditor
    {
        private static readonly (string name, string version) UniTaskManifest =
            ("com.cysharp.unitask",
                "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");

        private static readonly (string name, string version) NewtonsoftJsonManifest =
            ("com.unity.nuget.newtonsoft-json", "3.2.1");

        [MenuItem("Tools/Xiyu/添加 UniTask 和 Newtonsoft.Json 依赖")]
        public static void AddDependencies()
        {
            const string manifestPath = "Packages/manifest.json";
            var manifestJson = File.ReadAllText(manifestPath);
            var modified = false;

            // 检查并添加 UniTask
            if (!ContainsDependency(manifestJson, UniTaskManifest.name))
            {
                manifestJson = AddDependency(manifestJson, UniTaskManifest.name, UniTaskManifest.version);
                modified = true;
                Debug.Log("<color=yellow>添加了</color><color=#d85b50>UniTask</color><color=yellow>依赖</color>");
            }

            // 检查并添加 Newtonsoft.Json
            if (!ContainsDependency(manifestJson, NewtonsoftJsonManifest.name))
            {
                manifestJson = AddDependency(manifestJson, NewtonsoftJsonManifest.name, NewtonsoftJsonManifest.version);
                modified = true;
                Debug.Log("<color=yellow>添加了</color><color=#d85b50>Newtonsoft.Json</color><color=yellow>依赖</color>");
            }

            if (modified)
            {
                // 修复可能的JSON格式问题
                manifestJson = FixJsonFormat(manifestJson);
                File.WriteAllText(manifestPath, manifestJson);
                AssetDatabase.Refresh();
                Debug.Log("<color=#98c379>Manifest.json 已成功更新</color>");
            }
            else
            {
                Debug.Log("<color=yellow>已经包含了</color><color=#d85b50>UniTask</color><color=yellow>和</color><color=#d85b50>Newtonsoft.Json</color><color=yellow>依赖</color>");
            }
        }

        private static bool ContainsDependency(string manifestJson, string packageName)
        {
            // 使用带引号的包名进行匹配
            return Regex.IsMatch(manifestJson, $@"\""{packageName}\""\s*:");
        }

        private static string AddDependency(string manifestJson, string packageName, string version)
        {
            // 1. 找到dependencies对象开始位置
            int dependenciesIndex = manifestJson.IndexOf("\"dependencies\"", StringComparison.Ordinal);
            if (dependenciesIndex == -1)
            {
                // 如果没有dependencies节点，创建新的
                int lastBrace = manifestJson.LastIndexOf('}');
                return manifestJson.Insert(lastBrace,
                    $",\n  \"dependencies\": {{\n    \"{packageName}\": \"{version}\"\n  }}");
            }

            // 2. 找到dependencies对象的大括号开始位置
            int startBraceIndex = manifestJson.IndexOf('{', dependenciesIndex);
            if (startBraceIndex == -1) return manifestJson;

            // 3. 找到dependencies对象结束位置
            int endBraceIndex = FindMatchingBrace(manifestJson, startBraceIndex);
            if (endBraceIndex == -1) return manifestJson;

            // 4. 在结束大括号前插入新依赖
            string newDependency = $"\"{packageName}\": \"{version}\"";

            // 检查是否需要前置逗号
            char lastChar = manifestJson[endBraceIndex - 1];
            if (lastChar != '{' && lastChar != ',' && lastChar != '\n')
            {
                newDependency = ",\n    " + newDependency;
            }
            else
            {
                newDependency = "\n    " + newDependency;
            }

            return manifestJson.Insert(endBraceIndex, newDependency);
        }

        private static int FindMatchingBrace(string json, int startIndex)
        {
            int depth = 1;
            for (int i = startIndex + 1; i < json.Length; i++)
            {
                if (json[i] == '{') depth++;
                else if (json[i] == '}') depth--;

                if (depth == 0) return i;
            }

            return -1;
        }

        private static string FixJsonFormat(string json)
        {
            // 修复尾随逗号问题
            json = Regex.Replace(json, @",(\s*})", "$1");

            // 添加缺失的逗号
            json = Regex.Replace(json, @"(\""\w+\""\s*:\s*\""[^\""]+\"")(\s*\""\w+\""\s*:)", "$1,$2");

            return json;
        }
    }
}