using LibGit2Sharp;
using lscyane.Wpf.Services;
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
            // ダイアログサービスの設定
            lscyane.Wpf.Services.DialogService.Main = new lscyane.Wpf.Services.DialogService(this);
            lscyane.Wpf.Services.DialogService.Main.RegisterDialog<Dialogs.CheckoutDialog, Dialogs.CheckoutDialogViewModel>();

            this.DataContext = new LogWindowViewModel(DialogService.Main, path);
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender is ListViewItem listViewItem)
             && (listViewItem.DataContext is Models.CommitChanges changes)
            ) {
                var vm = (LogWindowViewModel)this.DataContext;
                if (vm.SelectedCommitLog != null)
                {
                    Models.WinMerge.OpenDiff(vm.repo, vm.SelectedCommitLog, changes);
                }
            }
        }

        private void CommitLogListView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // ログが無い時はコンテキストメニューを表示しない
            var listViewItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
            if (listViewItem == null)
            {
                e.Handled = true;
                return;
            }

            // 選択されているアイテムが無い場合時はコンテキストメニューを表示しない
            var commitLog = listViewItem.DataContext as Models.CommitLog;
            if (commitLog == null)
            {
                e.Handled = true;
                return;
            }

            var vm = (LogWindowViewModel)DataContext;
            var menu = new ContextMenu();

            // コンテキストメニューに「ここにリセット」を追加する
            var menuItem = new MenuItem
            {
                Header = $"\"{vm.CurrentBrancheName}\"をここにリセット"
            };
            menuItem.Click += (s, args) => vm.ResetBranchHard(commitLog, vm.CurrentBrancheName);
            menu.Items.Add(menuItem);

            // コンテキストメニューに「ここへ切り替え/チェックアウト」を追加する
            menuItem = new MenuItem
            {
                Header = $"ここへ切り替え/チェックアウト"
            };
            menuItem.Click += (s, args) => vm.CheckoutCommit(commitLog);
            menu.Items.Add(menuItem);

            ((ListView)sender).ContextMenu = menu;
        }

        private static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T t) return t;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
    }
}
