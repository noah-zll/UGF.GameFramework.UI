using System;
using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using GameFramework.UI;
using UnityGameFramework.Runtime;

namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI扩展方法
    /// </summary>
    public static class UIExtensions
    {
        /// <summary>
        /// 获取UI配置管理器
        /// </summary>
        /// <returns>UI配置管理器</returns>
        public static IUIConfigManager GetUIConfigManager()
        {
            return GameEntry.GetComponent<UIConfigManager>();
        }
        
        /// <summary>
        /// 打开UI窗体（使用配置）
        /// </summary>
        /// <param name="uiComponent">UI组件</param>
        /// <param name="formName">窗体名称</param>
        /// <param name="userData">用户数据</param>
        /// <returns>UI窗体序列编号</returns>
        public static int OpenUIFormWithConfig(this UIComponent uiComponent, string formName, object userData = null)
        {
            if (uiComponent == null)
            {
                throw new ArgumentNullException(nameof(uiComponent));
            }
            
            var configManager = GetUIConfigManager();
            var config = configManager?.GetUIFormConfig(formName);
            
            if (config == null)
            {
                Log.Error($"UI form config '{formName}' not found.");
                return -1;
            }
            
            return uiComponent.OpenUIForm(config.AssetPath, config.UIGroupName, config.Priority, config.PausesCoveredUIForm, userData);
        }
        
        /// <summary>
        /// 设置UI窗体配置
        /// </summary>
        /// <param name="uiForm">UI窗体</param>
        /// <param name="config">配置</param>
        public static void SetConfig(this UIFormBase uiForm, UIFormConfig config)
        {
            if (uiForm == null)
            {
                throw new ArgumentNullException(nameof(uiForm));
            }
            
            uiForm.SetConfig(config);
        }
        
        /// <summary>
        /// 获取UI窗体配置
        /// </summary>
        /// <param name="uiForm">UI窗体</param>
        /// <returns>配置</returns>
        public static UIFormConfig GetConfig(this UIFormBase uiForm)
        {
            if (uiForm == null)
            {
                throw new ArgumentNullException(nameof(uiForm));
            }
            
            return uiForm.Config;
        }
        
        /// <summary>
        /// 安全获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="uiForm">UI窗体</param>
        /// <param name="variableName">变量名</param>
        /// <returns>组件实例</returns>
        public static T SafeGetComponent<T>(this UIFormBase uiForm, string variableName) where T : Component
        {
            if (uiForm == null)
            {
                Log.Warning("UIForm is null.");
                return null;
            }
            
            if (string.IsNullOrEmpty(variableName))
            {
                Log.Warning("Variable name is null or empty.");
                return null;
            }
            
            var component = uiForm.GetComponent<T>(variableName);
            if (component == null)
            {
                Log.Warning($"Component '{variableName}' of type '{typeof(T).Name}' not found in UI form '{uiForm.name}'.");
            }
            
            return component;
        }
        
        /// <summary>
        /// 设置按钮点击事件
        /// </summary>
        /// <param name="button">按钮</param>
        /// <param name="onClick">点击事件</param>
        public static void SetOnClick(this Button button, UnityEngine.Events.UnityAction onClick)
        {
            if (button == null)
            {
                Log.Warning("Button is null.");
                return;
            }
            
            button.onClick.RemoveAllListeners();
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }
        }
        
        /// <summary>
        /// 设置切换按钮值改变事件
        /// </summary>
        /// <param name="toggle">切换按钮</param>
        /// <param name="onValueChanged">值改变事件</param>
        public static void SetOnValueChanged(this Toggle toggle, UnityEngine.Events.UnityAction<bool> onValueChanged)
        {
            if (toggle == null)
            {
                Log.Warning("Toggle is null.");
                return;
            }
            
            toggle.onValueChanged.RemoveAllListeners();
            if (onValueChanged != null)
            {
                toggle.onValueChanged.AddListener(onValueChanged);
            }
        }
        
        /// <summary>
        /// 设置滑动条值改变事件
        /// </summary>
        /// <param name="slider">滑动条</param>
        /// <param name="onValueChanged">值改变事件</param>
        public static void SetOnValueChanged(this Slider slider, UnityEngine.Events.UnityAction<float> onValueChanged)
        {
            if (slider == null)
            {
                Log.Warning("Slider is null.");
                return;
            }
            
            slider.onValueChanged.RemoveAllListeners();
            if (onValueChanged != null)
            {
                slider.onValueChanged.AddListener(onValueChanged);
            }
        }
        
        /// <summary>
        /// 设置输入框值改变事件
        /// </summary>
        /// <param name="inputField">输入框</param>
        /// <param name="onValueChanged">值改变事件</param>
        public static void SetOnValueChanged(this InputField inputField, UnityEngine.Events.UnityAction<string> onValueChanged)
        {
            if (inputField == null)
            {
                Log.Warning("InputField is null.");
                return;
            }
            
            inputField.onValueChanged.RemoveAllListeners();
            if (onValueChanged != null)
            {
                inputField.onValueChanged.AddListener(onValueChanged);
            }
        }
        
        /// <summary>
        /// 设置输入框结束编辑事件
        /// </summary>
        /// <param name="inputField">输入框</param>
        /// <param name="onEndEdit">结束编辑事件</param>
        public static void SetOnEndEdit(this InputField inputField, UnityEngine.Events.UnityAction<string> onEndEdit)
        {
            if (inputField == null)
            {
                Log.Warning("InputField is null.");
                return;
            }
            
            inputField.onEndEdit.RemoveAllListeners();
            if (onEndEdit != null)
            {
                inputField.onEndEdit.AddListener(onEndEdit);
            }
        }
        
        /// <summary>
        /// 设置下拉框值改变事件
        /// </summary>
        /// <param name="dropdown">下拉框</param>
        /// <param name="onValueChanged">值改变事件</param>
        public static void SetOnValueChanged(this Dropdown dropdown, UnityEngine.Events.UnityAction<int> onValueChanged)
        {
            if (dropdown == null)
            {
                Log.Warning("Dropdown is null.");
                return;
            }
            
            dropdown.onValueChanged.RemoveAllListeners();
            if (onValueChanged != null)
            {
                dropdown.onValueChanged.AddListener(onValueChanged);
            }
        }
        
        /// <summary>
        /// 设置UI元素激活状态
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <param name="active">是否激活</param>
        public static void SetActive(this GameObject gameObject, bool active)
        {
            if (gameObject == null)
            {
                Log.Warning("GameObject is null.");
                return;
            }
            
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
        }
        
        /// <summary>
        /// 设置UI元素可交互状态
        /// </summary>
        /// <param name="selectable">可选择组件</param>
        /// <param name="interactable">是否可交互</param>
        public static void SetInteractable(this Selectable selectable, bool interactable)
        {
            if (selectable == null)
            {
                Log.Warning("Selectable is null.");
                return;
            }
            
            selectable.interactable = interactable;
        }
        
        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="text">文本组件</param>
        /// <param name="content">文本内容</param>
        public static void SetText(this Text text, string content)
        {
            if (text == null)
            {
                Log.Warning("Text is null.");
                return;
            }
            
            text.text = content ?? string.Empty;
        }
        
        /// <summary>
        /// 设置图片精灵
        /// </summary>
        /// <param name="image">图片组件</param>
        /// <param name="sprite">精灵</param>
        public static void SetSprite(this Image image, Sprite sprite)
        {
            if (image == null)
            {
                Log.Warning("Image is null.");
                return;
            }
            
            image.sprite = sprite;
        }
        
        /// <summary>
        /// 设置图片颜色
        /// </summary>
        /// <param name="graphic">图形组件</param>
        /// <param name="color">颜色</param>
        public static void SetColor(this Graphic graphic, Color color)
        {
            if (graphic == null)
            {
                Log.Warning("Graphic is null.");
                return;
            }
            
            graphic.color = color;
        }
    }
}