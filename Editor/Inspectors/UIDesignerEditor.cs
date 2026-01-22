using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using UGF.GameFramework.UI.Editor;
using UnityEditor.Compilation;

namespace UGF.GameFramework.UI.Editor
{
    /// <summary>
    /// UIDesigner的Inspector编辑器
    /// </summary>
    [CustomEditor(typeof(UIDesigner))]
    public class UIDesignerEditor : UnityEditor.Editor
    {
        private UIDesigner designer;
        private SerializedProperty uiFormNameProp;
        private SerializedProperty namespaceNameProp;
        private SerializedProperty autoGenerateCodeProp;
        private SerializedProperty componentBindingsProp;
        
        private bool showComponentBindings = true;
        private bool showAdvancedSettings = false;
        private ReorderableList componentBindingsList;
        
        // 简化的两步式工作流程状态
        private bool _isCodeGenerated = false;

        private void OnEnable()
        {
            // 检查target对象是否为null
            if (target == null)
            {
                return;
            }
            
            // 检查serializedObject是否有效
            try
            {
                if (serializedObject == null || serializedObject.targetObject == null)
                {
                    return;
                }
            }
            catch (System.Exception)
            {
                // serializedObject访问失败，直接返回
                return;
            }
            
            designer = (UIDesigner)target;
            uiFormNameProp = serializedObject.FindProperty("uiFormName");
            namespaceNameProp = serializedObject.FindProperty("namespaceName");
            autoGenerateCodeProp = serializedObject.FindProperty("autoGenerateCode");
            componentBindingsProp = serializedObject.FindProperty("componentBindings");

            // 初始化ReorderableList
            componentBindingsList = new ReorderableList(serializedObject, componentBindingsProp, true, true, false, true)
            {
                drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, $"组件绑定列表 ({componentBindingsProp.arraySize})");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    DrawComponentBindingItemInList(rect, index);
                },
                elementHeightCallback = (int index) =>
                {
                    return GetComponentBindingItemHeight(index);
                },
                onRemoveCallback = (ReorderableList list) =>
                {
                    if (EditorUtility.DisplayDialog("确认", "确定要删除这个组件绑定吗？", "确定", "取消"))
                    {
                        ReorderableList.defaultBehaviours.DoRemoveButton(list);
                    }
                }
            };
            
