namespace CarbonKnown.FileWatcherService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.FileWatcherProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.FileWatcherInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // FileWatcherProcessInstaller
            // 
            this.FileWatcherProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.FileWatcherProcessInstaller.Password = null;
            this.FileWatcherProcessInstaller.Username = null;
            // 
            // FileWatcherInstaller
            // 
            this.FileWatcherInstaller.Description = "FileWatcher Service Monitors Folders specified in the config file to upload data " +
    "into the CarbonKnown system.";
            this.FileWatcherInstaller.DisplayName = "CarbonKnown FileWatcher Service";
            this.FileWatcherInstaller.ServiceName = "CarbonKnown FileWatcher";
            this.FileWatcherInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.FileWatcherInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.FileWatcherInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.FileWatcherProcessInstaller,
            this.FileWatcherInstaller});

        }

        #endregion

        public System.ServiceProcess.ServiceProcessInstaller FileWatcherProcessInstaller;
        public System.ServiceProcess.ServiceInstaller FileWatcherInstaller;
    }
}