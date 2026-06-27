using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SimpleGitClient.Models
{
    internal class CommitChanges : ObservableObject
    {
        public string Path { get; }
        public string Extension { get; }
        public Enum Status { get; }
        public int AddLines { get; }
        public int DelLines { get; }

        /// <summary>
        /// [コミットウインドウ用] コミットするファイルを選択するためのプロパティ。
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
        private bool _isChecked = true;


        public CommitChanges(LibGit2Sharp.TreeEntryChanges treeEntryChanges)
        {
            this.Path = treeEntryChanges.Path;
            this.Extension = System.IO.Path.GetExtension(treeEntryChanges.Path);
            this.Status = treeEntryChanges.Status;
        }


        public CommitChanges(LibGit2Sharp.StatusEntry statusEntry)
        {
            this.Path = statusEntry.FilePath;
            this.Extension = System.IO.Path.GetExtension(statusEntry.FilePath);
            this.Status = statusEntry.State;
            this.IsChecked = true;
        }


        public static CommitChanges[] FromTreeChanges(LibGit2Sharp.TreeChanges changes)
        {
            var retval = new List<CommitChanges>();
            foreach (var change in changes.Added)
            {
                retval.Add(new CommitChanges(change));
            }
            foreach (var change in changes.Deleted)
            {
                retval.Add(new CommitChanges(change));
            }
            foreach (var change in changes.Modified)
            {
                retval.Add(new CommitChanges(change));
            }
            foreach (var change in changes.Renamed)
            {
                retval.Add(new CommitChanges(change));
            }
            foreach (var change in changes.TypeChanged)
            {
                retval.Add(new CommitChanges(change));
            }
            return retval.ToArray();
        }


        public static CommitChanges[] FromRepositoryStatus(LibGit2Sharp.RepositoryStatus status)
        {
            var retval = new List<CommitChanges>();
            foreach (var entry in status)
            {
                if ((entry.State == LibGit2Sharp.FileStatus.Ignored)
                 || (entry.State == LibGit2Sharp.FileStatus.Unaltered)
                ) {
                    continue;
                }
                retval.Add(new CommitChanges(entry));
            }
            return retval.ToArray();
        }

    }
}
