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
            DataContext = new MainViewModel();
            (DataContext as MainViewModel).UserControlParams.ImageArea = imageArea;
            (DataContext as MainViewModel).UserControlParams.ToolbarArea = toolbarArea;

            this.propertyArea.DataContext = (DataContext as MainViewModel).PropertyAreaViewModel;
            this.imageArea.DataContext = (DataContext as MainViewModel).ImageAreaViewModel;
            this.toolbarArea.DataContext = (DataContext as MainViewModel).ToolbarAreaViewModel;
        }
    }
}
