using KnittingAssistant.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.View.userControls
{
    /// <summary>
    /// Логика взаимодействия для ImageArea.xaml
    /// </summary>
    public partial class ImageArea : UserControl
    {
        private bool imageLoaded;

        public ImageArea()
        {
            InitializeComponent();
        }

        private void click_LoadImageOnForm(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialogPicture = new OpenFileDialog();
            fileDialogPicture.Filter = "Изображения|*.bmp;*.jpg;*.gif;*.png;*.tif";
            fileDialogPicture.FilterIndex = 1;

            if (fileDialogPicture.ShowDialog() == true)
            {
                string imageFilename = fileDialogPicture.FileName;
                loadImageOnForm(imageFilename);
            }
        }

        private void drop_LoadImageOnForm(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (imageFilename.Contains(".bmp") || imageFilename.Contains(".jpg") ||
                imageFilename.Contains(".gif") || imageFilename.Contains(".png") ||
                imageFilename.Contains(".tif"))
            {
                loadImageOnForm(imageFilename);
            }
        }

        private void loadImageOnForm(string imageFilename)
        {
            mainImage.Source = new BitmapImage(new Uri(imageFilename));
            imageLoaded = true;
            (DataContext as MainViewModel).SetSettingsIsEnabled(imageLoaded);
            (DataContext as MainViewModel).MainImageWidth = mainImage.Source.Width;
            (DataContext as MainViewModel).MainImageHeight = mainImage.Source.Height;

            (DataContext as MainViewModel).DisplayImageWidth = 100 * (DataContext as MainViewModel).DisplayImageFragmentWidth;
        }
    }
}
