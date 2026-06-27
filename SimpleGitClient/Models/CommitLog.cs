using CommunityToolkit.Mvvm.ComponentModel;
using LibGit2Sharp;
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

        public BranchInfo[] Branches { get; set; }


        public CommitLog(LibGit2Sharp.Commit commit, LibGit2Sharp.Repository repo)
        {
            this.Hash = commit.Sha;
            this.Auther = commit.Author.Name;
            this.Email = commit.Author.Email;
            this.CommitTime = commit.Author.When.DateTime;
            this.Message = commit.Message;
            this.MessageShort = commit.MessageShort;

            this.Tree = commit.Tree;
            this.Parents = commit.Parents;
            this.Branches = GetBranchesForCommit(repo, commit).ToArray();
        }


        public static List<BranchInfo> GetBranchesForCommit(Repository repo, Commit commit)
        {
            // ローカルブランチとリモートブランチをすべてループ
            var branchInfos = new List<BranchInfo>();
            foreach (var branch in repo.Branches)
            {
                // ブランチの先端（Tip）のコミットを取得（シンボリックリンクなどの場合はNullの可能性を考慮）
                if (branch.Tip?.Sha == commit.Sha)
                {
                    branchInfos.Add(new BranchInfo(branch));
                }
            }
            return branchInfos;
        }


        public class BranchInfo
        {
            public string Name { get; set; } = string.Empty;
            public BranchType Type { get; set; }

            public BranchInfo(LibGit2Sharp.Branch branch)
            {
                this.Name = branch.FriendlyName;    // "main" や "origin/main"
                if (branch.IsRemote)
                {
                    this.Type = BranchType.Remote;
                }
                else if (branch.IsCurrentRepositoryHead)
                {
                    this.Type = BranchType.Current;
                }
                else
                {
                    this.Type = BranchType.Local;
                }
            }

            public enum BranchType
            {
                Local,
                Current,
                Remote
            }
        }
    }
}
