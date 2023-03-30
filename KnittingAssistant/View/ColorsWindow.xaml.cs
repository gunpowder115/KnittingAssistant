using KnittingAssistant.ViewModel;
using System.Windows;

namespace KnittingAssistant.View
{
    /// <summary>
    /// Логика взаимодействия для ColorsWindow.xaml
    /// </summary>
    public partial class ColorsWindow : Window
    {
        public ColorsWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new  ColorsViewModel();
        }
    }
}
