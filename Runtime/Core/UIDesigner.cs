using UnityEngine;
using System;
using System.Collections.Generic;

namespace UGF.GameFramework.UI
{
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
        /// 验证绑定信息
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(componentName) || string.IsNullOrEmpty(componentType))
                return false;
                
            return component != null;
        }
        
        /// <summary>
        /// 获取序列化字段声明代码
        /// </summary>
        /// <returns>序列化字段声明代码</returns>
        public string GetSerializedFieldDeclaration()
        {
            if (!IsValid()) return string.Empty;
            
            return $"[SerializeField] private {ComponentType} {FieldName};";
        }
        
        /// <summary>
        /// 获取属性声明代码
        /// </summary>
        /// <returns>属性声明代码</returns>
        public string GetPropertyDeclaration()
        {
            if (!IsValid()) return string.Empty;
            
            // 属性名使用首字母大写的ComponentName，字段名使用FieldName
            string propertyName = char.ToUpper(ComponentName[0]) + ComponentName.Substring(1);
            return $"public {ComponentType} {propertyName} => {FieldName};";
        }
        
        /// <summary>
        /// 获取组件绑定代码（用于初始化序列化字段）
        /// </summary>
        /// <returns>绑定代码</returns>
        public string GetBindingCode()
        {
            if (!IsValid()) return string.Empty;
            
            // 这个方法现在用于在编辑器中设置序列化字段的值
            // 实际的组件引用通过序列化字段直接获取，不需要运行时查找
            return $"// {ComponentName} 通过序列化字段 {FieldName} 直接引用";
        }
    }
}