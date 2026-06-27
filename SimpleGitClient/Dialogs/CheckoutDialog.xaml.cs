using System.Windows;
using System.Windows.Input;

namespace SimpleGitClient.Dialogs;

/// <summary>
/// CheckoutDialog.xaml の相互作用ロジック
/// </summary>
public partial class CheckoutDialog : Window
{
    public CheckoutDialog()
    {
        this.DataContext = new CheckoutDialogViewModel();
        InitializeComponent();
    }
}

