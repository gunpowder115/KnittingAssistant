using KnittingAssistant.Model;
using KnittingAssistant.View;

namespace KnittingAssistant.ViewModel
{
    public class ToolbarAreaViewModel : ViewModelBase
    {
        private readonly PropertyAreaViewModel propertyAreaViewModel;
        private ImageProcessor imageProcessor;

        #region Dependency Property

        private string switchGridIconFilename;
        public string SwitchGridIconFilename
        {
            get { return switchGridIconFilename; }
            set
            {
                switchGridIconFilename = value;
                OnPropertyChanged("SwitchGridIconFilename");
            }
        }

        private bool toolbarAreaIsEnabled;
        public bool ToolbarAreaIsEnabled
        {
            get { return toolbarAreaIsEnabled; }
            set
            {
                toolbarAreaIsEnabled = value;
                OnPropertyChanged("ToolbarAreaIsEnabled");
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
                        {
                            propertyAreaViewModel.UpdateForNewImage(imageProcessor.MainImageRatio);
                            imageProcessor.CallUpdateImageSaving();
                        }
                    }));
            }
        }

        private RelayCommand saveImageToFileCommand;
        public RelayCommand SaveImageToFileCommand
        {
            get
            {
                return saveImageToFileCommand ??
                    (saveImageToFileCommand = new RelayCommand(obj =>
                    {
                        if (imageProcessor.CurrentImageState == en_ImageStates.resultImageNotSaved ||
                            imageProcessor.CurrentImageState == en_ImageStates.resultImageSaved)
                        {
                            if (imageProcessor.ImageSaver.SaveImage(imageProcessor.GridLinesVis.Value ?
                                imageProcessor.ImageSplitter.GridBitmapImage : imageProcessor.ImageSplitter.SplittedBitmapImage))
                            {
                                imageProcessor.CurrentImageState = en_ImageStates.resultImageSaved;
                                imageProcessor.CallUpdateImageSaving();
                            }
                        }
                    }));
            }
        }

        private RelayCommand changeGridLinesVisCommand;
        public RelayCommand ChangeGridLinesVisCommand
        {
            get
            {
                return changeGridLinesVisCommand ??
                    (changeGridLinesVisCommand = new RelayCommand(obj =>
                    {
                        if (imageProcessor.GridLinesVis != null &&
                            (imageProcessor.CurrentImageState == en_ImageStates.resultImageNotSaved ||
                            imageProcessor.CurrentImageState == en_ImageStates.resultImageSaved))
                        {
                            if ((bool)imageProcessor.GridLinesVis)
                            {
                                imageProcessor.CallUpdateImageNotify(imageProcessor.ImageSplitter.SplittedBitmapImage, imageWasBroken: true);
                                SwitchGridIconFilename = "pack://application:,,,/Resources/Images/grid_on_icon.png";
                            }
                            else
                            {
                                imageProcessor.CallUpdateImageNotify(imageProcessor.ImageSplitter.GridBitmapImage, imageWasBroken: true);
                                SwitchGridIconFilename = "pack://application:,,,/Resources/Images/grid_off_icon.png";
                            }
                            imageProcessor.GridLinesVis = !imageProcessor.GridLinesVis;
                        }
                    }));
            }
        }

        private RelayCommand openColorsManagementWindowCommand;
        public RelayCommand OpenColorsManagementWindowCommand
        {
            get
            {
                return openColorsManagementWindowCommand ??
                    (openColorsManagementWindowCommand = new RelayCommand(obj =>
                    {
                        ColorsWindow colorsWindow = new ColorsWindow();
                        colorsWindow.ShowDialog();
                    }));
            }
        }

        private RelayCommand openInfoWindowCommand;
        public RelayCommand OpenInfoWindowCommand
        {
            get
            {
                return openInfoWindowCommand ??
                    (openInfoWindowCommand = new RelayCommand(obj =>
                    {
                        InfoWindow infoWindow = new InfoWindow();
                        infoWindow.ShowDialog();
                    }));
            }
        }

        #endregion

        public ToolbarAreaViewModel(PropertyAreaViewModel propertyAreaViewModel, ImageProcessor imageProcessor)
        {
            this.propertyAreaViewModel = propertyAreaViewModel;
            this.imageProcessor = imageProcessor;
            SwitchGridIconFilename = "pack://application:,,,/Resources/Images/grid_off_icon.png";
            ToolbarAreaIsEnabled = true;

            imageProcessor.SplittingStateUpdated += () => ToolbarAreaIsEnabled = !imageProcessor.IsSplitting;
            imageProcessor.ImageStateUpdated += () =>
            {
                if (imageProcessor.CurrentImageState == en_ImageStates.mainImageLoaded)
                    SwitchGridIconFilename = "pack://application:,,,/Resources/Images/grid_off_icon.png";
            };
        }
    }
}
