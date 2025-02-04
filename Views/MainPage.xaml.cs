using System.Windows.Controls;
using ZipApp.ViewModels;

namespace ZipApp.Views
{
    /// <summary>
    /// MainPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
