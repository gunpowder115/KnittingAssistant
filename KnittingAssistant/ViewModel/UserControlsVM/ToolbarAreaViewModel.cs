using KnittingAssistant.Model;
using KnittingAssistant.View;

namespace KnittingAssistant.ViewModel
{
    public class ToolbarAreaViewModel : ViewModelBase
    {
        private readonly PropertyAreaViewModel propertyAreaViewModel;
        private ImageProcessor imageProcessor;

        #region Dependency Property

        private string m_SwitchGridIconFilename;
        public string SwitchGridIconFilename
        {
            get { return m_SwitchGridIconFilename; }
            set
            {
                m_SwitchGridIconFilename = value;
                OnPropertyChanged("SwitchGridIconFilename");
            }
        }

        private string m_SwitchGridIconToolTip;
        public string SwitchGridIconToolTip
        {
            get { return m_SwitchGridIconToolTip; }
            set
            {
                m_SwitchGridIconToolTip = value;
                OnPropertyChanged("SwitchGridIconToolTip");
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
                        imageProcessor.ImageSaver.SaveImage(imageProcessor.GridLinesVis.Value ? imageProcessor.ImageSplitter.GridBitmapImage : imageProcessor.ImageSplitter.SplittedBitmapImage);
                        imageProcessor.CurrentImageState = en_ImageStates.resultImageSaved;
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
                            //Image splittedImage = new Image();
                            if ((bool)imageProcessor.GridLinesVis)
                            {
                                imageProcessor.CallUpdateImageNotify(imageProcessor.ImageSplitter.SplittedBitmapImage);
                                //imageAreaViewModel.DisplayedImage = imageProcessor.ImageSplitter.SplittedBitmapImage;
                                //splittedImage.Source = imageProcessor.ImageSplitter.SplittedBitmapImage;
                                SwitchGridIconFilename = "../resources/grid_on_icon_1.png";
                                SwitchGridIconToolTip = "Показать сетку";
                            }
                            else
                            {
                                imageProcessor.CallUpdateImageNotify(imageProcessor.ImageSplitter.GridBitmapImage);
                                //imageAreaViewModel.DisplayedImage = imageProcessor.ImageSplitter.GridBitmapImage;
                                //splittedImage.Source = imageProcessor.ImageSplitter.GridBitmapImage;
                                SwitchGridIconFilename = "../resources/grid_off_icon_1.png";
                                SwitchGridIconToolTip = "Скрыть сетку";
                            }
                            //Grid.SetColumn(splittedImage, 0);
                            //Grid.SetRow(splittedImage, 0);
                            //userControlParams.ImageArea.mainImageContainer.Children.Add(splittedImage);
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
        }
    }
}
