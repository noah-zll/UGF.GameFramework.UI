using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using System.Collections;

namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI窗体基类，扩展GameFramework的UIFormLogic
    /// </summary>
    public abstract class UIFormBase : UIFormLogic
    {
        [SerializeField] private UIFormConfig m_Config;
        [SerializeField] private bool m_AutoBindComponents = true;

        private bool m_IsInitialized;

        /// <summary>
        /// UI窗体配置
        /// </summary>
        public UIFormConfig Config => m_Config;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => m_IsInitialized;

        protected virtual float FadeTime => Config != null ? Config.FadeInDuration : 0.3f;
        protected virtual float FadeOutTime => Config != null ? Config.FadeOutDuration : 0.3f;

        protected override void OnInit(object userData)
        {
            if (userData is UIFormBaseData uiFormBaseData)
            {
                m_Config = uiFormBaseData.Config;
                base.OnInit(uiFormBaseData.UserData);
            }
            else
            {
                base.OnInit(userData);
            }


            if (m_AutoBindComponents)
            {
                BindComponents();
            }

            OnUIFormInit(userData);
            m_IsInitialized = true;
        }

        protected override void OnOpen(object userData)
        {
            if (userData is UIFormBaseData uiFormBaseData)
            {
                base.OnOpen(uiFormBaseData.UserData);
            }
            else
            {
                base.OnOpen(userData);
            }

            // 自动播放进场动画
            StopAllCoroutines();
            StartCoroutine(PlayFadeIn(FadeTime));
            OnUIFormOpen(userData);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            // 解绑事件
            UnbindEvents();

            OnUIFormClose(isShutdown, userData);

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
        /// 简单的原生淡入实现
        /// </summary>
        private IEnumerator PlayFadeIn(float duration)
        {
            if (!TryGetComponent<CanvasGroup>(out var group)) group = gameObject.AddComponent<CanvasGroup>();

            group.alpha = 0;
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                group.alpha = timer / duration;
                yield return null;
            }
            group.alpha = 1;
        }

        /// <summary>
        /// 简单的原生淡出实现
        /// </summary>
        private IEnumerator PlayFadeOut(float duration)
        {
            if (!TryGetComponent<CanvasGroup>(out var group)) group = gameObject.AddComponent<CanvasGroup>();

            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                group.alpha = 1 - timer / duration;
                yield return null;
            }
            group.alpha = 0;
            // 关闭UI窗体
            GameEntry.GetComponent<UIComponent>().CloseUIForm(UIForm);
        }

        /// <summary>
        /// 绑定组件（由生成的分部类重写）
        /// </summary>
        protected virtual void BindComponents()
        {
            // 绑定事件
            BindEvents();
        }

        public void CloseWithFadeOut()
        {
            StopAllCoroutines();
            StartCoroutine(PlayFadeOut(FadeOutTime));
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