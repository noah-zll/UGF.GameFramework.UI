using System;
using UnityEngine;

namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI全局配置
    /// </summary>
    [Serializable]
    public class UIGlobalConfig
    {
        [SerializeField] private bool m_EnableUISound = true;
        [SerializeField] private bool m_EnableUIAnimation = true;
        [SerializeField] private bool m_EnableUIParticle = true;
        [SerializeField] private float m_DefaultFadeDuration = 0.3f;
        [SerializeField] private string m_DefaultUIGroupName = "Default";
        [SerializeField] private int m_MaxUIFormCount = 100;
        [SerializeField] private bool m_AutoReleaseUnusedUIForm = true;
        [SerializeField] private float m_UIFormReleaseDelay = 60f;
        [SerializeField] private bool m_EnableUIFormPool = true;
        [SerializeField] private int m_UIFormPoolCapacity = 10;
        
        /// <summary>
        /// 是否启用UI音效
        /// </summary>
        public bool EnableUISound
        {
            get => m_EnableUISound;
            set => m_EnableUISound = value;
        }
        
        /// <summary>
        /// 是否启用UI动画
        /// </summary>
        public bool EnableUIAnimation
        {
            get => m_EnableUIAnimation;
            set => m_EnableUIAnimation = value;
        }
        
        /// <summary>
        /// 是否启用UI粒子效果
        /// </summary>
        public bool EnableUIParticle
        {
            get => m_EnableUIParticle;
            set => m_EnableUIParticle = value;
        }
        
        /// <summary>
        /// 默认淡入淡出持续时间
        /// </summary>
        public float DefaultFadeDuration
        {
            get => m_DefaultFadeDuration;
            set => m_DefaultFadeDuration = Mathf.Max(0f, value);
        }
        
        /// <summary>
        /// 默认UI组名称
        /// </summary>
        public string DefaultUIGroupName
        {
            get => m_DefaultUIGroupName;
            set => m_DefaultUIGroupName = value ?? "Default";
        }
        
        /// <summary>
        /// 最大UI窗体数量
        /// </summary>
        public int MaxUIFormCount
        {
            get => m_MaxUIFormCount;
            set => m_MaxUIFormCount = Mathf.Max(1, value);
        }
        
        /// <summary>
        /// 是否自动释放未使用的UI窗体
        /// </summary>
        public bool AutoReleaseUnusedUIForm
        {
            get => m_AutoReleaseUnusedUIForm;
            set => m_AutoReleaseUnusedUIForm = value;
        }
        
        /// <summary>
        /// UI窗体释放延迟时间
        /// </summary>
        public float UIFormReleaseDelay
        {
            get => m_UIFormReleaseDelay;
            set => m_UIFormReleaseDelay = Mathf.Max(0f, value);
        }
        
        /// <summary>
        /// 是否启用UI窗体对象池
        /// </summary>
        public bool EnableUIFormPool
        {
            get => m_EnableUIFormPool;
            set => m_EnableUIFormPool = value;
        }
        
        /// <summary>
        /// UI窗体对象池容量
        /// </summary>
        public int UIFormPoolCapacity
        {
            get => m_UIFormPoolCapacity;
            set => m_UIFormPoolCapacity = Mathf.Max(1, value);
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public UIGlobalConfig()
        {
        }
        
        /// <summary>
        /// 克隆配置
        /// </summary>
        /// <returns>克隆的配置</returns>
        public UIGlobalConfig Clone()
        {
            return new UIGlobalConfig
            {
                m_EnableUISound = m_EnableUISound,
                m_EnableUIAnimation = m_EnableUIAnimation,
                m_EnableUIParticle = m_EnableUIParticle,
                m_DefaultFadeDuration = m_DefaultFadeDuration,
                m_DefaultUIGroupName = m_DefaultUIGroupName,
                m_MaxUIFormCount = m_MaxUIFormCount,
                m_AutoReleaseUnusedUIForm = m_AutoReleaseUnusedUIForm,
                m_UIFormReleaseDelay = m_UIFormReleaseDelay,
                m_EnableUIFormPool = m_EnableUIFormPool,
                m_UIFormPoolCapacity = m_UIFormPoolCapacity
            };
        }
    }
    
    /// <summary>
    /// UI组配置
    /// </summary>
    [Serializable]
    public class UIGroupConfig
    {
        [SerializeField] private string m_GroupName;
        [SerializeField] private int m_Depth = 0;
        [SerializeField] private bool m_Pause = true;
        [SerializeField] private Canvas m_CanvasTemplate;
        [SerializeField] private int m_SortingOrder = 0;
        [SerializeField] private string m_SortingLayerName = "Default";
        
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName
        {
            get => m_GroupName;
            set => m_GroupName = value;
        }
        
        /// <summary>
        /// 深度
        /// </summary>
        public int Depth
        {
            get => m_Depth;
            set => m_Depth = value;
        }
        
        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool Pause
        {
            get => m_Pause;
            set => m_Pause = value;
        }
        
        /// <summary>
        /// Canvas模板
        /// </summary>
        public Canvas CanvasTemplate
        {
            get => m_CanvasTemplate;
            set => m_CanvasTemplate = value;
        }
        
        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortingOrder
        {
            get => m_SortingOrder;
            set => m_SortingOrder = value;
        }
        
        /// <summary>
        /// 排序层名称
        /// </summary>
        public string SortingLayerName
        {
            get => m_SortingLayerName;
            set => m_SortingLayerName = value ?? "Default";
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public UIGroupConfig()
        {
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="groupName">组名称</param>
        /// <param name="depth">深度</param>
        public UIGroupConfig(string groupName, int depth)
        {
            m_GroupName = groupName;
            m_Depth = depth;
        }
        
        /// <summary>
        /// 克隆配置
        /// </summary>
        /// <returns>克隆的配置</returns>
        public UIGroupConfig Clone()
        {
            return new UIGroupConfig
            {
                m_GroupName = m_GroupName,
                m_Depth = m_Depth,
                m_Pause = m_Pause,
                m_CanvasTemplate = m_CanvasTemplate,
                m_SortingOrder = m_SortingOrder,
                m_SortingLayerName = m_SortingLayerName
            };
        }
    }
}