using CommunityToolkit.Mvvm.ComponentModel;
using LibGit2Sharp;
using SimpleGitClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGitClient.Views
{
    internal class LogWindowViewModel : ObservableObject
    {
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


        public LogWindowViewModel(string path)
        {
            this.repo = new LibGit2Sharp.Repository(path);

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
                repo.Reset(ResetMode.Hard, commit);
            }
            else
            {
                // 別のブランチ → ブランチ参照のみ更新
                repo.Refs.UpdateTarget(branch.Reference, commit.Sha);
            }

            RefreshLogs();
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
