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

        private string switchGridIconToolTip;
        public string SwitchGridIconToolTip
        {
            get { return switchGridIconToolTip; }
            set
            {
                switchGridIconToolTip = value;
                OnPropertyChanged("SwitchGridIconToolTip");
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
                            propertyAreaViewModel.UpdateForNewImage(imageProcessor.MainImageRatio);
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
                        if (imageProcessor.ImageSaver.SaveImage(imageProcessor.GridLinesVis.Value ?
                            imageProcessor.ImageSplitter.GridBitmapImage : imageProcessor.ImageSplitter.SplittedBitmapImage))
                        {
                            imageProcessor.CurrentImageState = en_ImageStates.resultImageSaved;
                            imageProcessor.CallUpdateImageSaving();
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
                        if (imageProcessor.GridLinesVis != null)
                        {
                            if ((bool)imageProcessor.GridLinesVis)
                            {
                                imageProcessor.CallUpdateImageNotify(imageProcessor.ImageSplitter.SplittedBitmapImage, imageWasBroken: true);
                                SwitchGridIconFilename = "../resources/grid_on_icon_1.png";
                                SwitchGridIconToolTip = "Показать сетку";
                            }
                            else
                            {
                                imageProcessor.CallUpdateImageNotify(imageProcessor.ImageSplitter.GridBitmapImage, imageWasBroken: true);
                                SwitchGridIconFilename = "../resources/grid_off_icon_1.png";
                                SwitchGridIconToolTip = "Скрыть сетку";
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

        #endregion

        public ToolbarAreaViewModel(PropertyAreaViewModel propertyAreaViewModel, ImageProcessor imageProcessor)
        {
            this.propertyAreaViewModel = propertyAreaViewModel;
            this.imageProcessor = imageProcessor;
            SwitchGridIconFilename = "../resources/grid_off_icon_1.png";
            imageProcessor.UpdateSplittingStateNotify += UpdateSplittingState;
            ToolbarAreaIsEnabled = true;
        }

        private void UpdateSplittingState()
        {
            ToolbarAreaIsEnabled = !imageProcessor.IsSplitting;
        }
    }
}
