using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UGF.GameFramework.UI.Editor;

namespace UGF.GameFramework.UI.Editor
{
    /// <summary>
    /// UIDesigner相关的菜单项
    /// </summary>
    public static class UIDesignerMenuItems
    {
        /// <summary>
        /// 为当前选中的GameObject添加UIDesigner组件
        /// </summary>
        [MenuItem("GameObject/UI/Add UIDesigner", false, 10)]
        public static void AddUIDesigner()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个GameObject", "确定");
                return;
            }
            
            // 检查是否已经有UIDesigner组件
            UIDesigner existingDesigner = selectedObject.GetComponent<UIDesigner>();
            if (existingDesigner != null)
            {
                EditorUtility.DisplayDialog("提示", "该GameObject已经有UIDesigner组件", "确定");
                Selection.activeObject = existingDesigner;
                return;
            }
            
            // 添加UIDesigner组件
            UIDesigner designer = selectedObject.AddComponent<UIDesigner>();
            
            // 设置默认值
            designer.UIFormName = selectedObject.name;
            designer.NamespaceName = "Game.UI";
            
            // 选中新添加的组件
            Selection.activeObject = designer;
            
            Debug.Log($"已为 {selectedObject.name} 添加UIDesigner组件");
        }
        
        /// <summary>
        /// 验证菜单项是否可用
        /// </summary>
        [MenuItem("GameObject/UI/Add UIDesigner", true)]
        public static bool ValidateAddUIDesigner()
        {
            return Selection.activeGameObject != null;
        }
        
        /// <summary>
        /// 生成当前选中UIDesigner的代码
        /// </summary>
        [MenuItem("UGF/GameFramework/UI Designer/Generate Code for Selected")]
        public static void GenerateCodeForSelected()
        {
            UIDesigner designer = GetSelectedUIDesigner();
            if (designer == null)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个包含UIDesigner组件的GameObject", "确定");
                return;
            }
            
            try
            {
                UICodeGenerator.GenerateCode(designer);
                EditorUtility.DisplayDialog("成功", $"已为 {designer.UIFormName} 生成代码", "确定");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("错误", $"代码生成失败：{ex.Message}", "确定");
                Debug.LogError($"代码生成失败：{ex}");
            }
        }
        
        /// <summary>
        /// 验证生成代码菜单项是否可用
        /// </summary>
        [MenuItem("UGF/GameFramework/UI Designer/Generate Code for Selected", true)]
        public static bool ValidateGenerateCodeForSelected()
        {
            return GetSelectedUIDesigner() != null;
        }
        
        /// <summary>
        /// 生成场景中所有UIDesigner的代码
        /// </summary>
        [MenuItem("UGF/GameFramework/UI Designer/Generate All Code in Scene")]
        public static void GenerateAllCodeInScene()
        {
            UIDesigner[] designers = Object.FindObjectsOfType<UIDesigner>();
            
            if (designers.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "场景中没有找到UIDesigner组件", "确定");
                return;
            }
            
            int successCount = 0;
            int failCount = 0;
            
            foreach (var designer in designers)
            {
                if (!designer.AutoGenerateCode)
                {
                    continue;
                }
                
                try
                {
                    UICodeGenerator.GenerateCode(designer);
                    successCount++;
                }
                catch (System.Exception ex)
                {
                    failCount++;
                    Debug.LogError($"为 {designer.UIFormName} 生成代码失败：{ex}");
                }
            }
            
            string message = $"代码生成完成！\n成功：{successCount} 个\n失败：{failCount} 个";
            EditorUtility.DisplayDialog("完成", message, "确定");
        }
        
        /// <summary>
        /// 生成项目中所有UIDesigner的代码
        /// </summary>
        [MenuItem("UGF/GameFramework/UI Designer/Generate All Code in Project")]
        public static void GenerateAllCodeInProject()
        {
            // 查找项目中所有的预制体
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            
            int successCount = 0;
            int failCount = 0;
            int totalCount = 0;
            
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    UIDesigner designer = prefab.GetComponent<UIDesigner>();
                    if (designer != null && designer.AutoGenerateCode)
                    {
                        totalCount++;
                        
                        try
                        {
                            UICodeGenerator.GenerateCode(designer);
                            successCount++;
                        }
                        catch (System.Exception ex)
                        {
                            failCount++;
                            Debug.LogError($"为预制体 {prefab.name} 生成代码失败：{ex}");
                        }
                    }
                }
            }
            
            if (totalCount == 0)
            {
                EditorUtility.DisplayDialog("提示", "项目中没有找到启用自动生成代码的UIDesigner组件", "确定");
                return;
            }
            
            string message = $"项目代码生成完成！\n" +
                           $"总计：{totalCount} 个\n" +
                           $"成功：{successCount} 个\n" +
                           $"失败：{failCount} 个";
            EditorUtility.DisplayDialog("完成", message, "确定");
        }
        
        /// <summary>
        /// 清理生成的代码文件
        /// </summary>
        [MenuItem("UGF/GameFramework/UI Designer/Clean Generated Code")]
        public static void CleanGeneratedCode()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "确认清理", 
                "这将删除所有自动生成的代码文件（*.Generated.cs）\n此操作不可撤销，确定继续吗？", 
                "确定", 
                "取消");
            
            if (!confirmed)
            {
                return;
            }
            
            // 查找所有.Generated.cs文件
            string[] generatedFiles = AssetDatabase.FindAssets("*.Generated t:TextAsset");
            int deletedCount = 0;
            
            foreach (string guid in generatedFiles)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".Generated.cs"))
                {
                    AssetDatabase.DeleteAsset(path);
                    deletedCount++;
                }
            }
            
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("完成", $"已删除 {deletedCount} 个生成的代码文件", "确定");
        }
        
        /// <summary>
        /// 打开UIDesigner文档
        /// </summary>
        [MenuItem("UGF/GameFramework/UI Designer/Open Documentation")]
        public static void OpenDocumentation()
        {
            string docPath = "Assets/UGF.GameFramework.UI/Documentation/UIDesigner_README.md";
            
            if (AssetDatabase.LoadAssetAtPath<TextAsset>(docPath) != null)
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<TextAsset>(docPath));
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "找不到文档文件", "确定");
            }
        }
        
        /// <summary>
        /// 获取当前选中的UIDesigner组件
        /// </summary>
        private static UIDesigner GetSelectedUIDesigner()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                return null;
            }
            
            return selectedObject.GetComponent<UIDesigner>();
        }
        
        /// <summary>
        /// 创建UI预制体模板
        /// </summary>
        [MenuItem("Assets/Create/UI/UI Form Template")]
        public static void CreateUIFormTemplate()
        {
            // 创建根GameObject
            GameObject uiRoot = new GameObject("NewUIForm");
            
            // 添加Canvas组件（如果需要）
            Canvas canvas = uiRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 添加CanvasScaler
            CanvasScaler scaler = uiRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // 添加GraphicRaycaster
            uiRoot.AddComponent<GraphicRaycaster>();
            
            // 添加UIDesigner组件
            UIDesigner designer = uiRoot.AddComponent<UIDesigner>();
            designer.UIFormName = "NewUIForm";
            designer.NamespaceName = "Game.UI";
            
            // 设置为预制体
            string path = EditorUtility.SaveFilePanelInProject(
                "保存UI预制体", 
                "NewUIForm", 
                "prefab", 
                "选择保存位置");
            
            if (!string.IsNullOrEmpty(path))
            {
                PrefabUtility.SaveAsPrefabAsset(uiRoot, path);
                AssetDatabase.Refresh();
                
                // 选中创建的预制体
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            
            // 删除场景中的临时对象
            Object.DestroyImmediate(uiRoot);
        }
    }
}