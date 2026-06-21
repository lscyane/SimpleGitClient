using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGitClient.Models
{
    internal class CommitLog : ObservableObject
    {
        /// <summary> コミットハッシュ </summary>
        public string Hash { get; set; } = string.Empty;

        /// <summary> アクション </summary>
        

        /// <summary> 作者 </summary>
        public string Auther { get; set; } = string.Empty;

        /// <summary> E-mail </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary> コミット日時 </summary>
        public DateTime CommitTime { get; set; } = DateTime.MinValue;

        /// <summary> コミットメッセージ </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary> コミットメッセージ省略形 </summary>
        public string MessageShort { get; set; } = string.Empty;

        public IEnumerable<LibGit2Sharp.Commit> Parents { get;}
        public LibGit2Sharp.Tree Tree { get; }

        public CommitLog(LibGit2Sharp.Commit commit)
        {
            this.Hash = commit.Sha;
            this.Auther = commit.Author.Name;
            this.Email = commit.Author.Email;
            this.CommitTime = commit.Author.When.DateTime;
            this.Message = commit.Message;
            this.MessageShort = commit.MessageShort;

            this.Tree = commit.Tree;
            this.Parents = commit.Parents;
        }
    }
}
