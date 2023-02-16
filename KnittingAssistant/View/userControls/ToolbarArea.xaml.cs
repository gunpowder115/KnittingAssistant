using KnittingAssistant.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace KnittingAssistant.View.userControls
{
    /// <summary>
    /// Логика взаимодействия для ToolbarArea.xaml
    /// </summary>
    public partial class ToolbarArea : UserControl
    {
        private ToolbarAreaViewModel toolbarAreaViewModel;

        public ToolbarArea()
        {
            InitializeComponent();

            toolbarAreaViewModel = new ToolbarAreaViewModel();
            this.Loaded += ToolbarArea_Loaded;
        }

        private void ToolbarArea_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = toolbarAreaViewModel;
        }

    }
}
