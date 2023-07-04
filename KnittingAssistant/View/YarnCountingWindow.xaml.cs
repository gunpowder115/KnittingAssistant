using KnittingAssistant.Model;
using KnittingAssistant.ViewModel;
using System.Windows;

namespace KnittingAssistant.View
{
    /// <summary>
    /// Логика взаимодействия для YarnCountingWindow.xaml
    /// </summary>
    public partial class YarnCountingWindow : Window
    {
        private ColorStorage colorStorage;

        public YarnCountingWindow(ColorStorage colorStorage)
        {
            InitializeComponent();

            this.colorStorage = colorStorage;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new YarnCountingViewModel(colorStorage);
        }
    }
}
