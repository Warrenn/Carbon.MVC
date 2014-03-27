using System.ComponentModel;
using System.Configuration.Install;

namespace CarbonKnown.FileWatcherService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void FileWatcherInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
