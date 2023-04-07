using KnittingAssistant.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.Model
{
    public class ImageProcessor
    {
        public double MainImageWidth { get; set; }
        public double MainImageHeight { get; set; }
        public double MainImageRatio => MainImageWidth / MainImageHeight;
        public bool? GridLinesVis { get; set; }
        public en_ImageStates CurrentImageState { get; set; }
        public ImageLoader ImageLoader { get; set; }
        public ImageSaver ImageSaver { get; set; }
        public ImageSplitter ImageSplitter { get; set; }
        public WriteableBitmap DisplayedImage { get; private set; }

        public event ImageHandler? UpdateImageNotify;

        public ImageProcessor()
        {
            MainImageWidth = 1.0;
            MainImageHeight = 1.0;
            GridLinesVis = null;
            CurrentImageState = en_ImageStates.emptyImage;
            ImageLoader = new ImageLoader();
            ImageSaver = new ImageSaver();
        }

        public WriteableBitmap UpdateMainImage(string imageFilename, en_ImageStates newImageState)
        {
            CurrentImageState = newImageState;
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(imageFilename));
            WriteableBitmap wbImage = new WriteableBitmap((BitmapSource)image.Source);

            return wbImage;
        }

        public void LoadImageOnForm(string imageFilename)
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
                WriteableBitmap displayedImage = UpdateMainImage(imageFilename, en_ImageStates.mainImageLoaded);
                CallUpdateImageNotify(displayedImage, imageWasBroken: false);
                MainImageWidth = displayedImage.PixelWidth;
                MainImageHeight = displayedImage.PixelHeight;
                GridLinesVis = null;
            }
        }

        public void CallUpdateImageNotify(WriteableBitmap wbImage, bool imageWasBroken)
        {
            DisplayedImage = wbImage;
            UpdateImageNotify?.Invoke(wbImage, imageWasBroken);
        }

        public bool LoadMainImageByClick()
        {
            string imageFilename = ImageLoader.GetImageFilename();
            if (imageFilename.Length > 0)
            {
                LoadImageOnForm(imageFilename);
                return true;
            }
            return false;
        }

        public delegate void ImageHandler(WriteableBitmap wbImage, bool imageWasBroken);
    }
}
