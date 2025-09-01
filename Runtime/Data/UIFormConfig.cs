using System;
using UnityEngine;

namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI窗体配置
    /// </summary>
    [Serializable]
    public class UIFormConfig
    {
        [SerializeField] private string m_FormName;
        [SerializeField] private string m_AssetPath;
        [SerializeField] private int m_Priority = 0;
        [SerializeField] private bool m_AllowMultipleInstances = false;
        [SerializeField] private bool m_PausesCoveredUIForm = true;
        [SerializeField] private UIFormShowMode m_ShowMode = UIFormShowMode.Normal;
        [SerializeField] private string m_UIGroupName = "Default";
        [SerializeField] private float m_FadeInDuration = 0.3f;
        [SerializeField] private float m_FadeOutDuration = 0.3f;
        [SerializeField] private AnimationCurve m_FadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve m_FadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        
        /// <summary>
        /// 窗体名称
        /// </summary>
        public string FormName
        {
            get => m_FormName;
            set => m_FormName = value;
        }
        
        /// <summary>
        /// 资源路径
        /// </summary>
        public string AssetPath
        {
            get => m_AssetPath;
            set => m_AssetPath = value;
        }
        
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority
        {
            get => m_Priority;
            set => m_Priority = value;
        }
        
        /// <summary>
        /// 是否允许多实例
        /// </summary>
        public bool AllowMultipleInstances
        {
            get => m_AllowMultipleInstances;
            set => m_AllowMultipleInstances = value;
        }
        
        /// <summary>
        /// 是否暂停被覆盖的UI窗体
        /// </summary>
        public bool PausesCoveredUIForm
        {
            get => m_PausesCoveredUIForm;
            set => m_PausesCoveredUIForm = value;
        }
        
        /// <summary>
        /// 显示模式
        /// </summary>
        public UIFormShowMode ShowMode
        {
            get => m_ShowMode;
            set => m_ShowMode = value;
        }
        
        /// <summary>
        /// UI组名称
        /// </summary>
        public string UIGroupName
        {
            get => m_UIGroupName;
            set => m_UIGroupName = value;
        }
        
        /// <summary>
        /// 淡入持续时间
        /// </summary>
        public float FadeInDuration
        {
            get => m_FadeInDuration;
            set => m_FadeInDuration = value;
        }
        
        /// <summary>
        /// 淡出持续时间
        /// </summary>
        public float FadeOutDuration
        {
            get => m_FadeOutDuration;
            set => m_FadeOutDuration = value;
        }
        
        /// <summary>
        /// 淡入曲线
        /// </summary>
        public AnimationCurve FadeInCurve
        {
            get => m_FadeInCurve;
            set => m_FadeInCurve = value;
        }
        
        /// <summary>
        /// 淡出曲线
        /// </summary>
        public AnimationCurve FadeOutCurve
        {
            get => m_FadeOutCurve;
            set => m_FadeOutCurve = value;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public UIFormConfig()
        {
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="formName">窗体名称</param>
        /// <param name="assetPath">资源路径</param>
        public UIFormConfig(string formName, string assetPath)
        {
            m_FormName = formName;
            m_AssetPath = assetPath;
        }
        
        /// <summary>
        /// 克隆配置
        /// </summary>
        /// <returns>克隆的配置</returns>
        public UIFormConfig Clone()
        {
            return new UIFormConfig
            {
                m_FormName = m_FormName,
                m_AssetPath = m_AssetPath,
                m_Priority = m_Priority,
                m_AllowMultipleInstances = m_AllowMultipleInstances,
                m_PausesCoveredUIForm = m_PausesCoveredUIForm,
                m_ShowMode = m_ShowMode,
                m_UIGroupName = m_UIGroupName,
                m_FadeInDuration = m_FadeInDuration,
                m_FadeOutDuration = m_FadeOutDuration,
                m_FadeInCurve = new AnimationCurve(m_FadeInCurve.keys),
                m_FadeOutCurve = new AnimationCurve(m_FadeOutCurve.keys)
            };
        }
    }
    
    /// <summary>
    /// UI窗体显示模式
    /// </summary>
    public enum UIFormShowMode
    {
        /// <summary>
        /// 普通模式
        /// </summary>
        Normal,
        
        /// <summary>
        /// 模态模式
        /// </summary>
        Modal,
        
        /// <summary>
        /// 全屏模式
        /// </summary>
        Fullscreen,
        
        /// <summary>
        /// 弹窗模式
        /// </summary>
        Popup
    }
}