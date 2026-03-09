using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.UI;
using UnityGameFramework.Runtime;

namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI配置管理器
    /// </summary>
    public sealed class UIConfigManager : GameFrameworkComponent, IUIConfigManager
    {
        private readonly Dictionary<string, UIFormConfig> m_UIFormConfigs;
        private readonly Dictionary<string, UIGroupConfig> m_UIGroupConfigs;
        private UIGlobalConfig m_GlobalConfig;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UIConfigManager()
        {
            m_UIFormConfigs = new Dictionary<string, UIFormConfig>();
            m_UIGroupConfigs = new Dictionary<string, UIGroupConfig>();
            m_GlobalConfig = new UIGlobalConfig();
        }

        /// <summary>
        /// 全局配置
        /// </summary>
        public UIGlobalConfig GlobalConfig
        {
            get => m_GlobalConfig;
            set => m_GlobalConfig = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 组件销毁时清理资源
        /// </summary>
        private void OnDestroy()
        {
            m_UIFormConfigs?.Clear();
            m_UIGroupConfigs?.Clear();
            m_GlobalConfig = null;
        }

        /// <summary>
        /// 初始化UI组
        /// </summary>
        public void InitializeUIGroups()
        {
            UIComponent uiComponent = GameEntry.GetComponent<UIComponent>();
            if (uiComponent == null)
            {
                Log.Error("UIComponent is invalid.");
                return;
            }

            foreach (var groupConfig in m_UIGroupConfigs.Values)
            {
                if (!uiComponent.HasUIGroup(groupConfig.GroupName))
                {
                    uiComponent.AddUIGroup(groupConfig.GroupName, groupConfig.Depth);
                    Log.Info($"Auto create UI Group: {groupConfig.GroupName}, Depth: {groupConfig.Depth}");
                }
            }
        }

        /// <summary>
        /// 添加UI窗体配置
        /// </summary>
        /// <param name="config">UI窗体配置</param>
        public void AddUIFormConfig(UIFormConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrEmpty(config.FormName))
            {
                throw new ArgumentException("UI form name is null or empty.", nameof(config));
            }

            m_UIFormConfigs[config.FormName] = config;
        }

        /// <summary>
        /// 移除UI窗体配置
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveUIFormConfig(string formName)
        {
            if (string.IsNullOrEmpty(formName))
            {
                return false;
            }

            return m_UIFormConfigs.Remove(formName);
        }

        /// <summary>
        /// 获取UI窗体配置
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <returns>UI窗体配置</returns>
        public UIFormConfig GetUIFormConfig(string formName)
        {
            if (string.IsNullOrEmpty(formName))
            {
                return null;
            }

            m_UIFormConfigs.TryGetValue(formName, out var config);
            return config;
        }

        /// <summary>
        /// 获取所有UI窗体配置
        /// </summary>
        /// <returns>所有UI窗体配置</returns>
        public UIFormConfig[] GetAllUIFormConfigs()
        {
            var configs = new UIFormConfig[m_UIFormConfigs.Count];
            var index = 0;
            foreach (var config in m_UIFormConfigs.Values)
            {
                configs[index++] = config;
            }
            return configs;
        }

        /// <summary>
        /// 是否存在UI窗体配置
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <returns>是否存在</returns>
        public bool HasUIFormConfig(string formName)
        {
            if (string.IsNullOrEmpty(formName))
            {
                return false;
            }

            return m_UIFormConfigs.ContainsKey(formName);
        }

        /// <summary>
        /// 添加UI组配置
        /// </summary>
        /// <param name="config">UI组配置</param>
        public void AddUIGroupConfig(UIGroupConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrEmpty(config.GroupName))
            {
                throw new ArgumentException("UI group name is null or empty.", nameof(config));
            }

            m_UIGroupConfigs[config.GroupName] = config;
        }

        /// <summary>
        /// 移除UI组配置
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveUIGroupConfig(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return false;
            }

            return m_UIGroupConfigs.Remove(groupName);
        }

        /// <summary>
        /// 获取UI组配置
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>UI组配置</returns>
        public UIGroupConfig GetUIGroupConfig(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return null;
            }

            m_UIGroupConfigs.TryGetValue(groupName, out var config);
            return config;
        }

        /// <summary>
        /// 获取所有UI组配置
        /// </summary>
        /// <returns>所有UI组配置</returns>
        public UIGroupConfig[] GetAllUIGroupConfigs()
        {
            var configs = new UIGroupConfig[m_UIGroupConfigs.Count];
            var index = 0;
            foreach (var config in m_UIGroupConfigs.Values)
            {
                configs[index++] = config;
            }
            return configs;
        }

        /// <summary>
        /// 是否存在UI组配置
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>是否存在</returns>
        public bool HasUIGroupConfig(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return false;
            }

            return m_UIGroupConfigs.ContainsKey(groupName);
        }

        /// <summary>
        /// 清空所有配置
        /// </summary>
        public void ClearAllConfigs()
        {
            m_UIFormConfigs.Clear();
            m_UIGroupConfigs.Clear();
        }

        /// <summary>
        /// 从资源加载配置
        /// </summary>
        /// <param name="configAssetPath">配置资源路径（相对于Resources文件夹，无扩展名）</param>
        public void LoadConfigFromAsset(string configAssetPath)
        {
            if (string.IsNullOrEmpty(configAssetPath))
            {
                throw new ArgumentException("Config asset path is null or empty.", nameof(configAssetPath));
            }

            TextAsset configAsset = Resources.Load<TextAsset>(configAssetPath);
            if (configAsset == null)
            {
                Log.Error($"Failed to load config asset from Resources: {configAssetPath}");
                return;
            }

            try
            {
                UIConfigData configData = JsonUtility.FromJson<UIConfigData>(configAsset.text);
                if (configData != null)
                {
                    ClearAllConfigs();
                    
                    if (configData.GlobalConfig != null)
                    {
                        m_GlobalConfig = configData.GlobalConfig;
                    }

                    if (configData.GroupConfigs != null)
                    {
                        foreach (var groupConfig in configData.GroupConfigs)
                        {
                            AddUIGroupConfig(groupConfig);
                        }
                    }

                    if (configData.FormConfigs != null)
                    {
                        foreach (var formConfig in configData.FormConfigs)
                        {
                            AddUIFormConfig(formConfig);
                        }
                    }
                    
                    Log.Info($"Successfully loaded UI config from {configAssetPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to parse config asset: {ex.Message}");
            }
        }

        /// <summary>
        /// 保存配置到资源
        /// </summary>
        /// <param name="configAssetPath">配置资源路径（完整路径或Assets开头路径）</param>
        public void SaveConfigToAsset(string configAssetPath)
        {
            if (string.IsNullOrEmpty(configAssetPath))
            {
                throw new ArgumentException("Config asset path is null or empty.", nameof(configAssetPath));
            }

#if UNITY_EDITOR
            var configData = new UIConfigData
            {
                GlobalConfig = m_GlobalConfig,
                GroupConfigs = new List<UIGroupConfig>(m_UIGroupConfigs.Values),
                FormConfigs = new List<UIFormConfig>(m_UIFormConfigs.Values)
            };

            string json = JsonUtility.ToJson(configData, true);
            string fullPath = configAssetPath;
            if (configAssetPath.StartsWith("Assets"))
            {
                fullPath = System.IO.Path.Combine(Application.dataPath, configAssetPath.Substring(7));
            }
            
            try 
            {
                string dir = System.IO.Path.GetDirectoryName(fullPath);
                if (!System.IO.Directory.Exists(dir))
                {
                    System.IO.Directory.CreateDirectory(dir);
                }

                System.IO.File.WriteAllText(fullPath, json);
                Log.Info($"Successfully saved UI config to {configAssetPath}");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to save config asset: {ex.Message}");
            }
#else
            Log.Error("SaveConfigToAsset is only supported in Editor mode.");
#endif
        }

        [Serializable]
        private class UIConfigData
        {
            public UIGlobalConfig GlobalConfig;
            public List<UIGroupConfig> GroupConfigs;
            public List<UIFormConfig> FormConfigs;
        }
    }
}