using UnityEngine;
using UnityEditor;
using UGF.GameFramework.UI;

namespace UGF.GameFramework.UI.Editor
{
    /// <summary>
    /// UIComponentBinder自定义Inspector
    /// </summary>
    [CustomEditor(typeof(UIComponentBinder))]
    public class UIComponentBinderEditor : UnityEditor.Editor
    {
        private SerializedProperty m_ComponentTypeName;
        private SerializedProperty m_BindEvents;
        private SerializedProperty m_ComponentPath;
        
        private UIComponentBinder m_Target;
        private bool m_ShowAdvanced = false;
        private string[] m_ComponentTypeNames;
        private int m_SelectedComponentIndex = 0;
        
        private void OnEnable()
        {
            // 检查target对象是否有效
            if (target == null)
            {
                return;
            }
            
            m_Target = target as UIComponentBinder;
            
            // 检查serializedObject是否有效
            if (serializedObject == null || serializedObject.targetObject == null)
            {
                return;
            }
            
            try
            {
                m_ComponentTypeName = serializedObject.FindProperty("m_ComponentTypeName");
                m_BindEvents = serializedObject.FindProperty("m_BindEvents");
                m_ComponentPath = serializedObject.FindProperty("m_ComponentPath");
                
                UpdateComponentTypeNames();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[UIComponentBinderEditor] OnEnable初始化失败: {ex.Message}");
            }
        }
        
