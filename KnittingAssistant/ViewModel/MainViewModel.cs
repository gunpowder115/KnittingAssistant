using System;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using KnittingAssistant.View.userControls;
using KnittingAssistant.View;
using KnittingAssistant.Model;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.IO;

namespace KnittingAssistant.ViewModel
{
    public enum en_ImageStates
    {
        emptyImage,
        mainImageLoaded,
        mainImageSplitting,
        resultImageNotSaved,
        resultImageSaved
    }

    public struct Fragmentation
    {
        public int mainCount;
        public int mainSize;
        public int secondaryCount;
        public int secondarySize;
        public int SumCount => mainCount + secondaryCount;
    }

    public class MainViewModel : ViewModelBase
    {
        #region Dependency Properties

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

        #region User Controls Dependency Property

        private ImageArea m_ImageArea;
        public ImageArea ImageArea
        {
            get { return m_ImageArea; }
            set
            {
                m_ImageArea = value;
                OnPropertyChanged("ImageArea");
            }
        }

        private ToolbarArea m_ToolbarArea;
        public ToolbarArea ToolbarArea
        {
            get { return m_ToolbarArea; }
            set
            {
                m_ToolbarArea = value;
                OnPropertyChanged("ToolbarArea");
            }
        }

        #endregion

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
                        string imageFilename = imageLoader.GetImageFilename();
                        if (imageFilename.Length > 0)
                            LoadImageOnForm(imageFilename);
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
                        imageSaver.SaveImage(gridLinesVis.Value ? splitImage.GridBitmapImage : splitImage.SplittedBitmapImage);
                        currentImageState = en_ImageStates.resultImageSaved;
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
                        if (gridLinesVis != null)
                        {
                            Image splittedImage = new Image();
                            if ((bool)gridLinesVis)
                            {
                                splittedImage.Source = splitImage.SplittedBitmapImage;
                                SwitchGridIconFilename = "../resources/grid_on_icon_1.png";
                                SwitchGridIconToolTip = "Показать сетку";
                            }
                            else
                            {
                                splittedImage.Source = splitImage.GridBitmapImage;
                                SwitchGridIconFilename = "../resources/grid_off_icon_1.png";
                                SwitchGridIconToolTip = "Скрыть сетку";
                            }
                            Grid.SetColumn(splittedImage, 0);
                            Grid.SetRow(splittedImage, 0);
                            ImageArea.mainImageContainer.Children.Add(splittedImage);
                            gridLinesVis = !gridLinesVis;
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

        private double MainImageWidth = 1.0;
        private double MainImageHeight = 1.0;
        public double MainImageRatio => MainImageWidth / MainImageHeight;

        private double FragmentRatio = 1.0;

        private Image mainImage;
        private Grid fragmentsGrid;
        private ImageSplitter splitImage;
        private bool? gridLinesVis;

        private WriteableBitmap[,] resultImageBitmaps;
        private Color[,] resultImageColors;

        private en_ImageStates currentImageState;
        private ImageLoader imageLoader;
        private ImageSaver imageSaver;

        public MainViewModel()
        {
            gridLinesVis = null;
            SwitchGridIconFilename = "../resources/grid_off_icon_1.png";
            currentImageState = en_ImageStates.emptyImage;
            imageLoader = new ImageLoader();
            imageSaver = new ImageSaver();
        }

        public void LoadMainImageByDropCommand(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (imageLoader.IsSupportedFormat(imageFilename))
            {
                LoadImageOnForm(imageFilename);
            }
        }

        private void LoadImageOnForm(string imageFilename)
        {
            bool loadNewImage = true;
            if (currentImageState == en_ImageStates.resultImageNotSaved)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Загрузить новое изображение?\nТекущее изображение не было сохранено!", "Внимание", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    loadNewImage = false;
                }
            }

            if (loadNewImage)
            {
                mainImage = CreateMainImage();
                mainImage.Source = new BitmapImage(new Uri(imageFilename));
                MainImageWidth = (mainImage.Source as BitmapSource).PixelWidth;
                MainImageHeight = (mainImage.Source as BitmapSource).PixelHeight;

                ImageArea.mainImageContainer.Children.Clear();
                Grid.SetColumn(mainImage, 0);
                Grid.SetRow(mainImage, 0);
                ImageArea.mainImageContainer.Children.Add(mainImage);

                gridLinesVis = null;
            }
        }

        private Image CreateMainImage()
        {
            Image mainImage = new Image();
            mainImage.Name = "mainImage";
            mainImage.Cursor = Cursors.Hand;
            mainImage.Stretch = Stretch.Uniform;
            currentImageState = en_ImageStates.mainImageLoaded;
            return mainImage;
        }
    }
}
