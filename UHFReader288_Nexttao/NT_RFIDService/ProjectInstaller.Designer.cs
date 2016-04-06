namespace NT_RFIDService
{
    partial class ProjectInstaller
    {
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
            this.NT_RFIDServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.NT_RFIDServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // NT_RFIDServiceProcessInstaller
            // 
            this.NT_RFIDServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.NT_RFIDServiceProcessInstaller.Password = null;
            this.NT_RFIDServiceProcessInstaller.Username = null;
            this.NT_RFIDServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller1_AfterInstall);
            // 
            // NT_RFIDServiceInstaller
            // 
            this.NT_RFIDServiceInstaller.Description = "用于户获取用户试衣数据";
            this.NT_RFIDServiceInstaller.DisplayName = "NextTao读写器_0";
            this.NT_RFIDServiceInstaller.ServiceName = "NTRFIDService_0";
            this.NT_RFIDServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.NT_RFIDServiceProcessInstaller,
            this.NT_RFIDServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller NT_RFIDServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller NT_RFIDServiceInstaller;
    }
}