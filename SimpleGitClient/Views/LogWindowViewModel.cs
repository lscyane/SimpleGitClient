using CommunityToolkit.Mvvm.ComponentModel;
using LibGit2Sharp;
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
                        var changes = repo.Diff.Compare<LibGit2Sharp.TreeChanges>(parent.Tree, _selectedCommitLog.Tree);
                        foreach (var change in Models.CommitChanges.FromTreeChanges(changes))
                        {
                            this.CommitChanges.Add(change);
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
                this.CommitLogs.Add(new Models.CommitLog(commit));
            }
        }
    }
}
