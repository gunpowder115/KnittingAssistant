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

using KnittingAssistant.ViewModel;

namespace KnittingAssistant.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static bool imageLoaded;
        private double mainImageWidth, mainImageHeight;
        private double mainImageRatio;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;

            imageLoaded = false;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new MainViewModel();
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
            getMainlImageSize();
            setDisplayImageSize();
        }

        private void clickButton_ApplyProperty(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel).SetDisplayImageSize(double.Parse(MainImageWidthTextBlock.Text), double.Parse(MainImageHeightTextBlock.Text));
        }

        private void getMainlImageSize()
        {
            mainImageWidth = mainImage.Source.Width;
            mainImageHeight = mainImage.Source.Height;
            mainImageRatio = mainImageWidth / mainImageHeight;
            (DataContext as MainViewModel).MainImageRatio = mainImageRatio;
        }

        private void MainImageWidthTextBlock_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void setDisplayImageSize()
        {
            (DataContext as MainViewModel).SetDisplayImageSize(100.0, 100.0 / mainImageRatio);
        }
    }
}
