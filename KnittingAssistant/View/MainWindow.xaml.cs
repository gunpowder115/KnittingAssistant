using KnittingAssistant.ViewModel;
using System.Windows;

namespace KnittingAssistant.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).ImageArea = imageArea;
            (DataContext as MainViewModel).PropertyArea = propertyArea;
            (DataContext as MainViewModel).ToolbarArea = toolbarArea;
        }
    }
}
