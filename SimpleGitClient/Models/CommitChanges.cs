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
        public string Status { get; }
        public int AddLines { get; }
        public int DelLines { get; }


        public CommitChanges(LibGit2Sharp.TreeEntryChanges treeEntryChanges)
        {
            this.Path = treeEntryChanges.Path;
            this.Extension = System.IO.Path.GetExtension(treeEntryChanges.Path);
            this.Status = treeEntryChanges.Status.ToString();
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

    }
}
