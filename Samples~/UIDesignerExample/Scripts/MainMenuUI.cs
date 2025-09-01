using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;

namespace Game.UI.Examples
{
    /// <summary>
    /// 主菜单UI - 业务逻辑类
    /// 这个类展示了如何使用新的UIDesigner流程开发UI
    /// </summary>
    public partial class MainMenuUI
    {
        /// <summary>
        /// 自定义初始化逻辑
        /// 在这里绑定事件和初始化UI状态
        /// </summary>
        partial void OnInitCustom(object userData)
        {
            // 绑定按钮事件
            startGameButton.onClick.AddListener(OnStartGameClick);
            settingsButton.onClick.AddListener(OnSettingsClick);
            exitGameButton.onClick.AddListener(OnExitGameClick);
            
            // 绑定滑块事件
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            
            // 初始化UI状态
            InitializeUI();
        }
        
        /// <summary>
        /// 界面打开时调用
        /// </summary>
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            
            // 播放打开动画
            PlayOpenAnimation();
            
            // 刷新UI显示
            RefreshUI();
        }
        
        /// <summary>
        /// 界面关闭时调用
        /// </summary>
        protected override void OnClose(bool isShutdown, object userData)
        {
            // 播放关闭动画
            if (!isShutdown)
            {
                PlayCloseAnimation();
            }
            
            base.OnClose(isShutdown, userData);
        }
        
