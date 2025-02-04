using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Navigation;
using ZipApp.Views;

namespace ZipApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [RelayCommand]
        private void OpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "압축 파일|*.zip"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    var filePage = new FilePage(filePath);
                    mainWindow.MainFrame.Navigate(filePage);
                }
            }
        }
    }

}

