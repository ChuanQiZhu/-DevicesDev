using log4net;
using System.Configuration;

namespace NT_RFIDService
{
    partial class NT_RFIDService
    {
        private ILog log = log4net.LogManager.GetLogger(typeof(Reader));
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            //
            // 摘要:
            //     刷新命名节，这样在下次检索它时将从磁盘重新读取它。
            //
            // 参数:
            //   sectionName:
            //     要刷新的节的配置节名称或配置路径和节名称。
            ConfigurationManager.RefreshSection("appSettings");
            // 获得系统参数
            string URL = ConfigurationManager.AppSettings["UPLOAD_URL"];
            string LogURL = ConfigurationManager.AppSettings["UPLOAD_LogURL"];
            string IP = ConfigurationManager.AppSettings["READER_IP"];
            int Port = int.Parse(ConfigurationManager.AppSettings["READER_PORT"]);
            byte fComAdr = byte.Parse(ConfigurationManager.AppSettings["READER_ENUM"]);
            int OUT_DURATION = int.Parse(ConfigurationManager.AppSettings["LABLE_OUT_DURATION"]);
            int IN_DURATION = int.Parse(ConfigurationManager.AppSettings["LABLE_IN_DURATION"]);
            int SleepTime = int.Parse(ConfigurationManager.AppSettings["SLEEP_TIME"]);
            log.Info("Function [InitializeComponent] Update ConfigInfo: {URL:" + URL + ", LogURL:" + LogURL + ", IP:" + IP + ", Port:" + Port + ", fComAdr: " + fComAdr + ", OUT_DURATION: " + OUT_DURATION + ", IN_DURATION: " + IN_DURATION + ", SleepTime: " + SleepTime);

            // 实例化对象
            reader = new Reader(URL, LogURL, IP, Port, fComAdr, OUT_DURATION, IN_DURATION, SleepTime);
            components = new System.ComponentModel.Container();
            this.ServiceName = "NTRFIDService_0";
        }

        #endregion
    }
}
