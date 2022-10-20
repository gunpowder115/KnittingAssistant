using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;

namespace KnittingAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool imageLoaded;

        public MainWindow()
        {
            InitializeComponent();

            imageLoaded = false;
        }

        private void click_LoadImageOnForm(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialogPicture = new OpenFileDialog();
            fileDialogPicture.Filter = "Изображения|*.bmp;*.jpg;*.gif;*.png;*.tif";
            fileDialogPicture.FilterIndex = 1;

            if (fileDialogPicture.ShowDialog() == true)
            {
                mainImage.Source = new BitmapImage(new Uri(fileDialogPicture.FileName));
                imageLoaded = false;
            }
        }
    }
}
