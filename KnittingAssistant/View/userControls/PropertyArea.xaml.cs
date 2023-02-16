using KnittingAssistant.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace KnittingAssistant.View.userControls
{
    /// <summary>
    /// Логика взаимодействия для SettingsArea.xaml
    /// </summary>
    public partial class PropertyArea : UserControl
    {
        private PropertyAreaViewModel propertyAreaViewModel;

        public PropertyArea()
        {
            InitializeComponent();

            propertyAreaViewModel = new PropertyAreaViewModel();
            this.Loaded += PropertyArea_Loaded;
        }

        private void PropertyArea_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = propertyAreaViewModel;
        }
    }
}
