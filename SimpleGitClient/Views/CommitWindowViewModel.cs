using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGitClient.Views
{
    class CommitWindowViewModel : ObservableObject
    {
        LibGit2Sharp.Repository repo;
        public event EventHandler? RequestClose;

        public ObservableCollection<Models.CommitChanges> CommitChanges { get; private set; } = new ObservableCollection<Models.CommitChanges>();

        /// <summary>
        /// コミットメッセージテキストボックスの内容
        /// </summary>
        public string CommitMessage
        {
            get => _commitMessage;
            set => SetProperty(ref _commitMessage, value);
        }
        private string _commitMessage = string.Empty;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path"></param>
        public CommitWindowViewModel(string path)
        {
            this.repo = new LibGit2Sharp.Repository(path);
            LoadChanges();
        }


        /// <summary>
        /// 変更されたファイルのリストを取得して、CommitChangesコレクションに追加する。
        /// </summary>
        private void LoadChanges()
        {
            CommitChanges.Clear();
            var status = repo.RetrieveStatus(new StatusOptions
            {
                IncludeUntracked = true,
                RecurseUntrackedDirs = true,
            });
            foreach (var change in Models.CommitChanges.FromRepositoryStatus(status))
            {
                CommitChanges.Add(change);
            }
        }


        /// <summary>
        /// コミットボタンがクリックされたときの処理
        /// </summary>
        public RelayCommand CommitCommand => this._CommitCommand ??= new RelayCommand(() =>
        {
            if (string.IsNullOrWhiteSpace(CommitMessage))
            {
                MessageBox.Show("コミットメッセージを入力してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var checkedItems = CommitChanges.Where(c => c.IsChecked).ToList();
            if (!checkedItems.Any())
            {
                MessageBox.Show("コミット対象のファイルを選択してください。", "入力エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // ステージエリアをリセット（初回コミット以外）
                if (!repo.Info.IsHeadUnborn)
                {
                    repo.Reset(ResetMode.Mixed);
                }

                // 選択したファイルをステージ
                foreach (var item in checkedItems)
                {
                    var fullPath = Path.Combine(repo.Info.WorkingDirectory, item.Path);
                    if (File.Exists(fullPath))
                        repo.Index.Add(item.Path);
                    else
                        repo.Index.Remove(item.Path);
                }
                repo.Index.Write();

                var sig = repo.Config.BuildSignature(DateTimeOffset.Now);
                repo.Commit(CommitMessage, sig, sig);

                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"コミットに失敗しました。\n{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        private RelayCommand? _CommitCommand = null;


        /// <summary>
        /// すべて選択ボタンがクリックされたときの処理
        /// </summary>
        public RelayCommand SelectAllCommand => this._SelectAllCommand ??= new RelayCommand(() =>
        {
            foreach (var item in CommitChanges)
            {
                item.IsChecked = true;
            }
        });
        private RelayCommand? _SelectAllCommand = null;


        /// <summary>
        /// すべて選択解除ボタンがクリックされたときの処理
        /// </summary>
        public RelayCommand SelectNoneCommand => this._SelectNoneCommand ??= new RelayCommand(() =>
        {
            foreach (var item in CommitChanges)
            {
                item.IsChecked = false;
            }
        });
        private RelayCommand? _SelectNoneCommand = null;


        /// <summary>
        /// キャンセルボタンがクリックされたときの処理
        /// </summary>
        public RelayCommand CancelCommand => this._CancelCommand ??= new RelayCommand(() =>
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        });
        private RelayCommand? _CancelCommand = null;
    }
}

