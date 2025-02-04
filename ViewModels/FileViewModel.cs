using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Windows;
using ZipApp.Views;
using ZipApp.Models;
using System.Collections.ObjectModel;
using System.IO.Compression;

namespace ZipApp.ViewModels
{
    public partial class FileViewModel : ObservableObject
    {
        private string currentDirectory = ""; // ZIP 내부 경로
        private string zipFilePath = ""; // ZIP 파일 경로

        [ObservableProperty]
        private string _filePath;

        [ObservableProperty]
        private ObservableCollection<ZipItemModel> _items = new();

        [ObservableProperty]
        private ZipItemModel _selectedItem;

        partial void OnSelectedItemChanged(ZipItemModel value)
        {
            if (value != null)
            {
                OnItemSelected(value);
            }
        }

        public FileViewModel(string path)
        {
            FilePath = path;
            zipFilePath = path;
            LoadZipContents();
        }

        [RelayCommand]
        private void LoadZipContents()
        {
            Items.Clear();

            using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
            {
                var entries = archive.Entries
                    .Where(entry => entry.FullName.StartsWith(currentDirectory) && entry.FullName != currentDirectory)
                    .Select(entry => entry.FullName.Substring(currentDirectory.Length).TrimStart('/'))
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                if (!string.IsNullOrEmpty(currentDirectory))
                {
                    Items.Add(new ZipItemModel
                    {
                        Name = "..",
                        Type = "Directory",
                        Size = "-",
                        DateModified = "-"
                    });
                }

                var folderNames = entries
                    .Where(name => name.Contains("/"))
                    .Select(name => name.Split('/')[0] + "/")
                    .Distinct();

                foreach (var folder in folderNames)
                {
                    Items.Add(new ZipItemModel
                    {
                        Name = folder.TrimEnd('/'),
                        Type = "Directory",
                        Size = "-",
                        DateModified = "-"
                    });
                }

                var files = archive.Entries
                    .Where(entry => entry.FullName.StartsWith(currentDirectory) && !entry.FullName.EndsWith("/") &&
                                    entry.FullName.Substring(currentDirectory.Length).TrimStart('/').Contains("/") == false)
                    .Select(entry => new ZipItemModel
                    {
                        Name = entry.FullName.Substring(currentDirectory.Length),
                        Type = "File",
                        Size = FormatFileSize(entry.Length),
                        DateModified = entry.LastWriteTime.LocalDateTime.ToString("yyyy-MM-dd HH:mm")
                    });

                foreach (var file in files)
                {
                    Items.Add(file);
                }

                FilePath = currentDirectory;
            }
        }

        [RelayCommand]
        private void OnItemSelected(ZipItemModel item)
        {
            if (item.Name == "..")
            {
                if (!string.IsNullOrEmpty(currentDirectory))
                {
                    int lastSlashIndex = currentDirectory.LastIndexOf('/', currentDirectory.Length - 2);
                    currentDirectory = lastSlashIndex >= 0 ? currentDirectory.Substring(0, lastSlashIndex + 1) : "";
                }
                LoadZipContents();
            }
            else if (item.Type == "Directory")
            {
                currentDirectory += item.Name + "/";
                LoadZipContents();
            }
            else
            {
                MessageBox.Show($"파일 선택됨: {item.Name}", "파일 열기");
            }
        }

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

        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
            return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
        }

    }
}
