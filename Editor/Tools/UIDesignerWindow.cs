using UnityEditor;
using UnityEngine;

namespace UGF.GameFramework.UI.Editor
{
    public class UIDesignerWindow : EditorWindow
    {
        private UIDesignerSettings _settings;
        private Vector2 _scrollPosition;

        [MenuItem("UGF/GameFramework/UI Designer/Settings Window", false, 1)]
        public static void Open()
        {
            var window = GetWindow<UIDesignerWindow>("UI Designer 配置");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnEnable()
        {
            _settings = UIDesignerSettings.Instance;
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                _settings = UIDesignerSettings.Instance;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("代码生成设置", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _settings.defaultNamespace = EditorGUILayout.TextField("默认命名空间", _settings.defaultNamespace);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("路径设置", EditorStyles.boldLabel);
            _settings.defaultGeneratePath = EditorGUILayout.TextField(new GUIContent("默认生成路径", "如果未设置特定路径，则使用此路径生成脚本"), _settings.defaultGeneratePath);
            _settings.logicGeneratePath = EditorGUILayout.TextField(new GUIContent("业务逻辑生成路径", "业务逻辑脚本的生成路径 (例如 MyForm.cs)"), _settings.logicGeneratePath);
            _settings.bindingGeneratePath = EditorGUILayout.TextField(new GUIContent("绑定代码生成路径", "绑定脚本的生成路径 (例如 MyForm.Generated.cs)"), _settings.bindingGeneratePath);
            _settings.defaultPrefabPath = EditorGUILayout.TextField(new GUIContent("默认 Prefab 路径", "生成的 Prefab 存放路径"), _settings.defaultPrefabPath);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("选项", EditorStyles.boldLabel);
            _settings.autoCreateDirectories = EditorGUILayout.Toggle("自动创建目录", _settings.autoCreateDirectories);
            _settings.generateDetailedComments = EditorGUILayout.Toggle("生成详细注释", _settings.generateDetailedComments);
            _settings.useRegionMarkers = EditorGUILayout.Toggle("使用 Region 标记", _settings.useRegionMarkers);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefab 设置", EditorStyles.boldLabel);
            _settings.refreshAssetsAfterSave = EditorGUILayout.Toggle("保存后刷新资源", _settings.refreshAssetsAfterSave);
            _settings.autoSelectGeneratedPrefab = EditorGUILayout.Toggle("自动选中生成的 Prefab", _settings.autoSelectGeneratedPrefab);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("界面设置", EditorStyles.boldLabel);
            _settings.showDetailedLogs = EditorGUILayout.Toggle("显示详细日志", _settings.showDetailedLogs);
            _settings.showCompletionNotification = EditorGUILayout.Toggle("显示完成通知", _settings.showCompletionNotification);
            _settings.errorDialogTimeout = EditorGUILayout.Slider("错误对话框超时时间", _settings.errorDialogTimeout, 0f, 30f);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("高级设置", EditorStyles.boldLabel);
            _settings.debugMode = EditorGUILayout.Toggle("调试模式", _settings.debugMode);
            _settings.backupOriginalFiles = EditorGUILayout.Toggle("备份原始文件", _settings.backupOriginalFiles);
            if (_settings.backupOriginalFiles)
            {
                _settings.backupRetentionDays = EditorGUILayout.IntSlider("备份保留天数", _settings.backupRetentionDays, 1, 30);
            }
            _settings.validateGeneratedCode = EditorGUILayout.Toggle("验证生成的代码", _settings.validateGeneratedCode);

            EditorGUILayout.Space();
            if (GUILayout.Button("重置为默认值"))
            {
                if (EditorUtility.DisplayDialog("重置设置", "确定要将所有设置重置为默认值吗？", "是", "否"))
                {
                    _settings.ResetToDefaults();
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_settings);
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
