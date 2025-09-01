using UnityEngine;
using UnityEditor;
using UGF.GameFramework.UI;

namespace UGF.GameFramework.UI.Editor
{
    /// <summary>
    /// UIComponentBinder菜单项
    /// </summary>
    public static class UIComponentBinderMenuItems
    {
        /// <summary>
        /// 添加UIComponentBinder组件
        /// </summary>
        [MenuItem("GameObject/UI/Add Component Binder", false, 10)]
        public static void AddComponentBinder()
        {
            var selectedObjects = Selection.gameObjects;
            
            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个GameObject。", "确定");
                return;
            }
            
            foreach (var obj in selectedObjects)
            {
                // 检查是否已经有UIComponentBinder组件
                if (obj.GetComponent<UIComponentBinder>() != null)
                {
                    Debug.LogWarning($"GameObject '{obj.name}' 已经有UIComponentBinder组件。");
                    continue;
                }
                
                // 添加组件
                var binder = obj.AddComponent<UIComponentBinder>();
                
                // 自动更新组件信息
                binder.UpdateComponentInfo();
                
                // 标记为已修改
                EditorUtility.SetDirty(obj);
                
                Debug.Log($"已为GameObject '{obj.name}' 添加UIComponentBinder组件。");
            }
        }
        
        /// <summary>
        /// 批量添加UIComponentBinder组件
        /// </summary>
        [MenuItem("GameObject/UI/Batch Add Component Binders", false, 11)]
        public static void BatchAddComponentBinders()
        {
            var selectedObjects = Selection.gameObjects;
            
            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个或多个GameObject。", "确定");
                return;
            }
            
            var addedCount = 0;
            var skippedCount = 0;
            
            foreach (var obj in selectedObjects)
            {
                // 递归处理所有子对象
                ProcessGameObjectRecursively(obj, ref addedCount, ref skippedCount);
            }
            
            var message = $"批量添加完成！\n添加: {addedCount} 个\n跳过: {skippedCount} 个";
            EditorUtility.DisplayDialog("批量添加结果", message, "确定");
        }
        
        /// <summary>
        /// 移除UIComponentBinder组件
        /// </summary>
        [MenuItem("GameObject/UI/Remove Component Binder", false, 12)]
        public static void RemoveComponentBinder()
        {
            var selectedObjects = Selection.gameObjects;
            
            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("错误", "请先选择一个GameObject。", "确定");
                return;
            }
            
            var removedCount = 0;
            
            foreach (var obj in selectedObjects)
            {
                var binder = obj.GetComponent<UIComponentBinder>();
                if (binder != null)
                {
                    Object.DestroyImmediate(binder);
                    EditorUtility.SetDirty(obj);
                    removedCount++;
                    Debug.Log($"已从GameObject '{obj.name}' 移除UIComponentBinder组件。");
                }
            }
            
            if (removedCount == 0)
            {
                EditorUtility.DisplayDialog("提示", "选中的GameObject中没有找到UIComponentBinder组件。", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("移除完成", $"已移除 {removedCount} 个UIComponentBinder组件。", "确定");
            }
        }
        

        
        /// <summary>
        /// 递归处理GameObject
        /// </summary>
        /// <param name="obj">GameObject</param>
        /// <param name="addedCount">添加计数</param>
        /// <param name="skippedCount">跳过计数</param>
        private static void ProcessGameObjectRecursively(GameObject obj, ref int addedCount, ref int skippedCount)
        {
            // 检查当前对象是否需要添加组件
            if (ShouldAddBinder(obj))
            {
                if (obj.GetComponent<UIComponentBinder>() == null)
                {
                    var binder = obj.AddComponent<UIComponentBinder>();
                    binder.UpdateComponentInfo();
                    EditorUtility.SetDirty(obj);
                    addedCount++;
                }
                else
                {
                    skippedCount++;
                }
            }
            
            // 递归处理子对象
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                ProcessGameObjectRecursively(obj.transform.GetChild(i).gameObject, ref addedCount, ref skippedCount);
            }
        }
        
        /// <summary>
        /// 判断是否应该添加绑定器
        /// </summary>
        /// <param name="obj">GameObject</param>
        /// <returns>是否应该添加</returns>
        private static bool ShouldAddBinder(GameObject obj)
        {
            // 检查是否有常见的UI组件
            return obj.GetComponent<UnityEngine.UI.Button>() != null ||
                   obj.GetComponent<UnityEngine.UI.Toggle>() != null ||
                   obj.GetComponent<UnityEngine.UI.Text>() != null ||
                   obj.GetComponent<UnityEngine.UI.Image>() != null ||
                   obj.GetComponent<UnityEngine.UI.InputField>() != null ||
                   obj.GetComponent<UnityEngine.UI.Slider>() != null ||
                   obj.GetComponent<UnityEngine.UI.Scrollbar>() != null ||
                   obj.GetComponent<UnityEngine.UI.Dropdown>() != null ||
                   obj.GetComponent<TMPro.TextMeshProUGUI>() != null ||
                   obj.GetComponent<TMPro.TMP_InputField>() != null ||
                   obj.GetComponent<TMPro.TMP_Dropdown>() != null;
        }
        
        /// <summary>
        /// 查找窗体根节点
        /// </summary>
        /// <param name="current">当前节点</param>
        /// <returns>窗体根节点</returns>
        private static Transform FindFormRoot(Transform current)
        {
            while (current != null)
            {
                if (current.GetComponent<UIFormBase>() != null || current.GetComponent<Canvas>() != null)
                {
                    return current;
                }
                current = current.parent;
            }
            return null;
        }
        
        /// <summary>
        /// 验证菜单项是否可用
        /// </summary>
        /// <returns>是否可用</returns>
        [MenuItem("GameObject/UI/Add Component Binder", true)]
        [MenuItem("GameObject/UI/Batch Add Component Binders", true)]
        [MenuItem("GameObject/UI/Remove Component Binder", true)]
        [MenuItem("GameObject/UI/Validate Component Bindings", true)]
        public static bool ValidateMenuItems()
        {
            return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
        }
    }
}