        /// <summary>
        /// 界面暂停时调用
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            // 暂停相关逻辑
        }
        
        /// <summary>
        /// 界面恢复时调用
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            // 恢复相关逻辑
            RefreshUI();
        }
        
        /// <summary>
        /// 初始化UI状态
        /// </summary>
        private void InitializeUI()
        {
            // 设置版本信息
            if (versionText != null)
            {
                versionText.text = $"Version {Application.version}";
            }
            
            // 设置音量滑块初始值
            if (volumeSlider != null)
            {
                volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
            }
            
            // 设置玩家名称
            if (playerNameText != null)
            {
                string playerName = PlayerPrefs.GetString("PlayerName", "Player");
                playerNameText.text = $"Welcome, {playerName}!";
            }
        }
        
        /// <summary>
        /// 刷新UI显示
        /// </summary>
        private void RefreshUI()
        {
            // 检查是否有存档
            bool hasSaveData = CheckSaveDataExists();
            
            // 根据存档状态更新继续游戏按钮
            if (continueGameButton != null)
            {
                continueGameButton.interactable = hasSaveData;
                
                // 如果没有存档，显示提示
                if (!hasSaveData && noSaveDataText != null)
                {
                    noSaveDataText.gameObject.SetActive(true);
                }
            }
            
            // 更新成就显示
            UpdateAchievementDisplay();
        }
        
        /// <summary>
        /// 播放打开动画
        /// </summary>
        private void PlayOpenAnimation()
        {
            // 使用DOTween或其他动画系统播放打开动画
            if (mainPanel != null)
            {
                mainPanel.transform.localScale = Vector3.zero;
                // mainPanel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
            
            // 按钮依次出现动画
            AnimateButtonsSequentially();
        }
        
        /// <summary>
        /// 播放关闭动画
        /// </summary>
        private void PlayCloseAnimation()
        {
            if (mainPanel != null)
            {
                // mainPanel.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
            }
        }
        
        /// <summary>
        /// 按钮依次出现动画
        /// </summary>
        private void AnimateButtonsSequentially()
        {
            Button[] buttons = { startGameButton, continueGameButton, settingsButton, exitGameButton };
            
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null)
                {
                    buttons[i].transform.localScale = Vector3.zero;
                    // 延迟播放动画
                    // buttons[i].transform.DOScale(Vector3.one, 0.2f).SetDelay(i * 0.1f);
                }
            }
        }
        
        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        private bool CheckSaveDataExists()
        {
            // 这里应该检查实际的存档系统
            return PlayerPrefs.HasKey("SaveData");
        }
        
        /// <summary>
        /// 更新成就显示
        /// </summary>
        private void UpdateAchievementDisplay()
        {
            if (achievementIcons != null && achievementIcons.Length > 0)
            {
                // 获取已解锁的成就
                bool[] unlockedAchievements = GetUnlockedAchievements();
                
                for (int i = 0; i < achievementIcons.Length && i < unlockedAchievements.Length; i++)
                {
                    if (achievementIcons[i] != null)
                    {
                        achievementIcons[i].color = unlockedAchievements[i] ? Color.white : Color.gray;
                    }
                }
            }
        }
        
        /// <summary>
        /// 获取已解锁的成就
        /// </summary>
        private bool[] GetUnlockedAchievements()
        {
            // 这里应该从成就系统获取数据
            return new bool[] { true, false, true, false, false };
        }
        
        #region 按钮事件处理
        
        /// <summary>
        /// 开始游戏按钮点击
        /// </summary>
        private void OnStartGameClick()
        {
            Debug.Log("开始新游戏");
            
            // 播放按钮点击音效
            PlayButtonClickSound();
            
            // 打开游戏场景选择界面或直接开始游戏
            GameEntry.GetComponent<UIComponent>().OpenUIForm("GameplayUI");
            
            // 关闭当前界面
            Close();
        }
        
        /// <summary>
        /// 继续游戏按钮点击
        /// </summary>
        private void OnContinueGameClick()
        {
            Debug.Log("继续游戏");
            
            PlayButtonClickSound();
            
            // 加载存档并继续游戏
            LoadSaveDataAndContinue();
        }
        
        /// <summary>
        /// 设置按钮点击
        /// </summary>
        private void OnSettingsClick()
        {
            Debug.Log("打开设置");
            
            PlayButtonClickSound();
            
            // 打开设置界面
            GameEntry.GetComponent<UIComponent>().OpenUIForm("SettingsUI");
        }
        
        /// <summary>
        /// 退出游戏按钮点击
        /// </summary>
        private void OnExitGameClick()
        {
            Debug.Log("退出游戏");
            
            PlayButtonClickSound();
            
            // 显示确认对话框
            ShowExitConfirmDialog();
        }
        
        /// <summary>
        /// 音量滑块值改变
        /// </summary>
        private void OnVolumeChanged(float value)
        {
            // 保存音量设置
            PlayerPrefs.SetFloat("MasterVolume", value);
            
            // 应用音量设置
            AudioListener.volume = value;
            
            // 更新音量显示文本
            if (volumeValueText != null)
            {
                volumeValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
            }
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 播放按钮点击音效
        /// </summary>
        private void PlayButtonClickSound()
        {
            // 这里应该调用音频管理器播放音效
            // GameEntry.GetComponent<SoundComponent>().PlaySound("ButtonClick");
        }
        
        /// <summary>
        /// 加载存档并继续游戏
        /// </summary>
        private void LoadSaveDataAndContinue()
        {
            // 这里应该调用存档系统加载数据
            // var saveData = GameEntry.GetComponent<SaveComponent>().LoadGame();
            // if (saveData != null)
            // {
            //     GameEntry.GetComponent<UIComponent>().OpenUIForm("GameplayUI", saveData);
            //     Close();
            // }
        }
        
        /// <summary>
        /// 显示退出确认对话框
        /// </summary>
        private void ShowExitConfirmDialog()
        {
            // 这里应该显示确认对话框
            // GameEntry.GetComponent<UIComponent>().OpenUIForm("ConfirmDialog", new ConfirmDialogData
            // {
            //     Title = "退出游戏",
            //     Message = "确定要退出游戏吗？",
            //     OnConfirm = () => Application.Quit(),
            //     OnCancel = null
            // });
            
            // 临时直接退出
            Application.Quit();
        }
        
        #endregion
    }
}