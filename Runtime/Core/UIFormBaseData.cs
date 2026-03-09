namespace UGF.GameFramework.UI
{
    /// <summary>
    /// UI窗体数据
    /// </summary>
    public class UIFormBaseData
    {
        /// <summary>
        /// UI窗体配置
        /// </summary>
        public UIFormConfig Config { get; set; }
        public object UserData { get; set; }

        public UIFormBaseData(UIFormConfig config, object userData)
        {
            Config = config;
            UserData = userData;
        }
    }
}