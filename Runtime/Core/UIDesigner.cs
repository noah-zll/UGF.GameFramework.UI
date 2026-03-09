using UnityEngine;
using System;
using System.Collections.Generic;

namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI基类类型
    /// </summary>
    public enum UIBaseType
    {
        UIFormBase,
        MonoBehaviour
    }

    /// <summary>
    /// UI设计器脚本，用于标记UI界面并生成组件绑定代码
    /// 该脚本应该添加到UI预制体的根节点上
    /// </summary>
    [System.Serializable]
    public class UIDesigner : MonoBehaviour
    {
        [Header("UI设计信息")]
        [SerializeField] private string uiFormName = "";
        [SerializeField] private string namespaceName = "";
        [SerializeField] private UIBaseType baseType = UIBaseType.UIFormBase;
        [SerializeField] private bool autoGenerateCode = true;

        [Header("组件绑定列表")]
        [SerializeField] private List<UIComponentBinding> componentBindings = new List<UIComponentBinding>();

        /// <summary>
        /// UI界面名称
        /// </summary>
        public string UIFormName
        {
            get => string.IsNullOrEmpty(uiFormName) ? gameObject.name : uiFormName;
            set => uiFormName = value;
        }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string NamespaceName
        {
            get => string.IsNullOrEmpty(namespaceName) ? "Game.UI" : namespaceName;
            set => namespaceName = value;
        }

        /// <summary>
        /// UI基类类型
        /// </summary>
        public UIBaseType BaseType
        {
            get => baseType;
            set => baseType = value;
        }

        /// <summary>
        /// 是否自动生成代码
        /// </summary>
        public bool AutoGenerateCode
        {
            get => autoGenerateCode;
            set => autoGenerateCode = value;
        }

        /// <summary>
        /// 组件绑定列表
        /// </summary>
        public List<UIComponentBinding> ComponentBindings
        {
            get => componentBindings;
            set => componentBindings = value;
        }

        /// <summary>
        /// 添加组件绑定
        /// </summary>
        /// <param name="binding">组件绑定信息</param>
        public void AddComponentBinding(UIComponentBinding binding)
        {
            if (binding != null && !componentBindings.Contains(binding))
            {
                componentBindings.Add(binding);
            }
        }

        /// <summary>
        /// 移除组件绑定
        /// </summary>
        /// <param name="binding">组件绑定信息</param>
        public void RemoveComponentBinding(UIComponentBinding binding)
        {
            if (binding != null)
            {
                componentBindings.Remove(binding);
            }
        }

        /// <summary>
        /// 清空组件绑定
        /// </summary>
        public void ClearComponentBindings()
        {
            componentBindings.Clear();
        }

        /// <summary>
        /// 获取生成的组件绑定类名
        /// </summary>
        public string GetBindingClassName()
        {
            return $"{UIFormName}Binding";
        }

        /// <summary>
        /// 获取生成的业务逻辑类名
        /// </summary>
        public string GetLogicClassName()
        {
            return UIFormName;
        }

        /// <summary>
        /// 验证设置
        /// </summary>
        /// <returns>验证结果</returns>
        public bool ValidateSettings()
        {
            if (string.IsNullOrEmpty(UIFormName))
            {
                Debug.LogError("UI界面名称不能为空", this);
                return false;
            }

            if (string.IsNullOrEmpty(NamespaceName))
            {
                Debug.LogError("命名空间不能为空", this);
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// UI组件绑定信息
    /// </summary>
    [System.Serializable]
    public class UIComponentBinding
    {
        [SerializeField] private string componentName = "";
        [SerializeField] private Component component = null;
        [SerializeField] private string componentType = "";
        [SerializeField] private string description = "";
        [SerializeField] private List<string> boundEvents = new List<string>();
        // fieldName字段已移除，直接使用componentName生成字段名

        /// <summary>
        /// 组件名称（用作变量名）
        /// </summary>
        public string ComponentName
        {
            get => componentName;
            set => componentName = value;
        }

        /// <summary>
        /// 绑定的事件列表
        /// </summary>
        public List<string> BoundEvents
        {
            get => boundEvents;
            set => boundEvents = value;
        }

        /// <summary>
        /// 绑定的组件
        /// </summary>
        public Component Component
        {
            get => component;
            set
            {
                component = value;
                if (component != null)
                {
                    componentType = component.GetType().Name;
                }
            }
        }

        /// <summary>
        /// 序列化字段名（基于组件名自动生成）
        /// </summary>
        public string FieldName
        {
            get
            {
                return componentName;
            }
        }

        /// <summary>
        /// 组件类型名称
        /// </summary>
        public string ComponentType
        {
            get => componentType;
            set => componentType = value;
        }


        /// <summary>
        /// 描述信息
        /// </summary>
        public string Description
        {
            get => description;
            set => description = value;
        }

        /// <summary>
        /// 验证绑定是否有效
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(componentName) && component != null;
        }

        /// <summary>
        /// 获取序列化字段声明代码
        /// </summary>
        public string GetSerializedFieldDeclaration()
        {
            return $"[SerializeField] private {componentType} {componentName};";
        }

        /// <summary>
        /// 获取属性声明代码
        /// </summary>
        public string GetPropertyDeclaration()
        {
            string propertyName = char.ToUpper(componentName[0]) + componentName.Substring(1);
            return $"public {componentType} {propertyName} => {componentName};";
        }
    }
}
