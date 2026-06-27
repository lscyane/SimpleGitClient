using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibGit2Sharp;
using lscyane.Core.Mvvm;
using System.Collections.Generic;

namespace SimpleGitClient.Dialogs;


public class CheckoutDialogViewModel : DialogViewModelBase
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CheckoutDialogViewModel()
    {
    }


    /// <inheritdoc/>
    public override void OnDialogPreviewOpen(DialogParameters? parameters)
    {
        this.Title = parameters?.GetValue<string>("Title") ?? throw new ArgumentException();

        // コミットハッシュを取得
        this.CommitHash = parameters?.GetValue<string>("CommitHash") ?? string.Empty;

        // 新しいブランチ名の初期値
        this.NewBranchName = $"Branch_{this.CommitHash.Substring(0, 8)}";

        // ブランチの一覧を取得
        var branches = parameters?.GetValue<BranchCollection>("Branches");
        if (branches != null)
        {
            var branchNames = branches
                .Where(b => !b.IsRemote)
                .Select(b => b.FriendlyName)
                .ToList();
            this.Branches = new List<string>(branchNames);

            // コミットハッシュに一致するブランチがあれば選択
            foreach (var branch in branches)
            {
                if ((branch.IsCurrentRepositoryHead == false)
                 && (branch.Reference.TargetIdentifier == this.CommitHash)
                ) {
                    this.SelectedBranch = branch.FriendlyName;
                    this.IsTargetBranch = true;
                    this.IsCreateNewBranch = false;
                    break;
                }
            }
        }
    }


    // ──────────────── ターゲット種別 ────────────────

    /// <summary> ブランチへ切り替え（ラジオボタン用） </summary>
    public bool IsTargetBranch
    {
        get => _isTargetBranch;
        set
        {
            if (SetProperty(ref _isTargetBranch, value))
                OnPropertyChanged(nameof(IsTargetCommit));
        }
    }
    private bool _isTargetBranch = false;

    /// <summary> コミットへ切り替え（ラジオボタン用） </summary>
    public bool IsTargetCommit
    {
        get => !_isTargetBranch;
        set => IsTargetBranch = !value;
    }


    // ──────────────── ブランチ ────────────────

    /// <summary> ブランチ一覧（ComboBox 用） </summary>
    public List<string> Branches
    {
        get => _branches;
        set => SetProperty(ref _branches, value);
    }
    private List<string> _branches = new();

    /// <summary> 選択中のブランチ名 </summary>
    public string SelectedBranch
    {
        get => _selectedBranch;
        set => SetProperty(ref _selectedBranch, value);
    }
    private string _selectedBranch = string.Empty;


    // ──────────────── コミットハッシュ ────────────────

    /// <summary> チェックアウト対象のコミット SHA </summary>
    public string CommitHash
    {
        get => _commitHash;
        set => SetProperty(ref _commitHash, value);
    }
    private string _commitHash = string.Empty;


    // ──────────────── 新しいブランチを作成 ────────────────

    /// <summary> 新しいブランチを作成するか </summary>
    public bool IsCreateNewBranch
    {
        get => _isCreateNewBranch;
        set => SetProperty(ref _isCreateNewBranch, value);
    }
    private bool _isCreateNewBranch = true;

    /// <summary> 新しいブランチ名 </summary>
    public string NewBranchName
    {
        get => _newBranchName;
        set => SetProperty(ref _newBranchName, value);
    }
    private string _newBranchName = string.Empty;


    // ──────────────── コマンド ────────────────

    /// <summary>
    /// キャンセルボタン
    /// </summary>
    public RelayCommand<object> CancelCommand => _CancelCommand ??= new RelayCommand<object>(e => this.OnRequestClose());
    RelayCommand<object>? _CancelCommand;


    /// <summary>
    /// OKボタン
    /// </summary>
    public RelayCommand<object> OkCommand => _OkCommand ??= new RelayCommand<object>(e =>
    {
        var retval = new DialogParameters();
        retval.Add("TargetType", IsTargetBranch ? "Branch" : "Commit");
        retval.Add("SelectedBranch", SelectedBranch);
        retval.Add("CommitHash", CommitHash);
        retval.Add("IsCreateNewBranch", IsCreateNewBranch);
        retval.Add("NewBranchName", NewBranchName);
        this.OnRequestClose(true, retval);
    });
    RelayCommand<object>? _OkCommand;
}

