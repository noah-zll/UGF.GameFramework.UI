using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
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
            GameEntry.GetComponent<UIComponent>().AddUIGroup(config.GroupName, config.Depth);
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
        /// <param name="configAssetPath">配置资源路径</param>
        public void LoadConfigFromAsset(string configAssetPath)
        {
            if (string.IsNullOrEmpty(configAssetPath))
            {
                throw new ArgumentException("Config asset path is null or empty.", nameof(configAssetPath));
            }
            
            // TODO: 实现从资源加载配置的逻辑
            // 这里可以使用GameFramework的资源管理器来加载配置文件
        }
        
        /// <summary>
        /// 保存配置到资源
        /// </summary>
        /// <param name="configAssetPath">配置资源路径</param>
        public void SaveConfigToAsset(string configAssetPath)
        {
            if (string.IsNullOrEmpty(configAssetPath))
            {
                throw new ArgumentException("Config asset path is null or empty.", nameof(configAssetPath));
            }
            
            // TODO: 实现保存配置到资源的逻辑
            // 这里可以使用Unity的序列化功能来保存配置文件
        }
    }
}