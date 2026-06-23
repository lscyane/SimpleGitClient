using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleGitClient.Views
{
    /// <summary>
    /// LogWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LogWindow : Window
    {
        public LogWindow(string path)
        {
            this.DataContext = new LogWindowViewModel(path);
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender is ListViewItem listViewItem)
             && (listViewItem.DataContext is Models.CommitChanges changes)
            ) {
                var vm = (LogWindowViewModel)this.DataContext;
                Models.WinMerge.OpenDiff(vm.repo, changes);
            }
        }
    }
}
