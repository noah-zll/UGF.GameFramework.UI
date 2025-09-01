using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UGF.GameFramework.UI.Editor
{
    /// <summary>
    /// UI设计器设置
    /// 管理UI设计器的各种配置参数
    /// </summary>
    [CreateAssetMenu(fileName = "UIDesignerSettings", menuName = "UGF/UI Designer/Settings")]
    public class UIDesignerSettings : ScriptableObject
    {
        #region 静态实例
        
        private static UIDesignerSettings _instance;
        
        /// <summary>
        /// 获取设置实例
        /// </summary>
        public static UIDesignerSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadOrCreateSettings();
                }
                return _instance;
            }
        }
        
        #endregion
        
        #region 代码生成设置
        
        [Header("代码生成设置")]
        [Tooltip("默认命名空间")]
        public string defaultNamespace = "UGF.GameFramework.UI";
        
        [Tooltip("默认生成路径")]
        public string defaultGeneratePath = "Assets/Scripts/UI/Generated";
        
        [Tooltip("默认Prefab路径")]
        public string defaultPrefabPath = "Assets/Prefabs/UI";
        
        [Tooltip("是否自动创建目录")]
        public bool autoCreateDirectories = true;
        
        [Tooltip("是否生成详细注释")]
        public bool generateDetailedComments = true;
        
        [Tooltip("是否使用区域标记")]
        public bool useRegionMarkers = true;
        
        #endregion
        
        // 编译设置已移除 - 简化工作流程不再需要复杂的编译等待机制
        
        #region Prefab生成设置
        
        [Header("Prefab生成设置")]
        [Tooltip("是否保存Prefab后刷新资源")]
        public bool refreshAssetsAfterSave = true;
        
        [Tooltip("是否自动选中生成的Prefab")]
        public bool autoSelectGeneratedPrefab = true;
        
        #endregion
        
        #region UI设置
        
        [Header("用户界面设置")]
        // 进度窗口已移除 - 使用Unity内置进度条
        
        [Tooltip("是否显示详细日志")]
        public bool showDetailedLogs = true;
        
        [Tooltip("是否在完成后显示通知")]
        public bool showCompletionNotification = true;
        
        [Tooltip("错误对话框显示时间（秒，0表示手动关闭）")]
        [Range(0f, 30f)]
        public float errorDialogTimeout = 0f;
        
        #endregion
        
        #region 高级设置
        
        [Header("高级设置")]
        [Tooltip("是否启用调试模式")]
        public bool debugMode = false;
        
        [Tooltip("是否备份原始文件")]
        public bool backupOriginalFiles = false;
        
        [Tooltip("备份文件保留天数")]
        [Range(1, 30)]
        public int backupRetentionDays = 7;
        
        [Tooltip("是否验证生成的代码")]
        public bool validateGeneratedCode = true;
        
        #endregion
        
        #region 静态方法
        
        /// <summary>
        /// 加载或创建设置
        /// </summary>
        private static UIDesignerSettings LoadOrCreateSettings()
        {
            string settingsPath = "Assets/Editor/UIDesignerSettings.asset";
            
            // 尝试加载现有设置
            UIDesignerSettings settings = AssetDatabase.LoadAssetAtPath<UIDesignerSettings>(settingsPath);
            
            if (settings == null)
            {
                // 创建新设置
                settings = CreateInstance<UIDesignerSettings>();
                
                // 确保目录存在
                string directory = Path.GetDirectoryName(settingsPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // 保存设置
                AssetDatabase.CreateAsset(settings, settingsPath);
                AssetDatabase.SaveAssets();
                
                Debug.Log($"[UIDesignerSettings] 创建新的设置文件: {settingsPath}");
            }
            
            return settings;
        }
        
        /// <summary>
        /// 重置为默认设置
        /// </summary>
        public void ResetToDefaults()
        {
            defaultNamespace = "UGF.GameFramework.UI";
            defaultGeneratePath = "Assets/Scripts/UI/Generated";
            defaultPrefabPath = "Assets/Prefabs/UI";
            autoCreateDirectories = true;
            generateDetailedComments = true;
            useRegionMarkers = true;
            
            // 编译设置和部分Prefab设置已移除
            refreshAssetsAfterSave = true;
            autoSelectGeneratedPrefab = true;
            
            // 进度窗口设置已移除
            showDetailedLogs = true;
            showCompletionNotification = true;
            errorDialogTimeout = 0f;
            
            debugMode = false;
            backupOriginalFiles = false;
            backupRetentionDays = 7;
            validateGeneratedCode = true;
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// 验证设置
        /// </summary>
        public bool ValidateSettings(out string errorMessage)
        {
            errorMessage = string.Empty;
            
            // 验证命名空间
            if (string.IsNullOrWhiteSpace(defaultNamespace))
            {
                errorMessage = "默认命名空间不能为空";
                return false;
            }
            
            // 验证路径
            if (string.IsNullOrWhiteSpace(defaultGeneratePath))
            {
                errorMessage = "默认生成路径不能为空";
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(defaultPrefabPath))
            {
                errorMessage = "默认Prefab路径不能为空";
                return false;
            }
            
            // 编译相关验证已移除
            
            return true;
        }
        
        #endregion
        
        #region Unity生命周期
        
        private void OnValidate()
        {
            // 确保设置值在合理范围内
            // 编译相关设置验证已移除
            backupRetentionDays = Mathf.Clamp(backupRetentionDays, 1, 30);
            errorDialogTimeout = Mathf.Clamp(errorDialogTimeout, 0f, 30f);
        }
        
        #endregion
    }
    
    /// <summary>
    /// UI设计器设置提供者
    /// 用于在Project Settings中显示设置
    /// </summary>
    public static class UIDesignerSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateUIDesignerSettingsProvider()
        {
            var provider = new SettingsProvider("Project/UGF/UI Designer", SettingsScope.Project)
            {
                label = "UI Designer",
                guiHandler = (searchContext) =>
                {
                    var settings = UIDesignerSettings.Instance;
                    if (settings != null)
                    {
                        var serializedObject = new SerializedObject(settings);
                        
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("UI设计器设置", EditorStyles.boldLabel);
                        EditorGUILayout.Space();
                        
                        EditorGUI.BeginChangeCheck();
                        
                        // 绘制所有属性
                        SerializedProperty prop = serializedObject.GetIterator();
                        if (prop.NextVisible(true))
                        {
                            do
                            {
                                if (prop.name != "m_Script")
                                {
                                    EditorGUILayout.PropertyField(prop, true);
                                }
                            }
                            while (prop.NextVisible(false));
                        }
                        
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                        }
                        
                        EditorGUILayout.Space();
                        
                        // 操作按钮
                        EditorGUILayout.BeginHorizontal();
                        
                        if (GUILayout.Button("重置为默认值"))
                        {
                            if (EditorUtility.DisplayDialog("重置设置", "确定要重置所有设置为默认值吗？", "确定", "取消"))
                            {
                                settings.ResetToDefaults();
                            }
                        }
                        
                        if (GUILayout.Button("验证设置"))
                        {
                            if (settings.ValidateSettings(out string errorMessage))
                            {
                                EditorUtility.DisplayDialog("设置验证", "所有设置都有效！", "确定");
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("设置验证失败", errorMessage, "确定");
                            }
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                },
                keywords = new[] { "UI", "Designer", "设计器", "设置" }
            };
            
            return provider;
        }
    }
}