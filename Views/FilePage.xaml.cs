using System.Windows.Controls;
using ZipApp.ViewModels;

namespace ZipApp.Views
{
    /// <summary>
    /// FilePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FilePage : Page
    {
        public FilePage(string filePath)
        {
            InitializeComponent();
            DataContext = new FileViewModel(filePath);
        }
    }
}
