using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SimpleGitClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleGitClient.Views
{
    class SettingsWindowViewModel : ObservableObject
    {
        private const string RegDirPath = @"Software\Classes\Directory\shell\SimpleGitClient";
        private const string RegBgPath  = @"Software\Classes\Directory\Background\shell\SimpleGitClient";

        public RelayCommand InstallRegCmd => this._InstallRegCmd ??= new RelayCommand(() =>
        {
            try
            {
                string exePath = Process.GetCurrentProcess().MainModule!.FileName;

                RegisterContextMenu(Registry.CurrentUser, RegDirPath, exePath, "%1");
                RegisterContextMenu(Registry.CurrentUser, RegBgPath,  exePath, "%V");

                MessageBox.Show("コンテキストメニューを登録しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"登録に失敗しました。\n{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        private RelayCommand? _InstallRegCmd = null;

        public RelayCommand UninstallRegCmd => this._UninstallRegCmd ??= new RelayCommand(() =>
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(RegDirPath, false);
                Registry.CurrentUser.DeleteSubKeyTree(RegBgPath,  false);

                MessageBox.Show("コンテキストメニューの登録を解除しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解除に失敗しました。\n{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        private RelayCommand? _UninstallRegCmd = null;

        private static void RegisterContextMenu(RegistryKey rootKey, string basePath, string exePath, string pathArg)
        {
            // MUIVerb を使わないとカスケードメニューとして表示されない
            using var parentKey = rootKey.CreateSubKey(basePath);
            parentKey.SetValue("MUIVerb",     "SimpleGitClient");
            parentKey.SetValue("SubCommands", "");
            parentKey.SetValue("Icon",        exePath);

            // .git が存在する場合のみ起動するよう cmd /c if exist でラップする
            using var commitKey = rootKey.CreateSubKey($@"{basePath}\shell\Commit");
            commitKey.SetValue("MUIVerb", "コミット (&C)");
            using var commitCmd = rootKey.CreateSubKey($@"{basePath}\shell\Commit\command");
            commitCmd.SetValue("", $"\"{exePath}\" commit \"{pathArg}\"");

            using var logKey = rootKey.CreateSubKey($@"{basePath}\shell\Log");
            logKey.SetValue("MUIVerb", "ログ (&L)");
            using var logCmd = rootKey.CreateSubKey($@"{basePath}\shell\Log\command");
            logCmd.SetValue("", $"\"{exePath}\" log \"{pathArg}\"");
        }
    }
}
