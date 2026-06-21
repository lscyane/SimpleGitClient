using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGitClient.Views
{
    class CommitWindowViewModel : ObservableObject
    {
        LibGit2Sharp.Repository repo;

        public ObservableCollection<Models.CommitChanges> CommitChanges { get; private set; } = new ObservableCollection<Models.CommitChanges>();


        public CommitWindowViewModel(string path)
        {
            this.repo = new LibGit2Sharp.Repository(path);
        }


        public RelayCommand CommitCommand => this._CommitCommand ??= this._CommitCommand ?? new RelayCommand(() =>
        {

        });
        private RelayCommand? _CommitCommand = null;


   
        public RelayCommand CancelCommand => this._CancelCommand ??= this._CancelCommand ?? new RelayCommand(() =>
        {

        });
        private RelayCommand? _CancelCommand = null;
    }
}
