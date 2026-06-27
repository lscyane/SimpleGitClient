using SimpleGitClient.Views;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace SimpleGitClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if ((e.Args.Length >= 2) && (e.Args[0] == "log"))
            {
                string path = e.Args[1];
                Views.LogWindow log_win = new Views.LogWindow(path);
                log_win.Show();
            }
            else if ((e.Args.Length >= 2) && (e.Args[0] == "commit"))
            {
                string path = e.Args[1];
                Views.CommitWindow commit_win = new Views.CommitWindow(path);
                commit_win.Show();
            }
            else
            {
                Views.SettingsWindow settings_win = new Views.SettingsWindow();
                settings_win.Show();
                //this.Shutdown();
            }
        }


        /// <summary>
        /// アプリケーション終了時（<see cref="Application.Exit"/>）に実行されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SimpleGitClient.Properties.EnvSettings.Default.Save();
        }
    }

}
