using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.Model
{
    public class ImageProcessor
    {
        public double MainImageWidth { get; private set; }
        public double MainImageHeight { get; private set; }
        public double MainImageRatio => MainImageWidth / MainImageHeight;
        public bool? GridLinesVis { get; set; }
        public en_ImageStates CurrentImageState { get; set; }
        public ImageLoader ImageLoader { get; private set; }
        public ImageSaver ImageSaver { get; private set; }
        public ImageSplitter ImageSplitter { get; set; }
        public WriteableBitmap DisplayedImage { get; private set; }
        public WriteableBitmap SourceImage { get; private set; }
        public bool IsSplitting { get; private set; }

        public event Action<WriteableBitmap, bool> ImageUpdated;
        public event Action SplittingStateUpdated;
        public event Action ImageStateUpdated;
        public event Action<double> PropertiesUpdated;

        public ImageProcessor()
        {
            MainImageWidth = 1.0;
            MainImageHeight = 1.0;
            GridLinesVis = null;
            CurrentImageState = en_ImageStates.emptyImage;
            IsSplitting = false;
            ImageLoader = new ImageLoader();
            ImageSaver = new ImageSaver();
        }

        public WriteableBitmap UpdateMainImage(object newImage, en_ImageStates newImageState)
        {
            CurrentImageState = newImageState;
            WriteableBitmap wbImage;
            try
            {
                if (newImage is string imageFilename)
                {
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri(imageFilename));
                    wbImage = new WriteableBitmap((BitmapSource)image.Source);
                }
                else if (newImage is BitmapSource bitmapSource)
                {
                    wbImage = new WriteableBitmap(bitmapSource);
                }
                else
                {
                    throw new Exception("Тип newImage не определён!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                wbImage = null;
            }

            return wbImage;
        }

        public void LoadImageOnForm(object newImage)
        {
            bool loadNewImage = true;
            if (CurrentImageState == en_ImageStates.resultImageNotSaved)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "Загрузить новое изображение?\nТекущее изображение не было сохранено!", "Внимание",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    loadNewImage = false;
                }
            }

            if (loadNewImage)
            {
                WriteableBitmap displayedImage = UpdateMainImage(newImage, en_ImageStates.mainImageLoaded);
                SourceImage = displayedImage;
                CallUpdateImageNotify(displayedImage, imageWasBroken: false);
                MainImageWidth = displayedImage.PixelWidth;
                MainImageHeight = displayedImage.PixelHeight;
                GridLinesVis = null;
                PropertiesUpdated?.Invoke(MainImageRatio);
            }
        }

        public void CallUpdateImageNotify(WriteableBitmap wbImage, bool imageWasBroken)
        {
            DisplayedImage = wbImage;
            ImageUpdated?.Invoke(wbImage, imageWasBroken);
            ImageStateUpdated?.Invoke();
        }

        public void CallUpdateSplittingStateNotify(bool isSplitting)
        {
            IsSplitting = isSplitting;
            SplittingStateUpdated?.Invoke();
        }

        public void CallUpdateImageSaving()
        {
            if (CurrentImageState == en_ImageStates.resultImageSaved)
                MessageBox.Show("Изображение успешно сохранено!", "Сохранение", MessageBoxButton.OK,
                    MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
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
    }

    public struct Fragmentation
    {
        public int mainCount;
        public int mainSize;
        public int secondaryCount;
        public int secondarySize;
        public int SumCount => mainCount + secondaryCount;
    }

    public enum en_ImageStates
    {
        emptyImage,
        mainImageLoaded,
        mainImageSplitting,
        resultImageNotSaved,
        resultImageSaved
    }
}
