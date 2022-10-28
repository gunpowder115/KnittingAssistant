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
        PartitionSettings partitionSettings;
        static bool imageLoaded;
        private double mainImageWidth, mainImageHeight;

        public MainWindow()
        {
            InitializeComponent();

            partitionSettings = new PartitionSettings();

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
                imageLoaded = true;
                partitionSettings.SettingsIsEnabled = true;
            }
        }

        private void propertyTextBox_textInput(object sender, TextCompositionEventArgs e)
        {
            int val;
            if (!Int32.TryParse(e.Text, out val))
                e.Handled = true;
        }

        private void propertyTextBox_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void getMainlImageSize()
        {
            mainImageWidth = mainImage.Source.Width;
            mainImageHeight = mainImage.Source.Height;
        }


    }
}
