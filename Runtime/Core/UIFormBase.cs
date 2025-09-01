using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.UI;
using UnityGameFramework.Runtime;

namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI窗体基类，扩展GameFramework的UIFormLogic
    /// </summary>
    public abstract class UIFormBase : UIFormLogic
    {
        [SerializeField] private UIFormConfig m_Config;
        [SerializeField] private bool m_AutoBindComponents = true;
        
        private Dictionary<string, Component> m_BoundComponents;
        private bool m_IsInitialized;
        
        /// <summary>
        /// UI窗体配置
        /// </summary>
        public UIFormConfig Config => m_Config;
        
        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => m_IsInitialized;
        
        /// <summary>
        /// 绑定的组件字典
        /// </summary>
        protected Dictionary<string, Component> BoundComponents => m_BoundComponents;
        
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            m_BoundComponents = new Dictionary<string, Component>();
            
            if (m_AutoBindComponents)
            {
                BindComponents();
            }
            
            OnUIFormInit(userData);
            m_IsInitialized = true;
        }
        
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            OnUIFormOpen(userData);
        }
        
        protected override void OnClose(bool isShutdown, object userData)
        {
            // 解绑事件
            UnbindEvents();
            
            OnUIFormClose(isShutdown, userData);
            
            // 清除组件缓存
            m_BoundComponents?.Clear();
            
            base.OnClose(isShutdown, userData);
        }
        
        protected override void OnPause()
        {
            base.OnPause();
            OnUIFormPause();
        }
        
        protected override void OnResume()
        {
            base.OnResume();
            OnUIFormResume();
        }
        
        protected override void OnCover()
        {
            base.OnCover();
            OnUIFormCover();
        }
        
        protected override void OnReveal()
        {
            base.OnReveal();
            OnUIFormReveal();
        }
        
        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            OnUIFormRefocus(userData);
        }
        
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            OnUIFormUpdate(elapseSeconds, realElapseSeconds);
        }
        
        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            OnUIFormDepthChanged(uiGroupDepth, depthInUIGroup);
        }
        
        /// <summary>
        /// 绑定组件（由生成的分部类重写）
        /// </summary>
        protected virtual void BindComponents()
        {
            // 绑定事件
            BindEvents();
        }
        
        /// <summary>
        /// 获取绑定的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="variableName">变量名</param>
        /// <returns>组件实例</returns>
        public T GetComponent<T>(string variableName) where T : Component
        {
            if (m_BoundComponents.TryGetValue(variableName, out var component))
            {
                return component as T;
            }
            
            return null;
        }
        
        /// <summary>
        /// 设置UI配置
        /// </summary>
        /// <param name="config">UI配置</param>
        public void SetConfig(UIFormConfig config)
        {
            m_Config = config;
        }
        
        #region 虚方法 - 子类重写
        
        /// <summary>
        /// 绑定UI事件
        /// </summary>
        protected virtual void BindEvents() { }
        
        /// <summary>
        /// 解绑UI事件
        /// </summary>
        protected virtual void UnbindEvents() { }
        
        /// <summary>
        /// UI窗体初始化
        /// </summary>
        /// <param name="userData">用户数据</param>
        protected virtual void OnUIFormInit(object userData) { }
        
        /// <summary>
        /// UI窗体打开
        /// </summary>
        /// <param name="userData">用户数据</param>
        protected virtual void OnUIFormOpen(object userData) { }
        
        /// <summary>
        /// UI窗体关闭
        /// </summary>
        /// <param name="isShutdown">是否关闭</param>
        /// <param name="userData">用户数据</param>
        protected virtual void OnUIFormClose(bool isShutdown, object userData) { }
        
        /// <summary>
        /// UI窗体暂停
        /// </summary>
        protected virtual void OnUIFormPause() { }
        
        /// <summary>
        /// UI窗体恢复
        /// </summary>
        protected virtual void OnUIFormResume() { }
        
        /// <summary>
        /// UI窗体遮挡
        /// </summary>
        protected virtual void OnUIFormCover() { }
        
        /// <summary>
        /// UI窗体显露
        /// </summary>
        protected virtual void OnUIFormReveal() { }
        
        /// <summary>
        /// UI窗体重新聚焦
        /// </summary>
        /// <param name="userData">用户数据</param>
        protected virtual void OnUIFormRefocus(object userData) { }
        
        /// <summary>
        /// UI窗体更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间</param>
        /// <param name="realElapseSeconds">真实流逝时间</param>
        protected virtual void OnUIFormUpdate(float elapseSeconds, float realElapseSeconds) { }
        
        /// <summary>
        /// UI窗体深度改变
        /// </summary>
        /// <param name="uiGroupDepth">UI组深度</param>
        /// <param name="depthInUIGroup">在UI组中的深度</param>
        protected virtual void OnUIFormDepthChanged(int uiGroupDepth, int depthInUIGroup) { }
        
        #endregion
    }
}