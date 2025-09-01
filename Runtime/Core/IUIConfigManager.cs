namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI配置管理器接口
    /// </summary>
    public interface IUIConfigManager
    {
        /// <summary>
        /// 全局配置
        /// </summary>
        UIGlobalConfig GlobalConfig { get; set; }
        
        /// <summary>
        /// 添加UI窗体配置
        /// </summary>
        /// <param name="config">UI窗体配置</param>
        void AddUIFormConfig(UIFormConfig config);
        
        /// <summary>
        /// 移除UI窗体配置
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <returns>是否移除成功</returns>
        bool RemoveUIFormConfig(string formName);
        
        /// <summary>
        /// 获取UI窗体配置
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <returns>UI窗体配置</returns>
        UIFormConfig GetUIFormConfig(string formName);
        
        /// <summary>
        /// 获取所有UI窗体配置
        /// </summary>
        /// <returns>所有UI窗体配置</returns>
        UIFormConfig[] GetAllUIFormConfigs();
        
        /// <summary>
        /// 是否存在UI窗体配置
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <returns>是否存在</returns>
        bool HasUIFormConfig(string formName);
        
        /// <summary>
        /// 添加UI组配置
        /// </summary>
        /// <param name="config">UI组配置</param>
        void AddUIGroupConfig(UIGroupConfig config);
        
        /// <summary>
        /// 移除UI组配置
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>是否移除成功</returns>
        bool RemoveUIGroupConfig(string groupName);
        
        /// <summary>
        /// 获取UI组配置
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>UI组配置</returns>
        UIGroupConfig GetUIGroupConfig(string groupName);
        
        /// <summary>
        /// 获取所有UI组配置
        /// </summary>
        /// <returns>所有UI组配置</returns>
        UIGroupConfig[] GetAllUIGroupConfigs();
        
        /// <summary>
        /// 是否存在UI组配置
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <returns>是否存在</returns>
        bool HasUIGroupConfig(string groupName);
        
        /// <summary>
        /// 清空所有配置
        /// </summary>
        void ClearAllConfigs();
        
        /// <summary>
        /// 从资源加载配置
        /// </summary>
        /// <param name="configAssetPath">配置资源路径</param>
        void LoadConfigFromAsset(string configAssetPath);
        
        /// <summary>
        /// 保存配置到资源
        /// </summary>
        /// <param name="configAssetPath">配置资源路径</param>
        void SaveConfigToAsset(string configAssetPath);
    }
}