using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGitClient.Models
{
    public class WinMerge
    {
        /// <summary>
        /// WinMergeを起動して差分を表示する
        /// </summary>
        internal static void OpenDiff(LibGit2Sharp.Repository repo, Models.CommitChanges item)
        {
            var winMergePaths = new[]
            {
                @"C:\Program Files\WinMerge\WinMergeU.exe",
                @"C:\Program Files (x86)\WinMerge\WinMergeU.exe",
            };
            var winMergePath = winMergePaths.FirstOrDefault(File.Exists);
            if (winMergePath == null)
            {
                MessageBox.Show("WinMergeが見つかりません。\nWinMergeをインストールしてください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var workDir = repo.Info.WorkingDirectory;
            var fullPath = Path.Combine(workDir, item.Path.Replace('/', Path.DirectorySeparatorChar));
            var tempPath = Path.Combine(Path.GetTempPath(), "sgc_" + Path.GetFileName(item.Path));

            try
            {
                if (!repo.Info.IsHeadUnborn)
                {
                    var headCommit = repo.Head.Tip;
                    var entry = headCommit[item.Path];
                    if (entry?.Target is LibGit2Sharp.Blob blob)
                    {
                        using var stream = blob.GetContentStream();
                        using var fileStream = File.Create(tempPath);
                        stream.CopyTo(fileStream);
                    }
                    else
                    {
                        File.WriteAllText(tempPath, string.Empty);
                    }
                }
                else
                {
                    File.WriteAllText(tempPath, string.Empty);
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = winMergePath,
                    Arguments = $"/u /e /wl \"{tempPath}\" \"{fullPath}\"",
                    UseShellExecute = false,
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"差分表示に失敗しました。\n{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
