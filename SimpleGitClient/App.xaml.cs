using System.Configuration;
using System.Data;
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
                Views.LogWindow log_win = new Views.LogWindow(e.Args[1]);
                log_win.Show();
            }
            else if ((e.Args.Length >= 2) && (e.Args[0] == "commit"))
            {
                Views.CommitWindow commit_win = new Views.CommitWindow(e.Args[1]);
                commit_win.Show();
            }
            else
            {
                this.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {

        }
    }

}
