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
                FragmentCountWidth = SetFragmentCountDimension(value, DisplayImageFragmentWidth);
                m_DisplayImageWidth = SetDisplayImageDimension(FragmentCountWidth, DisplayImageFragmentWidth);
                OnPropertyChanged("DisplayImageWidth");

                FragmentCountHeight = SetFragmentCountDimension(value, DisplayImageFragmentHeight, 1 / MainImageRatio);
                double newDisplayHeight = SetDisplayImageDimension(FragmentCountHeight, DisplayImageFragmentHeight);
                if (m_DisplayImageHeight != newDisplayHeight)
                    m_DisplayImageHeight = newDisplayHeight;
                OnPropertyChanged("DisplayImageHeight");
            }
        }

        private double m_DisplayImageHeight;
        public double DisplayImageHeight
        {
            get { return m_DisplayImageHeight; }
            set 
            {
                FragmentCountHeight = SetFragmentCountDimension(value, DisplayImageFragmentHeight);
                m_DisplayImageHeight = SetDisplayImageDimension(FragmentCountHeight, DisplayImageFragmentHeight);
                OnPropertyChanged("DisplayImageHeight");

                FragmentCountWidth = SetFragmentCountDimension(value, DisplayImageFragmentWidth, MainImageRatio);
                double newDisplayWidth = SetDisplayImageDimension(FragmentCountWidth, DisplayImageFragmentWidth);
                if (m_DisplayImageWidth != newDisplayWidth)
                    m_DisplayImageWidth = newDisplayWidth;
                OnPropertyChanged("DisplayImageWidth");
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

                m_DisplayImageFragmentHeight = m_DisplayImageFragmentWidth;
                OnPropertyChanged("DisplayImageFragmentHeight");
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

                m_DisplayImageFragmentWidth = m_DisplayImageFragmentHeight;
                OnPropertyChanged("DisplayImageFragmentWidth");
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

                        resultImageBitmaps = new WriteableBitmap[FragmentCountWidth, FragmentCountHeight];

                        splitImage = new SplitImage(mainImage, FragmentCountWidth, FragmentCountHeight,
                            (int)FragmentWidthInPixels, (int)FragmentHeightInPixels);

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
                    Application.Current.Dispatcher.Invoke(new Action(() => { resultImageBitmaps[i, j] = new WriteableBitmap(splitImage.DoSplitImage(i, j));
                        Image fragment = new Image();
                        fragment.Source = resultImageBitmaps[i, j];

                        Grid.SetColumn(fragment, i);
                        Grid.SetRow(fragment, j);

                        fragmentsGrid.Children.Add(fragment);
                    }));

                    progressPercentage = Convert.ToInt32((double)(i * FragmentCountHeight + j) / (FragmentCountWidth * FragmentCountHeight) * 100);
                    (sender as BackgroundWorker).ReportProgress(progressPercentage);
                }
            }

            e.Result = resultImageBitmaps;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SplittingProcessValue = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ImageArea.Content = fragmentsGrid;
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
                        fileDialogPicture.Filter = "Изображения|*.bmp;*.jpg;*.gif;*.png;*.tif";
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
                        Grid? content = ImageArea.Content as Grid;
                        if (content != null)
                            content.ShowGridLines = !content.ShowGridLines;
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
        private int FragmentCountWidth;
        private int FragmentCountHeight;
        public double FragmentWidthInPixels => MainImageWidth * DisplayImageFragmentWidth / DisplayImageWidth;
        public double FragmentHeightInPixels => MainImageHeight * DisplayImageFragmentHeight / DisplayImageHeight;

        private Image mainImage;
        private Grid fragmentsGrid;
        private SplitImage splitImage;

        private WriteableBitmap[,] resultImageBitmaps;

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
            SplittingProcessVisibility = Visibility.Visible;
        }

        public double SetDisplayImageDimension(int newFragmentCountDimension, double fragmentDimension) =>
            newFragmentCountDimension * fragmentDimension;

        public int SetFragmentCountDimension(double imageDimensionInput, double fragmentDimension, double ratio = 1.0) =>
            (int)Math.Round(imageDimensionInput * ratio / fragmentDimension);

        public void SetSettingsIsEnabled(bool imageIsLoaded) => SettingsIsEnabled = imageIsLoaded;

        public void LoadMainImageByDropCommand(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (imageFilename.Contains(".bmp") || imageFilename.Contains(".jpg") ||
                imageFilename.Contains(".gif") || imageFilename.Contains(".png") ||
                imageFilename.Contains(".tif"))
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

            ImageArea.Content = mainImage;
        }

        private Image CreateMainImage()
        {
            Image mainImage = new Image();

            mainImage.Name = "mainImage";
            mainImage.Cursor = Cursors.Hand;
            mainImage.Stretch = System.Windows.Media.Stretch.Uniform;

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
    }
}
