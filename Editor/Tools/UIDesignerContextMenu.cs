// UIDesignerContextMenu.cs
// UI设计器右键菜单功能

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UGF.GameFramework.UI.Editor
{
    /// <summary>
    /// UI设计器右键菜单
    /// </summary>
    public static class UIDesignerContextMenu
    {
        /// <summary>
        /// 在Inspector组件右键菜单添加"设计"选项
        /// </summary>
        [MenuItem("CONTEXT/Component/设计", false, 1000)]
        public static void AddUIDesigner(MenuCommand command)
        {
            Component component = command.context as Component;
            if (component == null)
                return;

            GameObject gameObject = component.gameObject;
            
            // 检查是否已经有UIDesigner组件
            UIDesigner existingDesigner = gameObject.GetComponent<UIDesigner>();
            if (existingDesigner != null)
            {
                Debug.LogWarning($"GameObject {gameObject.name} 已经有UIDesigner组件");
                Selection.activeObject = existingDesigner;
                return;
            }

            // 添加UIDesigner组件
            UIDesigner designer = gameObject.AddComponent<UIDesigner>();
            
            // 尝试从现有UI脚本还原设计状态
            RestoreDesignFromExistingScript(designer);
            
            // 选中新添加的UIDesigner组件
            Selection.activeObject = designer;
            
            Debug.Log($"已为 {gameObject.name} 添加UIDesigner组件");
        }
        
        /// <summary>
        /// 验证菜单项是否可用
        /// </summary>
        [MenuItem("CONTEXT/Component/设计", true)]
        public static bool ValidateAddUIDesigner(MenuCommand command)
        {
            Component component = command.context as Component;
            if (component == null)
                return false;
                
            // 只对UI相关组件显示菜单
            return component is RectTransform || 
                   component is Canvas || 
                   component is CanvasGroup ||
                   component is Graphic ||
                   component is Selectable;
        }
        
        /// <summary>
        /// 从现有UI脚本还原设计状态
        /// </summary>
        private static void RestoreDesignFromExistingScript(UIDesigner designer)
        {
            try
            {
                GameObject gameObject = designer.gameObject;
                
                // 查找可能的UI脚本组件
                Component[] components = gameObject.GetComponents<Component>();
                Component uiScriptComponent = null;
                System.Type uiScriptType = null;
                
                Debug.Log($"开始扫描 {gameObject.name} 上的组件，共找到 {components.Length} 个组件");
                
                foreach (var comp in components)
                {
                    if (comp == null || comp is Transform || comp is RectTransform || 
                        comp is UIDesigner || comp is UIComponentBinder)
                        continue;
                        
                    System.Type compType = comp.GetType();
                    Debug.Log($"检查组件：{compType.Name} (命名空间：{compType.Namespace})");
                    
                    // 检查是否是自动生成的UI脚本（继承自UIFormBase或包含特定命名模式）
                    if (IsUIScript(compType))
                    {
                        uiScriptComponent = comp;
                        uiScriptType = compType;
                        Debug.Log($"找到UI脚本组件：{compType.FullName}");
                        break;
                    }
                    else
                    {
                        Debug.Log($"组件 {compType.Name} 不是UI脚本类型");
                    }
                }
                
                if (uiScriptComponent != null && uiScriptType != null)
                {
                    Debug.Log($"找到UI脚本组件：{uiScriptType.Name}，开始还原设计状态");
                    
                    // 设置UIDesigner的基本信息
                    designer.UIFormName = uiScriptType.Name;
                    designer.NamespaceName = uiScriptType.Namespace ?? "Game.UI";
                    
                    // 直接从UI脚本组件实例获取已绑定的组件引用
                    List<UIComponentBinding> bindings = ExtractComponentBindingsFromScript(uiScriptComponent, uiScriptType);
                    designer.ComponentBindings = bindings;
                    
                    // 为相关GameObject添加UIComponentBinder标记
                    AddComponentBindersToGameObjects(bindings);
                    
                    Debug.Log($"成功还原设计状态，找到 {bindings.Count} 个组件绑定");
                }
                else
                {
                    Debug.Log("未找到现有UI脚本，将创建新的设计");
                    
                    // 设置默认值
                    designer.UIFormName = gameObject.name.Replace(" ", "").Replace("(", "").Replace(")", "");
                    if (!designer.UIFormName.EndsWith("UI"))
                        designer.UIFormName += "UI";
                        
                    designer.NamespaceName = "Game.UI";
                    designer.ComponentBindings = new List<UIComponentBinding>();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"还原设计状态时发生错误：{ex.Message}\n{ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 判断是否是UI脚本类型
        /// </summary>
        private static bool IsUIScript(System.Type type)
        {
            // 检查是否继承自UIFormBase
            System.Type baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.Name == "UIFormBase" || baseType.FullName == "UGF.GameFramework.UI.UIFormBase")
                    return true;
                baseType = baseType.BaseType;
            }
            
            // 检查命名模式（以UI结尾且在UI相关命名空间下）
            if (type.Name.EndsWith("UI") || type.Name.EndsWith("Form"))
            {
                if (type.Namespace?.Contains("UI") == true || 
                    type.Namespace?.Contains("Game") == true ||
                    type.Namespace?.Contains("Examples") == true)
                    return true;
            }
                
            return false;
        }
        
        /// <summary>
        /// 直接从UI脚本组件实例提取已绑定的组件引用
        /// </summary>
        private static List<UIComponentBinding> ExtractComponentBindingsFromScript(Component scriptComponent, System.Type scriptType)
        {
            List<UIComponentBinding> bindings = new List<UIComponentBinding>();
            
            try
            {
                Debug.Log($"[ExtractComponentBindingsFromScript] 开始从脚本实例提取组件绑定：{scriptType.FullName}");
                
                // 获取所有字段（包括私有字段）
                FieldInfo[] fields = scriptType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                Debug.Log($"[ExtractComponentBindingsFromScript] 找到 {fields.Length} 个字段");
                
                foreach (FieldInfo field in fields)
                {
                    Debug.Log($"[ExtractComponentBindingsFromScript] 检查字段：{field.Name}，类型：{field.FieldType.FullName}");
                    
                    // 检查是否是UI组件类型
                    if (IsUIComponentType(field.FieldType))
                    {
                        Debug.Log($"[ExtractComponentBindingsFromScript] 字段 {field.Name} 是UI组件类型：{field.FieldType.Name}");
                        
                        // 直接从脚本实例获取字段值（已绑定的组件引用）
                        Component fieldValue = field.GetValue(scriptComponent) as Component;
                        if (fieldValue != null)
                        {
                            Debug.Log($"[ExtractComponentBindingsFromScript] 字段 {field.Name} 已绑定到组件：{fieldValue.name} ({fieldValue.GetType().Name})");
                            
                            UIComponentBinding binding = new UIComponentBinding
                            {
                                ComponentName = field.Name,
                                ComponentType = field.FieldType.Name,
                                Component = fieldValue
                            };
                            
                            bindings.Add(binding);
                            Debug.Log($"[ExtractComponentBindingsFromScript] 成功提取绑定：{field.Name} -> {fieldValue.name} ({fieldValue.GetType().Name})");
                        }
                        else
                        {
                            Debug.LogWarning($"[ExtractComponentBindingsFromScript] 字段 {field.Name} 的值为null，可能未绑定组件");
                            
                            // 如果字段值为null，尝试通过名称查找作为备用方案
                            GameObject targetObject = FindGameObjectByFieldName(scriptComponent.gameObject, field.Name);
                            if (targetObject != null)
                            {
                                Component targetComponent = targetObject.GetComponent(field.FieldType);
                                if (targetComponent != null)
                                {
                                    UIComponentBinding binding = new UIComponentBinding
                                    {
                                        ComponentName = field.Name,
                                        ComponentType = field.FieldType.Name,
                                        Component = targetComponent
                                    };
                                    
                                    bindings.Add(binding);
                                    Debug.Log($"[ExtractComponentBindingsFromScript] 备用方案成功：{field.Name} -> {targetObject.name} ({field.FieldType.Name})");
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"[ExtractComponentBindingsFromScript] 字段 {field.Name} 不是UI组件类型");
                    }
                }
                
                Debug.Log($"[ExtractComponentBindingsFromScript] 提取完成，共找到 {bindings.Count} 个组件绑定");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"从脚本实例提取组件绑定时发生错误：{ex.Message}\n{ex.StackTrace}");
            }
            
            return bindings;
        }
        
        /// <summary>
        /// 解析脚本字段信息
        /// </summary>
        private static List<UIComponentBinding> ParseScriptFields(System.Type scriptType, GameObject rootObject)
        {
            List<UIComponentBinding> bindings = new List<UIComponentBinding>();
            
            try
            {
                // 获取所有字段
                FieldInfo[] fields = scriptType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                Debug.Log($"[ParseScriptFields] 脚本类型：{scriptType.FullName}，找到 {fields.Length} 个字段");
                
                foreach (var field in fields)
                {
                    Debug.Log($"[ParseScriptFields] 检查字段：{field.Name}，类型：{field.FieldType.FullName}");
                    
                    // 跳过非UI组件字段
                    if (!IsUIComponentType(field.FieldType))
                    {
                        Debug.Log($"[ParseScriptFields] 字段 {field.Name} 不是UI组件类型，跳过");
                        continue;
                    }
                    
                    Debug.Log($"[ParseScriptFields] 字段 {field.Name} 是UI组件类型，开始查找对应GameObject");
                        
                    // 尝试在GameObject层次结构中找到对应的组件
                    GameObject targetObject = FindGameObjectByFieldName(rootObject, field.Name);
                    if (targetObject != null)
                    {
                        Debug.Log($"[ParseScriptFields] 找到对应GameObject：{targetObject.name}");
                        Component targetComponent = targetObject.GetComponent(field.FieldType);
                        if (targetComponent != null)
                        {
                            UIComponentBinding binding = new UIComponentBinding
                            {
                                ComponentName = field.Name,
                                ComponentType = field.FieldType.Name,
                                Component = targetComponent
                            };
                            
                            bindings.Add(binding);
                            Debug.Log($"[ParseScriptFields] 成功创建绑定：{field.Name} -> {targetObject.name} ({field.FieldType.Name})");
                        }
                        else
                        {
                            Debug.LogWarning($"[ParseScriptFields] GameObject {targetObject.name} 上没有找到 {field.FieldType.Name} 组件");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[ParseScriptFields] 没有找到字段 {field.Name} 对应的GameObject");
                    }
                }
                
                Debug.Log($"[ParseScriptFields] 解析完成，共找到 {bindings.Count} 个组件绑定");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"解析脚本字段时发生错误：{ex.Message}\n{ex.StackTrace}");
            }
            
            return bindings;
        }
        
        /// <summary>
        /// 判断是否是UI组件类型
        /// </summary>
        private static bool IsUIComponentType(System.Type type)
        {
            // 检查是否继承自Component
            if (!typeof(Component).IsAssignableFrom(type))
            {
                Debug.Log($"[IsUIComponentType] {type.FullName} 不继承自Component");
                return false;
            }
            
            // 添加更多UI组件类型的检查
            bool isUIType = false;
            
            // 检查是否继承自Graphic（Text、Image、RawImage等）
            if (typeof(UnityEngine.UI.Graphic).IsAssignableFrom(type))
            {
                isUIType = true;
                Debug.Log($"[IsUIComponentType] {type.FullName} 继承自Graphic");
            }
            // 检查是否继承自Selectable（Button、Toggle、Slider、InputField等）
            else if (typeof(UnityEngine.UI.Selectable).IsAssignableFrom(type))
            {
                isUIType = true;
                Debug.Log($"[IsUIComponentType] {type.FullName} 继承自Selectable");
            }
            // 检查特定的UI组件类型
            else if (type == typeof(RectTransform) ||
                     type == typeof(Transform) ||
                     type == typeof(CanvasGroup) ||
                     type == typeof(Canvas) ||
                     type.FullName == "TMPro.TextMeshProUGUI" ||
                     type.FullName == "TMPro.TMP_InputField" ||
                     type.FullName == "TMPro.TMP_Dropdown")
            {
                isUIType = true;
                Debug.Log($"[IsUIComponentType] {type.FullName} 是特定UI组件类型");
            }
            else
            {
                Debug.Log($"[IsUIComponentType] {type.FullName} 不是UI组件类型");
            }
                           
            Debug.Log($"[IsUIComponentType] 最终结果：{type.FullName} 是UI组件类型：{isUIType}");
            return isUIType;
        }
        
        /// <summary>
        /// 根据字段名查找GameObject
        /// </summary>
        private static GameObject FindGameObjectByFieldName(GameObject root, string fieldName)
        {
            Debug.Log($"[FindGameObjectByFieldName] 开始查找字段 '{fieldName}' 对应的GameObject，根对象：{root.name}");
            
            // 生成多种可能的名称变体
            List<string> nameVariants = GenerateNameVariants(fieldName);
            Debug.Log($"[FindGameObjectByFieldName] 生成的名称变体：{string.Join(", ", nameVariants)}");
            
            // 首先尝试直接子对象匹配
            foreach (string variant in nameVariants)
            {
                Debug.Log($"[FindGameObjectByFieldName] 尝试直接匹配：'{variant}'");
                Transform found = root.transform.Find(variant);
                if (found != null)
                {
                    Debug.Log($"[FindGameObjectByFieldName] 直接匹配成功：找到 {found.name}");
                    return found.gameObject;
                }
            }
                
            // 递归搜索所有子对象
            Debug.Log($"[FindGameObjectByFieldName] 开始递归搜索所有子对象");
            GameObject result = FindGameObjectRecursive(root.transform, nameVariants);
            
            if (result != null)
            {
                Debug.Log($"[FindGameObjectByFieldName] 递归搜索成功：找到 {result.name}");
            }
            else
            {
                Debug.LogWarning($"[FindGameObjectByFieldName] 未找到字段 '{fieldName}' 对应的GameObject");
                // 输出所有子对象的名称以便调试
                Debug.Log($"[FindGameObjectByFieldName] 根对象 '{root.name}' 的所有子对象：");
                LogAllChildren(root.transform, 0);
            }
            
            return result;
        }
        
        /// <summary>
        /// 生成字段名的各种可能变体
        /// </summary>
        private static List<string> GenerateNameVariants(string fieldName)
        {
            List<string> variants = new List<string>();
            
            // 原始名称
            variants.Add(fieldName);
            
            // 移除常见前缀（btn, img, txt, lbl等）
            if (fieldName.Length > 3)
            {
                string prefix = fieldName.Substring(0, 3).ToLower();
                if (prefix == "btn" || prefix == "img" || prefix == "txt" || prefix == "lbl")
                {
                    string withoutPrefix = fieldName.Substring(3);
                    variants.Add(withoutPrefix);
                    
                    // 首字母小写版本
                    if (withoutPrefix.Length > 0)
                    {
                        variants.Add(char.ToLower(withoutPrefix[0]) + withoutPrefix.Substring(1));
                    }
                }
            }
            
            // 移除常见后缀（Button, Text, Image等）
            string[] suffixes = { "Button", "Text", "Image", "Label", "Input", "Toggle", "Slider" };
            foreach (string suffix in suffixes)
            {
                if (fieldName.EndsWith(suffix, System.StringComparison.OrdinalIgnoreCase))
                {
                    string withoutSuffix = fieldName.Substring(0, fieldName.Length - suffix.Length);
                    if (!string.IsNullOrEmpty(withoutSuffix))
                    {
                        variants.Add(withoutSuffix);
                    }
                }
            }
            
            // 首字母大写版本
            if (fieldName.Length > 0)
            {
                variants.Add(char.ToUpper(fieldName[0]) + fieldName.Substring(1));
            }
            
            // 全小写版本
            variants.Add(fieldName.ToLower());
            
            // 移除重复项
            return variants.Distinct().ToList();
        }
        
        /// <summary>
        /// 递归查找GameObject
        /// </summary>
        private static GameObject FindGameObjectRecursive(Transform parent, List<string> nameVariants)
        {
            foreach (Transform child in parent)
            {
                Debug.Log($"[FindGameObjectRecursive] 检查子对象：'{child.name}'");
                
                // 检查是否匹配任何一个名称变体
                foreach (string variant in nameVariants)
                {
                    if (child.name.Equals(variant, System.StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log($"[FindGameObjectRecursive] 匹配成功：'{child.name}' 匹配变体 '{variant}'");
                        return child.gameObject;
                    }
                }
                
                // 递归搜索子对象
                GameObject found = FindGameObjectRecursive(child, nameVariants);
                if (found != null)
                    return found;
            }
            
            return null;
        }
        
        /// <summary>
        /// 输出所有子对象的名称（用于调试）
        /// </summary>
        private static void LogAllChildren(Transform parent, int depth)
        {
            string indent = new string(' ', depth * 2);
            foreach (Transform child in parent)
            {
                Debug.Log($"[LogAllChildren] {indent}- {child.name}");
                if (depth < 3) // 限制深度避免过多输出
                {
                    LogAllChildren(child, depth + 1);
                }
            }
        }
        
        /// <summary>
        /// 为GameObject添加UIComponentBinder标记
        /// </summary>
        private static void AddComponentBindersToGameObjects(List<UIComponentBinding> bindings)
        {
            foreach (var binding in bindings)
            {
                if (binding.Component != null)
                {
                    UIComponentBinder binder = binding.Component.GetComponent<UIComponentBinder>();
                    if (binder == null)
                        binder = binding.Component.gameObject.AddComponent<UIComponentBinder>();
                    Debug.Log($"为 {binding.Component.name} 添加UIComponentBinder标记");
                }
            }
        }
    }
}