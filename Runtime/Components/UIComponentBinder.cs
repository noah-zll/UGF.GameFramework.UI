using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI组件绑定器，用于标记UI组件以便代码访问
    /// </summary>
    [DisallowMultipleComponent]
    public class UIComponentBinder : MonoBehaviour
    {
        [SerializeField] private string m_ComponentTypeName = "";
        [SerializeField] private bool m_BindEvents = true;
        [SerializeField] private string m_ComponentPath;
        
        private Component m_CachedComponent;
        private Component[] m_AvailableComponents;
        
        /// <summary>
        /// 组件类型名称
        /// </summary>
        public string ComponentTypeName
        {
            get => m_ComponentTypeName;
            set => m_ComponentTypeName = value;
        }
        
        /// <summary>
        /// 是否绑定事件
        /// </summary>
        public bool BindEvents
        {
            get => m_BindEvents;
            set => m_BindEvents = value;
        }
        
        /// <summary>
        /// 组件路径
        /// </summary>
        public string ComponentPath
        {
            get => m_ComponentPath;
            set => m_ComponentPath = value;
        }
        
        /// <summary>
        /// 获取当前GameObject上的所有组件
        /// </summary>
        public Component[] AvailableComponents
        {
            get
            {
                if (m_AvailableComponents == null)
                {
                    UpdateAvailableComponents();
                }
                return m_AvailableComponents;
            }
        }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid => !string.IsNullOrEmpty(m_ComponentTypeName) && GetTargetComponent() != null;
        
        private void Awake()
        {
            UpdateComponentInfo();
        }
        
        private void OnValidate()
        {
            UpdateComponentInfo();
        }
        
        /// <summary>
        /// 更新组件信息
        /// </summary>
        public void UpdateComponentInfo()
        {
            // 更新可用组件列表
            UpdateAvailableComponents();
            
            // 如果没有选择组件类型，自动选择第一个UI组件
            if (string.IsNullOrEmpty(m_ComponentTypeName) && m_AvailableComponents != null && m_AvailableComponents.Length > 0)
            {
                var uiComponent = GetFirstUIComponent();
                if (uiComponent != null)
                {
                    m_ComponentTypeName = uiComponent.GetType().Name;
                }
            }
            
            // 生成组件路径
            m_ComponentPath = GenerateComponentPath();
            
            // 清除缓存
            m_CachedComponent = null;
        }
        
        /// <summary>
        /// 获取目标组件
        /// </summary>
        /// <returns>目标组件</returns>
        public Component GetTargetComponent()
        {
            if (m_CachedComponent == null && !string.IsNullOrEmpty(m_ComponentTypeName))
            {
                m_CachedComponent = GetComponentByTypeName(m_ComponentTypeName);
            }
            return m_CachedComponent;
        }
        
        /// <summary>
        /// 获取指定类型的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件实例</returns>
        public new T GetComponent<T>() where T : Component
        {
            return GetTargetComponent() as T;
        }
        
        /// <summary>
        /// 更新可用组件列表
        /// </summary>
        private void UpdateAvailableComponents()
        {
            if (gameObject != null)
            {
                Component[] allComponents = gameObject.GetComponents<Component>();
                // 排除UIComponentBinder自身
                m_AvailableComponents = allComponents.Where(c => c.GetType() != typeof(UIComponentBinder)).ToArray();
            }
            else
            {
                m_AvailableComponents = new Component[0];
            }
        }
        
        /// <summary>
        /// 根据类型名称获取组件
        /// </summary>
        /// <param name="typeName">组件类型名称</param>
        /// <returns>组件实例</returns>
        private Component GetComponentByTypeName(string typeName)
        {
            if (m_AvailableComponents == null)
            {
                UpdateAvailableComponents();
            }
            
            foreach (var component in m_AvailableComponents)
            {
                if (component.GetType().Name == typeName)
                {
                    return component;
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取第一个UI组件
        /// </summary>
        /// <returns>第一个UI组件</returns>
        private Component GetFirstUIComponent()
        {
            if (m_AvailableComponents == null) return null;
            
            // 按优先级查找UI组件
            var uiComponentTypes = new System.Type[]
            {
                typeof(Button), typeof(Toggle), typeof(Slider), typeof(Scrollbar),
                typeof(Dropdown), typeof(InputField), typeof(ScrollRect),
                typeof(Text), typeof(Image), typeof(RawImage), typeof(CanvasGroup)
            };
            
            foreach (var uiType in uiComponentTypes)
            {
                foreach (var component in m_AvailableComponents)
                {
                    if (component.GetType() == uiType)
                    {
                        return component;
                    }
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 生成组件路径
        /// </summary>
        /// <returns>组件路径</returns>
        private string GenerateComponentPath()
        {
            var path = gameObject.name;
            var current = transform.parent;
            
            while (current != null)
            {
                // 如果找到Canvas或UIFormBase，停止向上查找
                if (current.GetComponent<Canvas>() != null || current.GetComponent<UIFormBase>() != null)
                {
                    break;
                }
                
                path = current.name + "/" + path;
                current = current.parent;
            }
            
            return path;
        }
        

    }
}