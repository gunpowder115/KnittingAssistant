using KnittingAssistant.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KnittingAssistant.ViewModel
{
    public class ToolbarAreaViewModel : ViewModelBase
    {
        private MainImageParams mainImageParams;
        private UserControlParams userControlParams;
        private readonly PropertyAreaViewModel propertyAreaViewModel;

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
                        mainImageParams.LoadMainImageByClick(userControlParams);
                        propertyAreaViewModel.UpdateForNewImage(mainImageParams.MainImageRatio);
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
                        mainImageParams.ImageSaver.SaveImage(mainImageParams.GridLinesVis.Value ? mainImageParams.ImageSplitter.GridBitmapImage : mainImageParams.ImageSplitter.SplittedBitmapImage);
                        mainImageParams.CurrentImageState = en_ImageStates.resultImageSaved;
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
                        if (mainImageParams.GridLinesVis != null)
                        {
                            Image splittedImage = new Image();
                            if ((bool)mainImageParams.GridLinesVis)
                            {
                                splittedImage.Source = mainImageParams.ImageSplitter.SplittedBitmapImage;
                                SwitchGridIconFilename = "../resources/grid_on_icon_1.png";
                                SwitchGridIconToolTip = "Показать сетку";
                            }
                            else
                            {
                                splittedImage.Source = mainImageParams.ImageSplitter.GridBitmapImage;
                                SwitchGridIconFilename = "../resources/grid_off_icon_1.png";
                                SwitchGridIconToolTip = "Скрыть сетку";
                            }
                            Grid.SetColumn(splittedImage, 0);
                            Grid.SetRow(splittedImage, 0);
                            userControlParams.ImageArea.mainImageContainer.Children.Add(splittedImage);
                            mainImageParams.GridLinesVis = !mainImageParams.GridLinesVis;
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

        public ToolbarAreaViewModel(MainImageParams mainImageParams, 
            UserControlParams userControlParams, PropertyAreaViewModel propertyAreaViewModel)
        {
            this.mainImageParams = mainImageParams;
            this.userControlParams = userControlParams;
            this.propertyAreaViewModel = propertyAreaViewModel;
            SwitchGridIconFilename = "../resources/grid_off_icon_1.png";
        }
    }
}
