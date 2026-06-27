using CommunityToolkit.Mvvm.ComponentModel;
using LibGit2Sharp;
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


        /// <summary>
        /// コミットウインドウ・ログウインドウ共通 コンストラクタ
        /// </summary>
        /// <param name="treeEntryChanges"></param>
        public CommitChanges(LibGit2Sharp.PatchEntryChanges treeEntryChanges)
        {
            this.Path = treeEntryChanges.Path;
            this.Extension = System.IO.Path.GetExtension(treeEntryChanges.Path);
            this.Status = treeEntryChanges.Status;
            this.AddLines = treeEntryChanges.LinesAdded;
            this.DelLines = treeEntryChanges.LinesDeleted;
        }


        /// <summary>
        /// コミットウインドウからの生成
        /// </summary>
        /// <param name="statusEntry"></param>
        public CommitChanges(LibGit2Sharp.StatusEntry statusEntry)
        {
            this.Path = statusEntry.FilePath;
            this.Extension = System.IO.Path.GetExtension(statusEntry.FilePath);
            this.Status = statusEntry.State;
            this.IsChecked = true;
        }
    }
}
