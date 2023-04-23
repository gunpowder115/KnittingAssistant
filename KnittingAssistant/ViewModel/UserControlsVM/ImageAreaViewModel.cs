using KnittingAssistant.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.ViewModel
{
    public class ImageAreaViewModel : ViewModelBase
    {
        private readonly PropertyAreaViewModel propertyAreaViewModel;
        private ImageProcessor imageProcessor;
        private DisplayedImageModes displayedImageMode;
        private WriteableBitmap sourceImageWb;
        private WriteableBitmap splittedImageWb;
        private Dictionary<DisplayedImageModes, string> buttonsToolTip;

        private const string DefaultFilename = "D:/Development/KnittingAssistant/KnittingAssistant/View/resources/default_image.png";

        #region Dependency Property

        private WriteableBitmap displayedImage;
        public WriteableBitmap DisplayedImage
        {
            get { return displayedImage; }
            set
            {
                displayedImage = value;
                OnPropertyChanged("DisplayedImage");
            }
        }

        private Visibility imageSwitchingVisibility;
        public Visibility ImageSwitchingVisibility
        {
            get { return imageSwitchingVisibility; }
            set
            {
                imageSwitchingVisibility = value;
                OnPropertyChanged("ImageSwitchingVisibility");
            }
        }

        private string buttonImageSwitchingToolTip;
        public string ButtonImageSwitchingToolTip
        {
            get { return buttonImageSwitchingToolTip; }
            set
            {
                buttonImageSwitchingToolTip = value;
                OnPropertyChanged("ButtonImageSwitchingToolTip");
            }
        }

        private Visibility dropInfoVis;
        public Visibility DropInfoVis
        {
            get { return dropInfoVis; }
            set
            {
                dropInfoVis = value;
                OnPropertyChanged("DropInfoVis");
            }
        }

        private bool imageAreaIsEnabled;
        public bool ImageAreaIsEnabled
        {
            get { return imageAreaIsEnabled; }
            set
            {
                imageAreaIsEnabled = value;
                OnPropertyChanged("ImageAreaIsEnabled");
            }
        }

        private string imageToolTip;
        public string ImageToolTip
        {
            get { return imageToolTip; }
            set
            {
                imageToolTip = value;
                OnPropertyChanged("ImageToolTip");
            }
        }

        #endregion

        #region Relay Commands

        private RelayCommand loadMainImageByClickCommand;
        public RelayCommand LoadMainImageByClickCommand
        {
            get
            {
                return loadMainImageByClickCommand ??
                    (loadMainImageByClickCommand = new RelayCommand(obj =>
                    {
                        bool loadNewImage = imageProcessor.LoadMainImageByClick();
                        if (loadNewImage)
                            propertyAreaViewModel.UpdateForNewImage(imageProcessor.MainImageRatio);
                    }));
            }
        }

        private RelayCommand clickOnMainImageCommand;
        public RelayCommand ClickOnMainImageCommand
        {
            get
            {
                return clickOnMainImageCommand ??
                    (clickOnMainImageCommand = new RelayCommand(obj =>
                    {
                        if (imageProcessor.CurrentImageState == en_ImageStates.resultImageNotSaved)
                        {
                            if (imageProcessor.ImageSaver.SaveImage(imageProcessor.GridLinesVis.Value ?
                                imageProcessor.ImageSplitter.GridBitmapImage : imageProcessor.ImageSplitter.SplittedBitmapImage))
                            {
                                imageProcessor.CurrentImageState = en_ImageStates.resultImageSaved;
                                imageProcessor.CallUpdateImageSaving();
                            }
                        }
                        else
                        {
                            LoadMainImageByClickCommand.Execute(obj);
                        }
                    }));
            }
        }

        private RelayCommand switchImageCommand;
        public RelayCommand SwitchImageCommand
        {
            get
            {
                return switchImageCommand ??
                    (switchImageCommand = new RelayCommand(obj =>
                    {
                        SwitchImage();
                    }));
            }
        }

        #endregion

        public ImageAreaViewModel(PropertyAreaViewModel propertyAreaViewModel, ImageProcessor imageProcessor)
        {
            this.propertyAreaViewModel = propertyAreaViewModel;
            this.imageProcessor = imageProcessor;
            displayedImageMode = DisplayedImageModes.splittedImage;
            ImageToolTip = "Загрузить изображение";

            buttonsToolTip = new Dictionary<DisplayedImageModes, string>();
            buttonsToolTip.Add(DisplayedImageModes.splittedImage, "Исходное изображение");
            buttonsToolTip.Add(DisplayedImageModes.sourceImage, "Разбитое изображение");

            DisplayedImage = imageProcessor.UpdateMainImage(DefaultFilename, en_ImageStates.emptyImage);
            ImageSwitchingVisibility = Visibility.Hidden;
            DropInfoVis = Visibility.Visible;
            ImageAreaIsEnabled = true;
            ButtonImageSwitchingToolTip = buttonsToolTip[displayedImageMode];

            imageProcessor.ImageUpdated += UpdateImageEventHandler;
            imageProcessor.SplittingStateUpdated += () => ImageAreaIsEnabled = !imageProcessor.IsSplitting;
            imageProcessor.ImageSaved += () => ImageToolTip = imageProcessor.CurrentImageState == en_ImageStates.resultImageNotSaved ?
                                                "Сохранить изображение" : "Загрузить изображение";
        }

        public void LoadMainImageByDropCommand(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (imageProcessor.ImageLoader.IsSupportedFormat(imageFilename))
            {
                imageProcessor.LoadImageOnForm(imageFilename);
                propertyAreaViewModel.UpdateForNewImage(imageProcessor.MainImageRatio);
                imageProcessor.CallUpdateImageSaving();
            }
        }

        private void UpdateImageEventHandler(WriteableBitmap wbImage, bool imageWasBroken)
        {
            DisplayedImage = wbImage;
            if (imageWasBroken)
            {
                ImageSwitchingVisibility = Visibility.Visible;
                splittedImageWb = wbImage;
            }
            else
            {
                ImageSwitchingVisibility = Visibility.Hidden;
                DropInfoVis = Visibility.Hidden;
                displayedImageMode = DisplayedImageModes.splittedImage;
                sourceImageWb = wbImage;
            }
        }

        private void SwitchImage()
        {
            switch (displayedImageMode)
            {
                case DisplayedImageModes.splittedImage:
                    if (sourceImageWb != null)
                    {
                        DisplayedImage = sourceImageWb;
                        displayedImageMode = DisplayedImageModes.sourceImage;
                    }
                    break;
                case DisplayedImageModes.sourceImage:
                    if (splittedImageWb != null)
                    {
                        DisplayedImage = splittedImageWb;
                        displayedImageMode = DisplayedImageModes.splittedImage;
                    }
                    break;
                default: break;
            }
            ButtonImageSwitchingToolTip = buttonsToolTip[displayedImageMode];
        }

        private enum DisplayedImageModes
        {
            sourceImage,
            splittedImage
        }
    }
}
