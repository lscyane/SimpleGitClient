using CommunityToolkit.Mvvm.ComponentModel;
using LibGit2Sharp;
using lscyane.Core.Mvvm;
using SimpleGitClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGitClient.Views
{
    internal class LogWindowViewModel : ObservableObject
    {
        private readonly IDialogService _DialogService;
        public LibGit2Sharp.Repository repo { get; }

        public ObservableCollection<Models.CommitLog> CommitLogs { get; } = new ObservableCollection<Models.CommitLog>();

        public string CurrentBrancheName
        {
            get => repo.Head.FriendlyName;
        }


        #region Window位置関連プロパティ
        public double WindowLeft
        {
            get => Properties.EnvSettings.Default.LogWindowLeft;
            set => Properties.EnvSettings.Default.LogWindowLeft = value;
        }

        public double WindowTop
        {
            get => Properties.EnvSettings.Default.LogWindowTop;
            set => Properties.EnvSettings.Default.LogWindowTop = value;
        }

        public double WindowWidth
        {
            get => Properties.EnvSettings.Default.LogWindowWidth;
            set => Properties.EnvSettings.Default.LogWindowWidth = value;
        }

        public double WindowHeight
        {
            get => Properties.EnvSettings.Default.LogWindowHeight;
            set => Properties.EnvSettings.Default.LogWindowHeight = value;
        }
        #endregion


        public Models.CommitLog? SelectedCommitLog
        {
            get => this._selectedCommitLog;
            set
            {
                if (SetProperty(ref this._selectedCommitLog, value) && (_selectedCommitLog != null))
                {
                    // 一つ前と比較して、ファイル差分のコレクションを得る。
                    var parent = _selectedCommitLog.Parents.FirstOrDefault();
                    if (parent != null)
                    {
                        this.CommitChanges.Clear();
                        var patch = repo.Diff.Compare<Patch>(parent.Tree, _selectedCommitLog.Tree);
                        foreach (PatchEntryChanges change in patch)
                        {
                            this.CommitChanges.Add(new CommitChanges(change));
                        }
                    }
                    else
                    {
                        this.CommitChanges.Clear();
                    }
                }
            }
        }
        Models.CommitLog? _selectedCommitLog = null;

        public ObservableCollection<Models.CommitChanges> CommitChanges { get; private set; } = new ObservableCollection<Models.CommitChanges>();


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="path"></param>
        public LogWindowViewModel(IDialogService dialogService, string path)
        {
            this.repo = new LibGit2Sharp.Repository(path);
            this._DialogService = dialogService;

            // コミットを調べる
            foreach (LibGit2Sharp.Commit commit in repo.Commits)
            {
                this.CommitLogs.Add(new Models.CommitLog(commit, repo));
            }
        }


        public void ResetBranchHard(Models.CommitLog commitLog, string branchName)
        {
            var branch = repo.Branches[branchName];
            if (branch == null) return;

            var commit = repo.Lookup<LibGit2Sharp.Commit>(commitLog.Hash);
            if (commit == null) return;

            if (branch.IsCurrentRepositoryHead)
            {
                // 現在のブランチ → Hard reset（インデックス・作業ツリーも更新）
                try
                {
                    repo.Reset(ResetMode.Hard, commit);
                }
                catch (LibGit2Sharp.LibGit2SharpException ex)
                {
                    MessageBox.Show(
                        $"Hard Resetに失敗しました。\n別のプロセスがファイルを使用中の可能性があります。\n\n詳細: {ex.Message}",
                        "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                // 別のブランチ → ブランチ参照のみ更新
                repo.Refs.UpdateTarget(branch.Reference, commit.Sha);
            }

            RefreshLogs();
        }


        public void CheckoutCommit(Models.CommitLog commitLog)
        {
            var dialog = new DialogParameters();
            dialog.Add("Title", "ここへ切り替え/チェックアウト");
            dialog.Add("Branches", repo.Branches);
            dialog.Add("CommitHash", commitLog.Hash);

            this._DialogService.ShowDialog(typeof(Dialogs.CheckoutDialogViewModel), dialog, (result, param) =>
            {
                if (!result || param is not DialogParameters dp) return;

                var targetType       = dp.GetValue<string>("TargetType")    ?? string.Empty;
                var isCreateNewBranch = dp.GetValue<bool>("IsCreateNewBranch");
                var newBranchName    = dp.GetValue<string>("NewBranchName") ?? string.Empty;

                if (targetType == "Branch")
                {
                    var branchName = dp.GetValue<string>("SelectedBranch") ?? string.Empty;
                    var branch = repo.Branches[branchName];
                    if (branch == null) return;

                    if (isCreateNewBranch && !string.IsNullOrWhiteSpace(newBranchName))
                    {
                        var newBranch = repo.CreateBranch(newBranchName, branch.Tip);
                        LibGit2Sharp.Commands.Checkout(repo, newBranch);
                    }
                    else
                    {
                        LibGit2Sharp.Commands.Checkout(repo, branch);
                    }
                }
                else if (targetType == "Commit")
                {
                    var hash = dp.GetValue<string>("CommitHash") ?? string.Empty;
                    var commit = repo.Lookup<LibGit2Sharp.Commit>(hash);
                    if (commit == null) return;

                    if (isCreateNewBranch && !string.IsNullOrWhiteSpace(newBranchName))
                    {
                        var newBranch = repo.CreateBranch(newBranchName, commit);
                        LibGit2Sharp.Commands.Checkout(repo, newBranch);
                    }
                    else
                    {
                        // デタッチド HEAD 状態でチェックアウト
                        LibGit2Sharp.Commands.Checkout(repo, commit.Sha);
                    }
                }

                RefreshLogs();
            });
        }


        private void RefreshLogs()
        {
            CommitLogs.Clear();
            foreach (LibGit2Sharp.Commit commit in repo.Commits)
            {
                CommitLogs.Add(new Models.CommitLog(commit, repo));
            }
        }
    }
}