        public override void OnInspectorGUI()
        {
            // 检查target对象是否有效
            if (target == null)
            {
                EditorGUILayout.HelpBox("目标对象无效", MessageType.Error);
                return;
            }
            
            // 检查serializedObject是否有效
            if (serializedObject == null || serializedObject.targetObject == null)
            {
                EditorGUILayout.HelpBox("序列化对象无效", MessageType.Error);
                return;
            }
            
            try
            {
                serializedObject.Update();
            }
            catch (System.Exception ex)
            {
                EditorGUILayout.HelpBox($"更新序列化对象失败: {ex.Message}", MessageType.Error);
                return;
            }
            
            EditorGUILayout.Space();
            
            // 标题
            EditorGUILayout.LabelField("UI Component Binder", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 基础配置
            EditorGUILayout.LabelField("基础配置", EditorStyles.boldLabel);
            
            // 组件类型选择
            if (m_ComponentTypeNames != null && m_ComponentTypeNames.Length > 0)
            {
                // 找到当前选中的组件索引
                var currentTypeName = m_ComponentTypeName.stringValue;
                for (int i = 0; i < m_ComponentTypeNames.Length; i++)
                {
                    if (m_ComponentTypeNames[i] == currentTypeName)
                    {
                        m_SelectedComponentIndex = i;
                        break;
                    }
                }
                
                EditorGUI.BeginChangeCheck();
                m_SelectedComponentIndex = EditorGUILayout.Popup(new GUIContent("组件类型", "选择要绑定的组件类型"), m_SelectedComponentIndex, m_ComponentTypeNames);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_SelectedComponentIndex >= 0 && m_SelectedComponentIndex < m_ComponentTypeNames.Length)
                    {
                        m_ComponentTypeName.stringValue = m_ComponentTypeNames[m_SelectedComponentIndex];
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("当前GameObject上没有找到可绑定的组件", MessageType.Warning);
            }
            
            // 事件绑定
            EditorGUILayout.PropertyField(m_BindEvents, new GUIContent("绑定事件", "是否在代码生成时包含事件绑定代码"));
            
            EditorGUILayout.Space();
            
            // 检测信息
            EditorGUILayout.LabelField("检测信息", EditorStyles.boldLabel);
            
            GUI.enabled = false;
            
            // 当前选择的组件类型
            var selectedTypeName = m_ComponentTypeName.stringValue;
            EditorGUILayout.TextField(new GUIContent("选择的类型", "当前选择的组件类型"), string.IsNullOrEmpty(selectedTypeName) ? "未选择" : selectedTypeName);
            
            // 组件路径
            EditorGUILayout.TextField(new GUIContent("组件路径", "从窗体根节点到此组件的路径"), m_Target.ComponentPath ?? "");
            
            // 有效性检查
            var isValid = m_Target.IsValid;
            var validityColor = isValid ? Color.green : Color.red;
            var validityText = isValid ? "有效" : "无效";
            
            var originalColor = GUI.color;
            GUI.color = validityColor;
            EditorGUILayout.TextField(new GUIContent("绑定状态", "组件绑定器的有效性状态"), validityText);
            GUI.color = originalColor;
            
            GUI.enabled = true;
            
            EditorGUILayout.Space();
            
            // 高级选项
            m_ShowAdvanced = EditorGUILayout.Foldout(m_ShowAdvanced, "高级选项", true);
            if (m_ShowAdvanced)
            {
                EditorGUI.indentLevel++;
                
                // 手动设置路径
                EditorGUILayout.PropertyField(m_ComponentPath, new GUIContent("手动路径", "手动指定组件路径（通常不需要修改）"));
                
                EditorGUILayout.Space();
                
                // 操作按钮
                EditorGUILayout.LabelField("操作", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("刷新信息", GUILayout.Height(25)))
                {
                    m_Target.UpdateComponentInfo();
                    UpdateComponentTypeNames();
                    EditorUtility.SetDirty(m_Target);
                }
                
                if (GUILayout.Button("生成变量名", GUILayout.Height(25)))
                {
                    // 变量名生成功能已移除，因为UIComponentBinder不再包含m_VariableName字段
                    EditorUtility.DisplayDialog("提示", "变量名生成功能已移除，请使用UIDesigner系统进行组件绑定。", "确定");
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space();
                
                // 验证结果
                if (GUILayout.Button("验证绑定", GUILayout.Height(25)))
                {
                    ValidateBinding();
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space();
            
            // 帮助信息
            if (!isValid)
            {
                EditorGUILayout.HelpBox(GetValidationMessage(), MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("组件绑定器配置正确，可以正常使用。", MessageType.Info);
            }
            
            SafeApplyModifiedProperties();
            
            // 如果有修改，自动更新组件信息
            if (GUI.changed)
            {
                m_Target.UpdateComponentInfo();
                UpdateComponentTypeNames();
            }
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
                Debug.LogWarning($"[UIComponentBinderEditor] 应用序列化对象修改失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 更新组件类型名称列表
        /// </summary>
        private void UpdateComponentTypeNames()
        {
            if (m_Target == null) return;
            
            var availableComponents = m_Target.AvailableComponents;
            if (availableComponents != null && availableComponents.Length > 0)
            {
                var typeNames = new System.Collections.Generic.List<string>();
                foreach (var component in availableComponents)
                {
                    var typeName = component.GetType().Name;
                    if (!typeNames.Contains(typeName))
                    {
                        typeNames.Add(typeName);
                    }
                }
                m_ComponentTypeNames = typeNames.ToArray();
            }
            else
            {
                m_ComponentTypeNames = new string[0];
            }
        }
        
        /// <summary>
        /// 获取验证消息
        /// </summary>
        /// <returns>验证消息</returns>
        private string GetValidationMessage()
        {
            if (m_Target.GetTargetComponent() == null)
            {
                var typeName = m_ComponentTypeName.stringValue;
                return string.IsNullOrEmpty(typeName) ? "请选择要绑定的组件类型。" : $"无法找到类型为 {typeName} 的组件。";
            }
            
            return "组件绑定器配置有误。";
        }
        
        /// <summary>
        /// 验证绑定
        /// </summary>
        private void ValidateBinding()
        {
            // 简单验证组件是否有效
            if (m_Target.IsValid)
            {
                EditorUtility.DisplayDialog("验证成功", "组件绑定配置正确。", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("验证失败", "组件绑定配置无效，请检查目标组件是否正确设置。", "确定");
            }
        }
        
        /// <summary>
        /// 查找窗体根节点
        /// </summary>
        /// <param name="current">当前节点</param>
        /// <returns>窗体根节点</returns>
        private Transform FindFormRoot(Transform current)
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
    }
}