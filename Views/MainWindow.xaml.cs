using System.Windows;
using ZipApp.ViewModels;
using ZipApp.Views;

namespace ZipApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new MainPage());
        }
    }
}