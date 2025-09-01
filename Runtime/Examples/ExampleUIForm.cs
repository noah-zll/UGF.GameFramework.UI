using UnityEngine;
using UnityEngine.UI;
using UGF.GameFramework.UI;
using TMPro;
using UnityGameFramework.Runtime;

namespace UGF.GameFramework.UI.Examples
{
    /// <summary>
    /// 示例UI窗体 - 演示组件绑定系统的使用
    /// </summary>
    public class ExampleUIForm : UIFormBase
    {
        // 这些组件将通过UIComponentBinder自动绑定
        private Button closeButton;
        private Button confirmButton;
        private Button cancelButton;
        private Text titleText;
        private Text contentText;
        private InputField nameInput;
        private Toggle agreeToggle;
        private Slider volumeSlider;
        private Image iconImage;
        private TextMeshProUGUI tmpText;
        
        protected override void OnUIFormInit(object userData)
        {
            base.OnUIFormInit(userData);
            
            // 初始化UI数据
            InitializeUI();
        }
        
        protected override void BindEvents()
        {
            base.BindEvents();
            
            // 绑定按钮事件
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }
            
            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmButtonClick);
            }
            
            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelButtonClick);
            }
            
            // 绑定Toggle事件
            if (agreeToggle != null)
            {
                agreeToggle.onValueChanged.AddListener(OnAgreeToggleChanged);
            }
            
            // 绑定Slider事件
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
            }
            
            // 绑定InputField事件
            if (nameInput != null)
            {
                nameInput.onValueChanged.AddListener(OnNameInputChanged);
                nameInput.onEndEdit.AddListener(OnNameInputEndEdit);
            }
        }
        
        protected override void UnbindEvents()
        {
            base.UnbindEvents();
            
            // 解绑按钮事件
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(OnCloseButtonClick);
            }
            
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(OnConfirmButtonClick);
            }
            
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(OnCancelButtonClick);
            }
            
            // 解绑Toggle事件
            if (agreeToggle != null)
            {
                agreeToggle.onValueChanged.RemoveListener(OnAgreeToggleChanged);
            }
            
            // 解绑Slider事件
            if (volumeSlider != null)
            {
                volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChanged);
            }
            
            // 解绑InputField事件
            if (nameInput != null)
            {
                nameInput.onValueChanged.RemoveListener(OnNameInputChanged);
                nameInput.onEndEdit.RemoveListener(OnNameInputEndEdit);
            }
        }
        
        /// <summary>
        /// 初始化UI
        /// </summary>
        private void InitializeUI()
        {
            // 设置标题
            if (titleText != null)
            {
                titleText.text = "示例UI窗体";
            }
            
            // 设置内容
            if (contentText != null)
            {
                contentText.text = "这是一个演示组件绑定系统的示例窗体。";
            }
            
            // 设置TMP文本
            if (tmpText != null)
            {
                tmpText.text = "TextMeshPro 文本组件";
            }
            
            // 设置输入框占位符
            if (nameInput != null)
            {
                var placeholder = nameInput.placeholder as Text;
                if (placeholder != null)
                {
                    placeholder.text = "请输入姓名";
                }
            }
            
            // 设置滑块初始值
            if (volumeSlider != null)
            {
                volumeSlider.value = 0.5f;
            }
            
            // 设置Toggle初始状态
            if (agreeToggle != null)
            {
                agreeToggle.isOn = false;
            }
            
            // 更新确认按钮状态
            UpdateConfirmButtonState();
        }
        
        /// <summary>
        /// 更新确认按钮状态
        /// </summary>
        private void UpdateConfirmButtonState()
        {
            if (confirmButton != null && agreeToggle != null)
            {
                confirmButton.interactable = agreeToggle.isOn;
            }
        }
        
        #region 事件处理
        
        /// <summary>
        /// 关闭按钮点击事件
        /// </summary>
        private void OnCloseButtonClick()
        {
            Debug.Log("ExampleUIForm: 关闭按钮被点击");
            Close();
        }
        
        /// <summary>
        /// 确认按钮点击事件
        /// </summary>
        private void OnConfirmButtonClick()
        {
            var name = nameInput != null ? nameInput.text : "未知";
            var volume = volumeSlider != null ? volumeSlider.value : 0f;
            
            Debug.Log($"ExampleUIForm: 确认操作 - 姓名: {name}, 音量: {volume:F2}");
            
            // 这里可以添加具体的确认逻辑
            Close();
        }
        
        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void OnCancelButtonClick()
        {
            Debug.Log("ExampleUIForm: 取消按钮被点击");
            Close();
        }
        
        /// <summary>
        /// 同意Toggle状态改变事件
        /// </summary>
        /// <param name="isOn">是否选中</param>
        private void OnAgreeToggleChanged(bool isOn)
        {
            Debug.Log($"ExampleUIForm: 同意状态改变 - {isOn}");
            UpdateConfirmButtonState();
        }
        
        /// <summary>
        /// 音量滑块值改变事件
        /// </summary>
        /// <param name="value">滑块值</param>
        private void OnVolumeSliderChanged(float value)
        {
            Debug.Log($"ExampleUIForm: 音量改变 - {value:F2}");
        }
        
        /// <summary>
        /// 姓名输入框值改变事件
        /// </summary>
        /// <param name="value">输入值</param>
        private void OnNameInputChanged(string value)
        {
            Debug.Log($"ExampleUIForm: 姓名输入改变 - {value}");
        }
        
        /// <summary>
        /// 姓名输入框编辑结束事件
        /// </summary>
        /// <param name="value">最终输入值</param>
        private void OnNameInputEndEdit(string value)
        {
            Debug.Log($"ExampleUIForm: 姓名输入完成 - {value}");
        }
        
        #endregion
        
        #region 窗体控制
        
        /// <summary>
        /// 关闭窗体
        /// </summary>
        protected void Close()
        {
            // 通过UIComponent关闭当前窗体
            var uiComponent = GameEntry.GetComponent<UIComponent>();
            if (uiComponent != null)
            {
                // 通过UIForm的SerialId属性关闭窗体
                uiComponent.CloseUIForm(UIForm.SerialId);
            }
        }
        
        #endregion
        
        #region 公共方法
        
        /// <summary>
        /// 设置窗体数据
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        public void SetData(string title, string content)
        {
            if (titleText != null)
            {
                titleText.text = title;
            }
            
            if (contentText != null)
            {
                contentText.text = content;
            }
        }
        
        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="sprite">图标精灵</param>
        public void SetIcon(Sprite sprite)
        {
            if (iconImage != null)
            {
                iconImage.sprite = sprite;
                iconImage.gameObject.SetActive(sprite != null);
            }
        }
        
        /// <summary>
        /// 获取用户输入的姓名
        /// </summary>
        /// <returns>姓名</returns>
        public string GetInputName()
        {
            return nameInput != null ? nameInput.text : string.Empty;
        }
        
        /// <summary>
        /// 获取音量设置
        /// </summary>
        /// <returns>音量值</returns>
        public float GetVolumeValue()
        {
            return volumeSlider != null ? volumeSlider.value : 0f;
        }
        
        /// <summary>
        /// 获取同意状态
        /// </summary>
        /// <returns>是否同意</returns>
        public bool GetAgreeState()
        {
            return agreeToggle != null && agreeToggle.isOn;
        }
        
        #endregion
    }
}