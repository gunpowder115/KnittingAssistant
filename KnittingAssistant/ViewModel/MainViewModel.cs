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

namespace KnittingAssistant.ViewModel
{
    public enum en_ImageStates
    {
        mainImage,
        fragmentsGrid
    }

    public class MainViewModel : ViewModelBase
    {
        #region Dependency Properties

        private bool m_SettingsIsEnabled;
        public bool SettingsIsEnabled
        {
            get { return m_SettingsIsEnabled; }
            set
            {
                m_SettingsIsEnabled = value;
                OnPropertyChanged("SettingsIsEnabled");
            }
        }

        private double m_DisplayImageWidth;
        public double DisplayImageWidth
        {
            get { return m_DisplayImageWidth; }
            set
            {
                m_DisplayImageWidth = value;
                OnPropertyChanged("DisplayImageWidth");
            }
        }

        private double m_DisplayImageHeight;
        public double DisplayImageHeight
        {
            get { return m_DisplayImageHeight; }
            set
            {
                m_DisplayImageHeight = value;
                OnPropertyChanged("DisplayImageHeight");
            }
        }

        private double m_DisplayImageFragmentWidth;
        public double DisplayImageFragmentWidth
        {
            get { return m_DisplayImageFragmentWidth; }
            set
            {
                m_DisplayImageFragmentWidth = value;
                OnPropertyChanged("DisplayImageFragmentWidth");
            }
        }

        private double m_DisplayImageFragmentHeight;
        public double DisplayImageFragmentHeight
        {
            get { return m_DisplayImageFragmentHeight; }
            set
            {
                m_DisplayImageFragmentHeight = value;
                OnPropertyChanged("DisplayImageFragmentHeight");
            }
        }

        private bool m_KeepRatioOfMainImage;
        public bool KeepRatioOfMainImage
        {
            get { return m_KeepRatioOfMainImage; }
            set
            {
                m_KeepRatioOfMainImage = value;
                OnPropertyChanged("KeepRatioOfMainImage");
            }
        }

        private bool m_IsSquareFragment;
        public bool IsSquareFragment
        {
            get { return m_IsSquareFragment; }
            set
            {
                m_IsSquareFragment = value;
                OnPropertyChanged("IsSquareFragment");
            }
        }

        private string m_SplittingProcessName;
        public string SplittingProcessName
        {
            get { return m_SplittingProcessName; }
            set
            {
                m_SplittingProcessName = value;
                OnPropertyChanged("SplittingProcessName");
            }
        }

        private double m_SplittingProcessValue;
        public double SplittingProcessValue
        {
            get { return m_SplittingProcessValue; }
            set
            {
                m_SplittingProcessValue = value;
                OnPropertyChanged("SplittingProcessValue");
            }
        }

        private Visibility m_SplittingProcessVisibility;
        public Visibility SplittingProcessVisibility
        {
            get { return m_SplittingProcessVisibility; }
            set
            {
                m_SplittingProcessVisibility = value;
                OnPropertyChanged("SplittingProcessVisibility");
            }
        }

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

        private PropertyArea m_PropertyArea;
        public PropertyArea PropertyArea
        {
            get { return m_PropertyArea; }
            set
            {
                m_PropertyArea = value;
                OnPropertyChanged("PropertyArea");
            }
        }

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

        private RelayCommand breakImageCommand;
        public RelayCommand BreakImageCommand
        {
            get
            {
                return breakImageCommand ??
                    (breakImageCommand = new RelayCommand(obj =>
                    {
                        SplittingProcessName = "Разбиение изображения...";
                        SplittingProcessVisibility = Visibility.Visible;
                        SplittingProcessValue = 0;

                        FragmentCountWidth = SetFragmentCount(DisplayImageWidth, DisplayImageFragmentWidth, FragmentWidthInPixels, MainImageWidth);
                        DisplayImageWidth = SetDisplayImageSize(FragmentCountWidth, DisplayImageFragmentWidth);
                        FragmentCountHeight = SetFragmentCount(DisplayImageHeight, DisplayImageFragmentHeight, FragmentHeightInPixels, MainImageHeight);
                        DisplayImageHeight = SetDisplayImageSize(FragmentCountHeight, DisplayImageFragmentHeight);

                        resultImageBitmaps = new WriteableBitmap[FragmentCountWidth, FragmentCountHeight];
                        resultImageColors = new Color[FragmentCountWidth, FragmentCountHeight];

                        splitImage = new SplitImage(mainImage, FragmentCountWidth, FragmentCountHeight, 
                            (int)Math.Round(FragmentWidthInPixels), (int)Math.Round(FragmentHeightInPixels));

                        fragmentsGrid = InitGridForFragments();

                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.WorkerSupportsCancellation = true;
                        worker.DoWork += worker_DoWork;
                        worker.ProgressChanged += worker_ProgressChanged;
                        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                        worker.RunWorkerAsync(splitImage);
                    }));
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            splitImage = (SplitImage)e.Argument;

            int progressPercentage = 0;
            for (int i = 0; i < FragmentCountWidth; i++)
            {
                for (int j = 0; j < FragmentCountHeight; j++)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        resultImageColors[i, j] = splitImage.DoSplitImage(i, j);
                        Border fragment = new Border();

                        fragment.Height = ImageArea.mainImageContainer.ActualHeight / FragmentCountHeight;
                        fragment.Width = fragment.Height;
                        fragment.BorderThickness = new Thickness(0.5);
                        fragment.BorderBrush = Brushes.Black;
                        fragment.Background = new SolidColorBrush(resultImageColors[i, j]);

                        Grid.SetColumn(fragment, i);
                        Grid.SetRow(fragment, j);
                        fragmentsGrid.Children.Add(fragment);
                    }));

