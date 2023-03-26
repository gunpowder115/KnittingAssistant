using KnittingAssistant.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.ViewModel
{
    public class MainImageParams
    {
        public double MainImageWidth { get; set; }
        public double MainImageHeight { get; set; }
        public double MainImageRatio => MainImageWidth / MainImageHeight;
        public Image MainImage { get; set; }
        public bool? GridLinesVis { get; set; }
        public en_ImageStates CurrentImageState { get; set; }
        public ImageLoader ImageLoader { get; set; }
        public ImageSaver ImageSaver { get; set; }
        public ImageSplitter ImageSplitter { get; set; }        

        public MainImageParams(double mainImageWidth, double mainImageHeight)
        {
            this.MainImageWidth = mainImageWidth;
            this.MainImageHeight = mainImageHeight;
            GridLinesVis = null;
            CurrentImageState = en_ImageStates.emptyImage;
            ImageLoader = new ImageLoader();
            ImageSaver = new ImageSaver();
        }
        public MainImageParams() : this(1.0, 1.0) { }

        public void LoadImageOnForm(string imageFilename, UserControlParams userControlParams)
        {
            bool loadNewImage = true;
            if (CurrentImageState == en_ImageStates.resultImageNotSaved)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Загрузить новое изображение?\nТекущее изображение не было сохранено!", "Внимание", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    loadNewImage = false;
                }
            }

            if (loadNewImage)
            {
                MainImage = CreateMainImage();
                MainImage.Source = new BitmapImage(new Uri(imageFilename));
                MainImageWidth = (MainImage.Source as BitmapSource).PixelWidth;
                MainImageHeight = (MainImage.Source as BitmapSource).PixelHeight;

                userControlParams.ImageArea.mainImageContainer.Children.Clear();
                Grid.SetColumn(MainImage, 0);
                Grid.SetRow(MainImage, 0);
                userControlParams.ImageArea.mainImageContainer.Children.Add(MainImage);

                GridLinesVis = null;
            }
        }

        public bool LoadMainImageByClick(UserControlParams userControlParams)
        {
            string imageFilename = ImageLoader.GetImageFilename();
            if (imageFilename.Length > 0)
            {
                LoadImageOnForm(imageFilename, userControlParams);
                return true;
            }
            return false;
        }

        private Image CreateMainImage()
        {
            Image mainImage = new Image();
            mainImage.Name = "mainImage";
            mainImage.Cursor = Cursors.Hand;
            mainImage.Stretch = Stretch.Uniform;
            CurrentImageState = en_ImageStates.mainImageLoaded;
            return mainImage;
        }
    }
}