            // 初始化代码生成状态
            InitializeCodeGenerationState();
        }
        
        /// <summary>
        /// 初始化代码生成状态
        /// </summary>
        private void InitializeCodeGenerationState()
        {
            if (designer == null || string.IsNullOrEmpty(designer.UIFormName))
            {
                _isCodeGenerated = false;
                Debug.Log("[UIDesignerEditor] 代码生成状态初始化：false（UIDesigner或UIFormName为空）");
                return;
            }
            
            // 使用固定的输出目录路径检测文件是否存在
            try
            {
                var settings = UIDesignerSettings.Instance;
                string projectPath = System.IO.Path.GetDirectoryName(Application.dataPath);
                string bindingDirectory = System.IO.Path.Combine(projectPath, settings.bindingGeneratePath);
                string logicDirectory = System.IO.Path.Combine(projectPath, settings.logicGeneratePath);
                
                string bindingFilePath = System.IO.Path.Combine(bindingDirectory, $"{designer.UIFormName}.Generated.cs");
                string logicFilePath = System.IO.Path.Combine(logicDirectory, $"{designer.UIFormName}.cs");
                
                bool bindingExists = System.IO.File.Exists(bindingFilePath);
                bool logicExists = System.IO.File.Exists(logicFilePath);
                
                _isCodeGenerated = bindingExists && logicExists;
                
                Debug.Log($"[UIDesignerEditor] 代码生成状态初始化：{_isCodeGenerated}（绑定类：{bindingExists}，逻辑类：{logicExists}）");
                Debug.Log($"[UIDesignerEditor] 检查路径 - 绑定类：{bindingFilePath}，逻辑类：{logicFilePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[UIDesignerEditor] 初始化代码生成状态失败: {ex.Message}");
                _isCodeGenerated = false;
            }
        }

        public override void OnInspectorGUI()
        {
            // 检查target对象是否为null，避免SerializedObjectNotCreatableException
            if (target == null)
            {
                EditorGUILayout.HelpBox("UIDesigner组件已被删除或无效", MessageType.Warning);
                return;
            }
            
            // 检查serializedObject是否有效
            try
            {
                if (serializedObject == null || serializedObject.targetObject == null)
                {
                    EditorGUILayout.HelpBox("序列化对象无效，请重新选择对象", MessageType.Warning);
                    return;
                }
                serializedObject.Update();
            }
            catch (System.Exception ex)
            {
                EditorGUILayout.HelpBox($"序列化对象访问失败: {ex.Message}", MessageType.Error);
                return;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("UI设计器", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 基本设置
            DrawBasicSettings();
            
            EditorGUILayout.Space();
            
            // 组件绑定列表
            DrawComponentBindings();
            
            EditorGUILayout.Space();
            
            // 高级设置
            DrawAdvancedSettings();
            
            EditorGUILayout.Space();
            
            // 操作按钮
            DrawActionButtons();
            
            // 安全地应用序列化对象的修改
            SafeApplyModifiedProperties();
        }
        
        /// <summary>
        /// 绘制基本设置
        /// </summary>
        private void DrawBasicSettings()
        {
            if (designer == null) return;
            
            EditorGUILayout.LabelField("基本设置", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(uiFormNameProp, new GUIContent("UI界面名称"));
            if (string.IsNullOrEmpty(uiFormNameProp.stringValue))
            {
                EditorGUILayout.HelpBox($"将使用GameObject名称: {designer.gameObject.name}", MessageType.Info);
            }
            
            EditorGUILayout.PropertyField(namespaceNameProp, new GUIContent("命名空间"));
            if (string.IsNullOrEmpty(namespaceNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("将使用默认命名空间: Game.UI", MessageType.Info);
            }
            
            EditorGUILayout.PropertyField(autoGenerateCodeProp, new GUIContent("自动生成代码"));
        }
        
        /// <summary>
        /// 绘制组件绑定列表
        /// </summary>
        private void DrawComponentBindings()
        {
            if (designer == null) return;
            
            showComponentBindings = EditorGUILayout.Foldout(showComponentBindings, $"组件绑定列表 ({componentBindingsProp.arraySize})", true);
            
            if (showComponentBindings)
            {
                EditorGUI.indentLevel++;
                
                // 操作按钮
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("同步组件", "同步所有UIComponentBinder，确保绑定列表与实际组件一致"), GUILayout.Width(120)))
                {
                    AutoScanAllComponents();
                }
                if (GUILayout.Button("清空列表", GUILayout.Width(80)))
                {
                    if (EditorUtility.DisplayDialog("确认", "确定要清空所有组件绑定吗？", "确定", "取消"))
                    {
                        ClearComponentBindings();
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                
                // 使用ReorderableList绘制可拖拽的组件绑定列表
                componentBindingsList.DoLayoutList();
                
                EditorGUI.indentLevel--;
            }
        }
        
        /// <summary>
        /// 绘制单个组件绑定项
        /// </summary>
        private void DrawComponentBindingItem(int index)
        {
            SerializedProperty bindingProp = componentBindingsProp.GetArrayElementAtIndex(index);
            SerializedProperty componentNameProp = bindingProp.FindPropertyRelative("componentName");
            SerializedProperty componentProp = bindingProp.FindPropertyRelative("component");
            SerializedProperty componentTypeProp = bindingProp.FindPropertyRelative("componentType");
            SerializedProperty descriptionProp = bindingProp.FindPropertyRelative("description");
            // fieldNameProp已移除，字段名现在自动生成
            
            EditorGUILayout.BeginVertical("box");
            
            // 标题行
            EditorGUILayout.BeginHorizontal();
            string title = string.IsNullOrEmpty(componentNameProp.stringValue) ? $"组件绑定 {index + 1}" : componentNameProp.stringValue;
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                RemoveComponentBinding(index);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.EndHorizontal();
            
            // 组件设置
            EditorGUILayout.PropertyField(componentNameProp, new GUIContent("变量名称"));
            

            
            // 单个组件绑定（只读显示）
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(componentProp, new GUIContent("绑定组件"));
            EditorGUI.EndDisabledGroup();
            if (componentProp.objectReferenceValue != null)
            {
                // 自动设置组件类型
                Component comp = componentProp.objectReferenceValue as Component;
                if (comp != null)
                {
                    componentTypeProp.stringValue = comp.GetType().Name;
                    
                    // 自动设置变量名称
                    if (string.IsNullOrEmpty(componentNameProp.stringValue))
                    {
                        string autoName = GenerateVariableName(comp);
                        componentNameProp.stringValue = autoName;
                    }
                    
                    // 字段名现在基于组件名自动生成，无需手动设置
                }
            }
            
            // 组件类型不显示，但仍然自动设置值
            
            // 描述可编辑
            EditorGUILayout.PropertyField(descriptionProp, new GUIContent("描述"));
            
            // 验证状态
            if (componentProp.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("请指定要绑定的组件", MessageType.Warning);
            }
            else if (string.IsNullOrEmpty(componentNameProp.stringValue))
            {
                EditorGUILayout.HelpBox("请输入变量名称", MessageType.Warning);
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        /// <summary>
        /// 在ReorderableList中绘制单个组件绑定项
        /// </summary>
        private void DrawComponentBindingItemInList(Rect rect, int index)
        {
            SerializedProperty bindingProp = componentBindingsProp.GetArrayElementAtIndex(index);
            SerializedProperty componentNameProp = bindingProp.FindPropertyRelative("componentName");
            SerializedProperty componentProp = bindingProp.FindPropertyRelative("component");
            SerializedProperty componentTypeProp = bindingProp.FindPropertyRelative("componentType");
            SerializedProperty descriptionProp = bindingProp.FindPropertyRelative("description");
            
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float currentY = rect.y + spacing;
            
            // 标题行
            Rect titleRect = new Rect(rect.x, currentY, rect.width, lineHeight);
            string title = string.IsNullOrEmpty(componentNameProp.stringValue) ? $"组件绑定 {index + 1}" : componentNameProp.stringValue;
            EditorGUI.LabelField(titleRect, title, EditorStyles.boldLabel);
            currentY += lineHeight + spacing;
            
            // 变量名称
            Rect nameRect = new Rect(rect.x, currentY, rect.width, lineHeight);
            EditorGUI.PropertyField(nameRect, componentNameProp, new GUIContent("变量名称"));
            currentY += lineHeight + spacing;
            

            
            // 单个组件绑定（只读显示）
            Rect componentRect = new Rect(rect.x, currentY, rect.width, lineHeight);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(componentRect, componentProp, new GUIContent("绑定组件"));
            EditorGUI.EndDisabledGroup();
            currentY += lineHeight + spacing;
            
            if (componentProp.objectReferenceValue != null)
            {
                Component comp = componentProp.objectReferenceValue as Component;
                if (comp != null)
                {
                    componentTypeProp.stringValue = comp.GetType().Name;
                    if (string.IsNullOrEmpty(componentNameProp.stringValue))
                    {
                        string autoName = GenerateVariableName(comp);
                        componentNameProp.stringValue = autoName;
                    }
                }
            }
            
            // 组件类型不显示，但仍然自动设置值
            
            // 描述
            Rect descRect = new Rect(rect.x, currentY, rect.width, lineHeight);
            EditorGUI.PropertyField(descRect, descriptionProp, new GUIContent("描述"));
            currentY += lineHeight + spacing;
            
            // 验证状态
            bool hasWarning = false;
            string warningMessage = "";
            
            if (componentProp.objectReferenceValue == null)
            {
                hasWarning = true;
                warningMessage = "请指定要绑定的组件";
            }
            else if (string.IsNullOrEmpty(componentNameProp.stringValue))
            {
                hasWarning = true;
                warningMessage = "请输入变量名称";
            }
            
            if (hasWarning)
            {
                Rect warningRect = new Rect(rect.x, currentY, rect.width, lineHeight * 2);
                EditorGUI.HelpBox(warningRect, warningMessage, MessageType.Warning);
            }
        }
        
        /// <summary>
        /// 获取组件绑定项的高度
        /// </summary>
        private float GetComponentBindingItemHeight(int index)
        {
            SerializedProperty bindingProp = componentBindingsProp.GetArrayElementAtIndex(index);
            SerializedProperty componentProp = bindingProp.FindPropertyRelative("component");
            SerializedProperty componentNameProp = bindingProp.FindPropertyRelative("componentName");
            
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float totalHeight = spacing; // 顶部间距
            
            // 标题行
            totalHeight += lineHeight + spacing;
            // 变量名称
            totalHeight += lineHeight + spacing;
            // 单个组件绑定
            totalHeight += lineHeight + spacing;
            // 描述
            totalHeight += lineHeight + spacing;
            
            // 验证状态警告
            bool hasWarning = componentProp.objectReferenceValue == null || string.IsNullOrEmpty(componentNameProp.stringValue);
            
            if (hasWarning)
            {
                totalHeight += lineHeight * 2 + spacing; // 警告框高度
            }
            
            totalHeight += spacing; // 底部间距
            
            return totalHeight;
        }
        
        /// <summary>
        /// 绘制高级设置
        /// </summary>
        private void DrawAdvancedSettings()
        {
            if (designer == null) return;
            
            showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "高级设置", true);
            
            if (showAdvancedSettings)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("生成的类名信息:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"组件绑定类: {designer.GetBindingClassName()}.Generated.cs");
                EditorGUILayout.LabelField($"业务逻辑类: {designer.GetLogicClassName()}.cs");
                
                EditorGUI.indentLevel--;
            }
        }
        
        /// <summary>
        /// 绘制操作按钮
        /// </summary>
        private void DrawActionButtons()
        {
            if (designer == null) return;
            
            EditorGUILayout.LabelField("操作", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            // 两步式工作流程：根据代码生成状态显示不同按钮
            if (!_isCodeGenerated)
            {
                // 第一步：生成代码
                if (GUILayout.Button("生成代码", GUILayout.Height(30)))
                {
                    GenerateCodeStep();
                }
            }
            else
            {
                // 第二步：保存（编译和生成Prefab）
                if (GUILayout.Button("保存", GUILayout.Height(30)))
                {
                    SaveUIDesign();
                }
                
                // 重新生成代码按钮
                if (GUILayout.Button("重新生成代码", GUILayout.Height(30)))
                {
                    RegenerateCode();
                }
            }
            
            if (GUILayout.Button("验证设置", GUILayout.Height(30)))
            {
                ValidateSettings();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 显示当前状态
            if (_isCodeGenerated)
            {
                EditorGUILayout.HelpBox("代码已生成，可以点击'保存'按钮继续后续流程（编译等待和Prefab生成）", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("请先点击'生成代码'按钮生成UI脚本", MessageType.Info);
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("清理设计脚本", GUILayout.Height(25)))
            {
                CleanupDesignScripts();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 安全地应用序列化对象的修改
        /// </summary>
        private void SafeApplyModifiedProperties()
        {
            try
            {
                if (serializedObject != null && serializedObject.targetObject != null)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[UIDesignerEditor] 应用序列化对象修改失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 添加新的组件绑定
        /// </summary>
        private void AddNewComponentBinding()
        {
            componentBindingsProp.arraySize++;
            SafeApplyModifiedProperties();
        }
        
        /// <summary>
        /// 移除组件绑定
        /// </summary>
        private void RemoveComponentBinding(int index)
        {
            componentBindingsProp.DeleteArrayElementAtIndex(index);
            SafeApplyModifiedProperties();
        }
        
        /// <summary>
        /// 清空组件绑定
        /// </summary>
        private void ClearComponentBindings()
        {
            componentBindingsProp.ClearArray();
            SafeApplyModifiedProperties();
        }
        
        /// <summary>
        /// 同步选中的UIComponentBinder组件
        /// </summary>
        private void AutoScanSelectedComponents()
        {
            var selectedObjects = Selection.gameObjects;
            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请先选择要扫描的GameObject", "确定");
                return;
            }
            
            var componentBinders = new List<UIComponentBinder>();
            foreach (var obj in selectedObjects)
            {
                var binder = obj.GetComponent<UIComponentBinder>();
                if (binder != null)
                {
                    componentBinders.Add(binder);
                }
            }
            
            int beforeCount = componentBindingsProp.arraySize;
            AddUIComponentBindersToBindings(componentBinders);
            int afterCount = componentBindingsProp.arraySize;
            
            Debug.Log($"同步选中UIComponentBinder完成，找到 {componentBinders.Count} 个组件绑定器，绑定列表从 {beforeCount} 个更新为 {afterCount} 个");
        }
        
        /// <summary>
        /// 同步所有子UIComponentBinder组件
        /// </summary>
        private void AutoScanAllComponents()
        {
            var componentBinders = designer.GetComponentsInChildren<UIComponentBinder>(true)
                .Where(c => c != null)
                .ToList();
            
            int beforeCount = componentBindingsProp.arraySize;
            AddUIComponentBindersToBindings(componentBinders);
            int afterCount = componentBindingsProp.arraySize;
            
            Debug.Log($"同步UIComponentBinder完成，找到 {componentBinders.Count} 个组件绑定器，绑定列表从 {beforeCount} 个更新为 {afterCount} 个");
        }
        
        /// <summary>
        /// 同步UIComponentBinder到绑定列表，确保数量一致
        /// </summary>
        private void AddUIComponentBindersToBindings(List<UIComponentBinder> componentBinders)
        {
            // 获取所有有效的目标组件
            var validTargetComponents = new List<Component>();
            foreach (var binder in componentBinders)
            {
                var targetComponent = binder.GetTargetComponent();
                if (targetComponent != null)
                {
                    validTargetComponents.Add(targetComponent);
                }
            }
            
            // 先删除不再存在的绑定（从后往前删除以避免索引问题）
            for (int i = componentBindingsProp.arraySize - 1; i >= 0; i--)
            {
                var bindingProp = componentBindingsProp.GetArrayElementAtIndex(i);
                var componentProp = bindingProp.FindPropertyRelative("component");
                var boundComponent = componentProp.objectReferenceValue as Component;
                
                // 如果绑定的组件不在有效的目标组件列表中，则删除这个绑定
                if (boundComponent == null || !validTargetComponents.Contains(boundComponent))
                {
                    componentBindingsProp.DeleteArrayElementAtIndex(i);
                }
            }
            
            // 再添加新的绑定
            foreach (var targetComponent in validTargetComponents)
            {
                // 检查是否已经绑定
                bool alreadyBound = false;
                for (int i = 0; i < componentBindingsProp.arraySize; i++)
                {
                    var bindingProp = componentBindingsProp.GetArrayElementAtIndex(i);
                    var componentProp = bindingProp.FindPropertyRelative("component");
                    if (componentProp.objectReferenceValue == targetComponent)
                    {
                        alreadyBound = true;
                        break;
                    }
                }
                
                if (!alreadyBound)
                {
                    // 添加新绑定
                    componentBindingsProp.arraySize++;
                    var newBindingProp = componentBindingsProp.GetArrayElementAtIndex(componentBindingsProp.arraySize - 1);
                    
                    newBindingProp.FindPropertyRelative("component").objectReferenceValue = targetComponent;
                    newBindingProp.FindPropertyRelative("componentType").stringValue = targetComponent.GetType().Name;
                    // 生成变量名
                    string variableName = GenerateVariableName(targetComponent);
                    newBindingProp.FindPropertyRelative("componentName").stringValue = variableName;
        
                    newBindingProp.FindPropertyRelative("description").stringValue = "";
                }
            }
            
            SafeApplyModifiedProperties();
        }
        
        /// <summary>
        /// 将组件添加到绑定列表（保留原方法以兼容其他功能）
        /// </summary>
        private void AddComponentsToBindings(List<Component> components)
        {
            foreach (var comp in components)
            {
                // 检查是否已经绑定
                bool alreadyBound = false;
                for (int i = 0; i < componentBindingsProp.arraySize; i++)
                {
                    var bindingProp = componentBindingsProp.GetArrayElementAtIndex(i);
                    var componentProp = bindingProp.FindPropertyRelative("component");
                    if (componentProp.objectReferenceValue == comp)
                    {
                        alreadyBound = true;
                        break;
                    }
                }
                
                if (!alreadyBound)
                {
                    // 添加新绑定
                    componentBindingsProp.arraySize++;
                    var newBindingProp = componentBindingsProp.GetArrayElementAtIndex(componentBindingsProp.arraySize - 1);
                    
                    newBindingProp.FindPropertyRelative("component").objectReferenceValue = comp;
                    newBindingProp.FindPropertyRelative("componentType").stringValue = comp.GetType().Name;
                    newBindingProp.FindPropertyRelative("componentName").stringValue = GenerateVariableName(comp);
        
                    newBindingProp.FindPropertyRelative("description").stringValue = "";
                }
            }
            
            SafeApplyModifiedProperties();
        }
        
        /// <summary>
        /// 生成变量名称
        /// </summary>
        private string GenerateVariableName(Component component)
        {
            string typeName = component.GetType().Name;
            string objectName = component.gameObject.name;
            
            // 移除常见的UI前缀
            objectName = objectName.Replace("UI", "").Replace("ui", "");
            
            // 移除所有空格和特殊字符
            objectName = System.Text.RegularExpressions.Regex.Replace(objectName, @"[^a-zA-Z0-9]", "");
            
            // 组合名称
            string variableName = $"{objectName}{typeName}";
            
            // 确保首字母小写
            if (!string.IsNullOrEmpty(variableName))
            {
                variableName = char.ToLower(variableName[0]) + variableName.Substring(1);
            }
            
            // 检查重复并生成唯一名称
            variableName = EnsureUniqueVariableName(variableName);
            
            return variableName;
        }
        
        /// <summary>
        /// 确保变量名唯一
        /// </summary>
        private string EnsureUniqueVariableName(string baseName)
        {
            string uniqueName = baseName;
            int counter = 1;
            
            while (IsVariableNameExists(uniqueName))
            {
                uniqueName = $"{baseName}{counter}";
                counter++;
            }
            
            return uniqueName;
        }
        
        /// <summary>
        /// 检查变量名是否已存在
        /// </summary>
        private bool IsVariableNameExists(string variableName)
        {
            for (int i = 0; i < componentBindingsProp.arraySize; i++)
            {
                var bindingProp = componentBindingsProp.GetArrayElementAtIndex(i);
                var componentNameProp = bindingProp.FindPropertyRelative("componentName");
                if (componentNameProp.stringValue == variableName)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 保存UI设计（使用简化的工作流程）
        /// </summary>
        private void SaveUIDesign()
        {
            try
            {
                Debug.Log("[UIDesignerEditor] 开始保存UI设计...");
                
                // 检查代码是否已生成
                if (!_isCodeGenerated)
                {
                    EditorUtility.DisplayDialog("错误", "请先点击'生成代码'按钮生成UI脚本", "确定");
                    return;
                }
                
                // 验证设置
                if (!ValidateSettings())
                {
                    return;
                }
                
                // 生成Prefab保存路径
                string prefabPath = GeneratePrefabPath(designer.UIFormName);
                
                Debug.Log($"[UIDesignerEditor] 启动简化保存工作流程: {designer.gameObject.name} -> {prefabPath}");
                
                // 使用简化的工作流程保存Prefab
                if (SimpleUIWorkflow.SaveAsPrefab(designer, prefabPath))
                {
                    // 重置代码生成状态，准备下次使用
                    _isCodeGenerated = false;
                    Debug.Log("[UIDesignerEditor] 重置代码生成状态，准备下次使用");
                    
                    // 刷新界面以更新按钮状态
                    Repaint();
                    
                    // 显示完成通知
                    var settings = UIDesignerSettings.Instance;
                    if (settings.showCompletionNotification)
                    {
                        EditorUtility.DisplayDialog("保存完成", $"UI设计已成功保存为Prefab:\n{prefabPath}", "确定");
                    }
                    
                    Debug.Log($"[UIDesignerEditor] UI设计保存完成: {prefabPath}");
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "保存失败，请查看控制台了解详细信息。", "确定");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[UIDesignerEditor] 保存UI设计失败：{ex.Message}\n{ex.StackTrace}");
                EditorUtility.DisplayDialog("错误", $"保存UI设计失败：{ex.Message}", "确定");
            }
        }
        
        #region 验证设置
        
        /// <summary>
        /// 验证设置
        /// </summary>
        private bool ValidateSettings()
        {
            if (designer == null)
            {
                EditorUtility.DisplayDialog("错误", "UIDesigner组件无效", "确定");
                return false;
            }
            
            if (string.IsNullOrEmpty(designer.UIFormName))
            {
                EditorUtility.DisplayDialog("错误", "UI窗体名称不能为空", "确定");
                return false;
            }
            
            if (string.IsNullOrEmpty(designer.NamespaceName))
            {
                EditorUtility.DisplayDialog("错误", "命名空间不能为空", "确定");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// 生成Prefab保存路径
        /// </summary>
        private string GeneratePrefabPath(string uiFormName)
        {
            string prefabDirectory = UIDesignerSettings.Instance.defaultPrefabPath;
            if (!System.IO.Directory.Exists(prefabDirectory))
            {
                System.IO.Directory.CreateDirectory(prefabDirectory);
            }
            return $"{prefabDirectory}/{uiFormName}.prefab";
        }
        

        
        #endregion
        
        /// <summary>
        /// 清理资源
        /// </summary>
        private void OnDisable()
        {
            // 清理进度条
            EditorUtility.ClearProgressBar();
        }
        
        /// <summary>
        /// 挂载生成的脚本组件
        /// </summary>
        private void AttachGeneratedScript()
        {
            AttachGeneratedScriptToGameObject(designer.gameObject, designer.UIFormName, designer.NamespaceName);
        }
        
        /// <summary>
        /// 为指定GameObject挂载生成的脚本组件
        /// </summary>
        private void AttachGeneratedScriptToGameObject(GameObject gameObject, string uiFormName, string namespaceName)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject为空，无法挂载脚本");
                return;
            }
            
            string scriptTypeName = $"{namespaceName}.{uiFormName}";
            
            // 查找生成的脚本类型
            System.Type scriptType = FindScriptType(scriptTypeName);
            if (scriptType == null)
            {
                Debug.LogError($"找不到生成的脚本类型：{scriptTypeName}，请确保脚本已编译");
                return;
            }
            
            // 检查是否已经挂载了该脚本
            if (gameObject.GetComponent(scriptType) != null)
            {
                Debug.Log($"脚本组件 {uiFormName} 已存在，跳过挂载");
                return;
            }
            
            // 挂载脚本组件
            try
            {
                var scriptComponent = gameObject.AddComponent(scriptType);
                Debug.Log($"成功挂载脚本组件：{uiFormName}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"挂载脚本组件失败：{ex.Message}");
            }
        }
        
        /// <summary>
        /// 绑定组件变量
        /// </summary>
        private void BindComponentVariables()
        {
            BindComponentVariablesToGameObject(designer.gameObject, designer.UIFormName, designer.NamespaceName, designer.ComponentBindings);
        }
        
        /// <summary>
        /// 为指定GameObject绑定组件变量
        /// </summary>
        private void BindComponentVariablesToGameObject(GameObject gameObject, string uiFormName, string namespaceName, List<UIComponentBinding> componentBindings)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject为空，无法绑定组件变量");
                return;
            }
            
            string scriptTypeName = $"{namespaceName}.{uiFormName}";
            
            // 查找脚本组件
            System.Type scriptType = FindScriptType(scriptTypeName);
            if (scriptType == null)
            {
                Debug.LogWarning($"未找到脚本类型：{scriptTypeName}，跳过组件绑定");
                return;
            }
            
            Component scriptComponent = gameObject.GetComponent(scriptType);
            if (scriptComponent == null)
            {
                Debug.LogWarning($"GameObject {gameObject.name} 未挂载脚本 {scriptType.Name}，跳过组件绑定");
                return;
            }
            
            // 使用SerializedObject进行组件绑定
            SerializedObject serializedScript = new SerializedObject(scriptComponent);
            
            // 调试：输出脚本组件的所有序列化字段
            Debug.Log($"脚本组件 {scriptType.Name} 的序列化字段：");
            SerializedProperty iterator = serializedScript.GetIterator();
            while (iterator.NextVisible(true))
            {
                Debug.Log($"  字段: {iterator.name}, 类型: {iterator.propertyType}");
            }
            
            int boundCount = 0;
            foreach (var binding in componentBindings)
            {
                if (binding.IsValid())
                {
                    SerializedProperty property = serializedScript.FindProperty(binding.ComponentName);
                    if (property != null && property.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        property.objectReferenceValue = binding.Component;
                        boundCount++;
                        Debug.Log($"绑定变量 {binding.ComponentName} -> {binding.Component.name} ({binding.ComponentType})");
                    }
                    else
                    {
                        Debug.LogWarning($"未找到变量 {binding.ComponentName} 或类型不匹配");
                    }
                }
            }
            
            serializedScript.ApplyModifiedProperties();
            Debug.Log($"组件绑定完成，成功绑定 {boundCount}/{componentBindings.Count} 个变量");
        }
        
        /// <summary>
        /// 清理设计脚本
        /// </summary>
        private void CleanupDesignScripts()
        {
            CleanupDesignScriptsFromGameObject(designer.gameObject);
            
            // 重置代码生成状态，因为设计脚本已清理
            _isCodeGenerated = false;
            Debug.Log("[UIDesignerEditor] 清理设计脚本后重置代码生成状态");
            
            // 刷新界面以更新按钮状态
            Repaint();
        }
        
        /// <summary>
        /// 从指定GameObject清理设计脚本
        /// </summary>
        private void CleanupDesignScriptsFromGameObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject为空，无法清理设计脚本");
                return;
            }
            
            // 移除所有UIComponentBinder组件
            UIComponentBinder[] binders = gameObject.GetComponentsInChildren<UIComponentBinder>(true);
            int binderCount = binders.Length;
            foreach (var binder in binders)
            {
                if (binder != null)
                {
                    DestroyImmediate(binder);
                }
            }
            
            // 移除UIDesigner组件
            UIDesigner designerComponent = gameObject.GetComponent<UIDesigner>();
            if (designerComponent != null)
            {
                DestroyImmediate(designerComponent);
            }
            
            Debug.Log($"清理设计脚本完成，移除了 {binderCount} 个UIComponentBinder和1个UIDesigner");
        }
        
        /// <summary>
        /// 保存为Prefab
        /// </summary>
        private void SaveAsPrefab()
        {
            // 检查designer是否仍然有效
            if (designer == null)
            {
                Debug.LogError("UIDesigner组件已被销毁，无法保存为Prefab");
                return;
            }
            
            SaveGameObjectAsPrefab(designer.gameObject, designer.UIFormName);
        }
        
        /// <summary>
        /// 将指定GameObject保存为Prefab
        /// </summary>
        private void SaveGameObjectAsPrefab(GameObject gameObject, string uiFormName)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject为空，无法保存为Prefab");
                return;
            }
            
            // 检查当前是否已经是Prefab
            bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(gameObject) || PrefabUtility.IsPartOfPrefabInstance(gameObject);
            
            if (isPrefab)
            {
                // 如果是Prefab实例，则覆盖原Prefab
                if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
                {
                    GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
                    if (prefabRoot != null)
                    {
                        PrefabUtility.ApplyPrefabInstance(prefabRoot, InteractionMode.AutomatedAction);
                        Debug.Log($"已覆盖现有Prefab：{prefabRoot.name}");
                    }
                }
                else
                {
                    Debug.Log("当前对象已经是Prefab资源，无需额外保存");
                }
            }
            else
            {
                // 如果不是Prefab，则保存为新Prefab
                string prefabDirectory = UIDesignerSettings.Instance.defaultPrefabPath;
                string prefabPath = $"{prefabDirectory}/{uiFormName}.prefab";
                
                // 确保目录存在
                string directory = System.IO.Path.GetDirectoryName(prefabPath);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                    AssetDatabase.Refresh();
                }
                
                // 创建Prefab
                GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, prefabPath);
                if (prefab != null)
                {
                    Debug.Log($"已保存为新Prefab：{prefabPath}");
                }
                else
                {
                    Debug.LogError($"保存Prefab失败：{prefabPath}");
                }
            }
        }
        
        /// <summary>
        /// 查找脚本类型
        /// </summary>
        private System.Type FindScriptType(string typeName)
        {
            // 首先尝试直接获取类型
            System.Type type = System.Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }
            
            // 遍历所有程序集查找类型
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 生成代码（保留原方法用于单独生成代码）
        /// </summary>
        /// <summary>
        /// 第一步：生成代码
        /// </summary>
        private void GenerateCodeStep()
        {
            try
            {
                // 验证设置
                if (!ValidateSettings())
                {
                    return;
                }
                
                Debug.Log("[UIDesignerEditor] 开始生成UI代码...");
                
                // 使用简化的工作流程生成代码
                if (SimpleUIWorkflow.GenerateCode(designer))
                {
                    // 标记代码已生成
                    _isCodeGenerated = true;
                    
                    Debug.Log("[UIDesignerEditor] UI代码生成完成");
                    EditorUtility.DisplayDialog("成功", "代码生成完成！现在可以点击'保存'按钮继续后续流程。", "确定");
                    
                    // 刷新界面
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "代码生成失败，请查看控制台了解详细信息。", "确定");
                }
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("错误", $"代码生成失败：{ex.Message}", "确定");
                Debug.LogError($"[UIDesignerEditor] 代码生成失败：{ex}");
            }
        }
        
        /// <summary>
        /// 重新生成代码
        /// </summary>
        private void RegenerateCode()
        {
            _isCodeGenerated = false;
            GenerateCodeStep();
        }
        
        /// <summary>
        /// 仅生成代码（保留原有方法用于兼容）
        /// </summary>
        private void GenerateCode()
        {
            try
            {
                UICodeGenerator.GenerateCode(designer);
                EditorUtility.DisplayDialog("成功", "代码生成完成！", "确定");
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog("错误", $"代码生成失败：{ex.Message}", "确定");
                Debug.LogError($"代码生成失败：{ex}");
            }
        }
        

    }
}
