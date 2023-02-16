using KnittingAssistant.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace KnittingAssistant.View.userControls
{
    /// <summary>
    /// Логика взаимодействия для ImageArea.xaml
    /// </summary>
    public partial class ImageArea : UserControl
    {
        private ImageAreaViewModel imageAreaViewModel;

        public ImageArea()
        {
            InitializeComponent();

            imageAreaViewModel = new ImageAreaViewModel();
            this.Loaded += ImageArea_Loaded;
        }

        private void ImageArea_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = imageAreaViewModel;
        }
    }
}
