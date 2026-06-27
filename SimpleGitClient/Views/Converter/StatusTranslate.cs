using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SimpleGitClient.Converter;

[System.Reflection.Obfuscation]
public class StatusTranslate : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is LibGit2Sharp.FileStatus status)
        {
            if (status.HasFlag(LibGit2Sharp.FileStatus.NewInWorkdir)) return "未追跡";
            if (status.HasFlag(LibGit2Sharp.FileStatus.ModifiedInWorkdir)) return "変更";
            if (status.HasFlag(LibGit2Sharp.FileStatus.DeletedFromWorkdir)) return "削除";
            if (status.HasFlag(LibGit2Sharp.FileStatus.RenamedInWorkdir)) return "名前変更";
            if (status.HasFlag(LibGit2Sharp.FileStatus.NewInIndex)) return "追加 (ステージ済)";
            if (status.HasFlag(LibGit2Sharp.FileStatus.ModifiedInIndex)) return "変更 (ステージ済)";
            if (status.HasFlag(LibGit2Sharp.FileStatus.DeletedFromIndex)) return "削除 (ステージ済)";
            if (status.HasFlag(LibGit2Sharp.FileStatus.RenamedInIndex)) return "名前変更 (ステージ済)";
            return status.ToString();
        }

        if (value is LibGit2Sharp.ChangeKind kind)
        {
            if (kind == LibGit2Sharp.ChangeKind.Unmodified) return "変更なし";
            if (kind == LibGit2Sharp.ChangeKind.Added) return "追加";
            if (kind == LibGit2Sharp.ChangeKind.Deleted) return "削除";
            if (kind == LibGit2Sharp.ChangeKind.Modified) return "変更";
            if (kind == LibGit2Sharp.ChangeKind.Renamed) return "名前変更";
            if (kind == LibGit2Sharp.ChangeKind.Copied) return "コピー";
            if (kind == LibGit2Sharp.ChangeKind.Ignored) return "無視";
            if (kind == LibGit2Sharp.ChangeKind.Untracked) return "未追跡";
            if (kind == LibGit2Sharp.ChangeKind.TypeChanged) return "タイプ変更";
            if (kind == LibGit2Sharp.ChangeKind.Unreadable) return "読み取り不可";
            if (kind == LibGit2Sharp.ChangeKind.Conflicted) return "競合";
            return kind.ToString();
        }
        return value;
    }


    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new System.NotSupportedException();
    }


    // 値コンバーターの実体
    public static StatusTranslate Converter = new StatusTranslate();
}