                    progressPercentage = Convert.ToInt32((double)(i * FragmentCountHeight + j) / (FragmentCountWidth * FragmentCountHeight) * 100);
                    if (progressPercentage >= 100)
                        progressPercentage = 99;
                    (sender as BackgroundWorker).ReportProgress(progressPercentage);
                }
            }

            e.Result = resultImageColors;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SplittingProcessValue = e.ProgressPercentage;
            if (SplittingProcessValue == 99)
                SplittingProcessName = "Подготовка изображения...";
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ImageArea.mainImageContainer.Children.Clear();

            Image splittedImage = new Image();
            splittedImage.Source = splitImage.GridBitmapImage;
            Grid.SetColumn(splittedImage, 0);
            Grid.SetRow(splittedImage, 0);
            ImageArea.mainImageContainer.Children.Add(splittedImage);
            gridLinesVis = true;

            SplittingProcessName = "Готово!";
            SplittingProcessValue = 100;
        }

        private RelayCommand loadMainImageByClickCommand;
        public RelayCommand LoadMainImageByClickCommand
        {
            get
            {
                return loadMainImageByClickCommand ??
                    (loadMainImageByClickCommand = new RelayCommand(obj =>
                    {
                        OpenFileDialog fileDialogPicture = new OpenFileDialog();
                        fileDialogPicture.Filter = "Изображения|*.bmp;*.jpg;*.jpeg;*.gif;*.png;*.tif";
                        fileDialogPicture.FilterIndex = 1;

                        if (fileDialogPicture.ShowDialog() == true)
                        {
                            string imageFilename = fileDialogPicture.FileName;

                            loadImageOnForm(imageFilename);
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

        private RelayCommand mainImageWidthChanged;
        public RelayCommand MainImageWidthChanged
        {
            get
            {
                return mainImageWidthChanged ??
                    (mainImageWidthChanged = new RelayCommand(obj =>
                    {
                        DisplayImageHeight = SetDisplayImageSize(DisplayImageWidth, 1 / MainImageRatio);
                    }));
            }
        }

        private RelayCommand mainImageHeightChanged;
        public RelayCommand MainImageHeightChanged
        {
            get
            {
                return mainImageHeightChanged ??
                    (mainImageHeightChanged = new RelayCommand(obj =>
                    {
                        DisplayImageWidth = SetDisplayImageSize(DisplayImageHeight, MainImageRatio);
                    }));
            }
        }

        private RelayCommand fragmentImageWidthChanged;
        public RelayCommand FragmentImageWidthChanged
        {
            get
            {
                return fragmentImageWidthChanged ??
                    (fragmentImageWidthChanged = new RelayCommand(obj =>
                    {
                        if (IsSquareFragment)
                        {
                            DisplayImageFragmentHeight = DisplayImageFragmentWidth;
                        }
                    }));
            }
        }

        private RelayCommand fragmentImageHeightChanged;
        public RelayCommand FragmentImageHeightChanged
        {
            get
            {
                return fragmentImageHeightChanged ??
                    (fragmentImageHeightChanged = new RelayCommand(obj =>
                    {
                        if (IsSquareFragment)
                        {
                            DisplayImageFragmentWidth = DisplayImageFragmentHeight;
                        }
                    }));
            }
        }

        #endregion

        private double MainImageWidth = 1.0;
        private double MainImageHeight = 1.0;
        public double MainImageRatio => MainImageWidth / MainImageHeight;

        private double FragmentRatio = 1.0;
        private int FragmentCountWidth;
        private int FragmentCountHeight;
        public double FragmentWidthInPixels => MainImageWidth * DisplayImageFragmentWidth / DisplayImageWidth;
        public double FragmentHeightInPixels => MainImageHeight * DisplayImageFragmentHeight / DisplayImageHeight;

        private Image mainImage;
        private Grid fragmentsGrid;
        private SplitImage splitImage;
        private bool? gridLinesVis;

        private WriteableBitmap[,] resultImageBitmaps;
        private Color[,] resultImageColors;

        private en_ImageStates currentImageState = en_ImageStates.mainImage;

        public MainViewModel()
        {
            SettingsIsEnabled = false;
            DisplayImageFragmentWidth = 1.0;
            DisplayImageFragmentHeight = 1.0;
            DisplayImageWidth = 100.0;
            DisplayImageHeight = 100.0;
            KeepRatioOfMainImage = true;
            IsSquareFragment = true;
            SplittingProcessVisibility = Visibility.Hidden;
            gridLinesVis = null;
            SwitchGridIconFilename = "../resources/grid_off_icon_1.png";
        }

        public double SetDisplayImageSize(int newFragmentCountDimension, double fragmentDimension)
        {
            return newFragmentCountDimension * fragmentDimension;
        }

        private double SetDisplayImageSize(double otherInputSize, double mainImageRatio = 1.0)
        {
            return otherInputSize * mainImageRatio;
        }

        public int SetFragmentCount(double imageSizeInput, double fragmentSizeInput, double fragmentSizePx, double imageSizePx, double mainImageRatio = 1.0)
        {
            int fragmentCount = (int)Math.Round(imageSizeInput * mainImageRatio / fragmentSizeInput);
            int fragmentSizePxInt = (int)Math.Round(fragmentSizePx);
            while (fragmentCount * fragmentSizePxInt - imageSizePx >= fragmentSizePxInt)
            {
                fragmentCount--;
            }
            return fragmentCount;
        }

        public void SetSettingsIsEnabled(bool imageIsLoaded) => SettingsIsEnabled = imageIsLoaded;

        public void LoadMainImageByDropCommand(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (imageFilename.Contains(".bmp") || imageFilename.Contains(".jpg") ||
                imageFilename.Contains(".gif") || imageFilename.Contains(".png") ||
                imageFilename.Contains(".tif") || imageFilename.Contains(".jpeg"))
            {
                loadImageOnForm(imageFilename);
            }
        }

        private void loadImageOnForm(string imageFilename)
        {
            mainImage = CreateMainImage();
            mainImage.Source = new BitmapImage(new Uri(imageFilename));
            SetSettingsIsEnabled(true);
            MainImageWidth = (mainImage.Source as BitmapSource).PixelWidth;
            MainImageHeight = (mainImage.Source as BitmapSource).PixelHeight;
            DisplayImageWidth = 100 * DisplayImageFragmentWidth;
            DisplayImageHeight = DisplayImageWidth / MainImageRatio;

            ImageArea.mainImageContainer.Children.Clear();
            Grid.SetColumn(mainImage, 0);
            Grid.SetRow(mainImage, 0);
            ImageArea.mainImageContainer.Children.Add(mainImage);

            gridLinesVis = null;
        }

        private Image CreateMainImage()
        {
            Image mainImage = new Image();

            mainImage.Name = "mainImage";
            mainImage.Cursor = Cursors.Hand;
            mainImage.Stretch = Stretch.Uniform;

            currentImageState = en_ImageStates.mainImage;
            return mainImage;
        }

        private Grid InitGridForFragments()
        {
            Grid fragmentsGrid = new Grid();
            fragmentsGrid.Name = "fragmentsGrid";
            fragmentsGrid.Cursor = Cursors.Hand;
            fragmentsGrid.ShowGridLines = true;
            ColumnDefinition[] colDef = new ColumnDefinition[FragmentCountWidth];
            RowDefinition[] rowDef = new RowDefinition[FragmentCountHeight];

            for (int i = 0; i < FragmentCountWidth; i++)
            {
                colDef[i] = new ColumnDefinition();
                fragmentsGrid.ColumnDefinitions.Add(colDef[i]);
            }
            for (int j = 0; j < FragmentCountHeight; j++)
            {
                rowDef[j] = new RowDefinition();
                fragmentsGrid.RowDefinitions.Add(rowDef[j]);
            }

            return fragmentsGrid;
        }

        private double FindMin(double a, double b)
        {
            return a < b ? a : b;
        }
    }
}
