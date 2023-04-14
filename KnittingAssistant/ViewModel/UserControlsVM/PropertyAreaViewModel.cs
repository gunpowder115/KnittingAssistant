using KnittingAssistant.Model;
using System;
using System.ComponentModel;
using System.Windows;

namespace KnittingAssistant.ViewModel
{
    public class PropertyAreaViewModel : ViewModelBase
    {
        private int FragmentCountWidth;
        private int FragmentCountHeight;
        private ImageProcessor imageProcessor;

        public double FragmentWidthInPixels => imageProcessor.MainImageWidth * DisplayImageFragmentWidth / DisplayImageWidth;
        public double FragmentHeightInPixels => imageProcessor.MainImageHeight * DisplayImageFragmentHeight / DisplayImageHeight;

        #region Dependency Property

        private bool settingsIsEnabled;
        public bool SettingsIsEnabled
        {
            get { return settingsIsEnabled; }
            set
            {
                settingsIsEnabled = value;
                OnPropertyChanged("SettingsIsEnabled");
            }
        }

        private double displayImageWidth;
        public double DisplayImageWidth
        {
            get { return displayImageWidth; }
            set
            {
                displayImageWidth = value;
                OnPropertyChanged("DisplayImageWidth");
            }
        }

        private double displayImageHeight;
        public double DisplayImageHeight
        {
            get { return displayImageHeight; }
            set
            {
                displayImageHeight = value;
                OnPropertyChanged("DisplayImageHeight");
            }
        }

        private double displayImageFragmentWidth;
        public double DisplayImageFragmentWidth
        {
            get { return displayImageFragmentWidth; }
            set
            {
                displayImageFragmentWidth = value;
                OnPropertyChanged("DisplayImageFragmentWidth");
            }
        }

        private double displayImageFragmentHeight;
        public double DisplayImageFragmentHeight
        {
            get { return displayImageFragmentHeight; }
            set
            {
                displayImageFragmentHeight = value;
                OnPropertyChanged("DisplayImageFragmentHeight");
            }
        }

        private bool keepRatioOfMainImage;
        public bool KeepRatioOfMainImage
        {
            get { return keepRatioOfMainImage; }
            set
            {
                keepRatioOfMainImage = value;
                OnPropertyChanged("KeepRatioOfMainImage");
            }
        }

        private bool isSquareFragment;
        public bool IsSquareFragment
        {
            get { return isSquareFragment; }
            set
            {
                isSquareFragment = value;
                OnPropertyChanged("IsSquareFragment");
            }
        }

        private string splittingProcessName;
        public string SplittingProcessName
        {
            get { return splittingProcessName; }
            set
            {
                splittingProcessName = value;
                OnPropertyChanged("SplittingProcessName");
            }
        }

        private double splittingProcessValue;
        public double SplittingProcessValue
        {
            get { return splittingProcessValue; }
            set
            {
                splittingProcessValue = value;
                OnPropertyChanged("SplittingProcessValue");
            }
        }

        private Visibility splittingProcessVisibility;
        public Visibility SplittingProcessVisibility
        {
            get { return splittingProcessVisibility; }
            set
            {
                splittingProcessVisibility = value;
                OnPropertyChanged("SplittingProcessVisibility");
            }
        }

        private bool keepValue;
        public bool KeepValue
        {
            get { return keepValue; }
            set
            {
                keepValue = value;
                OnPropertyChanged("KeepValue");
            }
        }

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

                        Fragmentation widthFragmentation = SetFragmentCount(DisplayImageWidth, DisplayImageFragmentWidth, FragmentWidthInPixels, imageProcessor.MainImageWidth);
                        FragmentCountWidth = widthFragmentation.SumCount;
                        DisplayImageWidth = SetDisplayImageSize(FragmentCountWidth, DisplayImageFragmentWidth);
                        Fragmentation heightFragmentation = SetFragmentCount(DisplayImageHeight, DisplayImageFragmentHeight, FragmentHeightInPixels, imageProcessor.MainImageHeight);
                        FragmentCountHeight = heightFragmentation.SumCount;
                        DisplayImageHeight = SetDisplayImageSize(FragmentCountHeight, DisplayImageFragmentHeight);

                        imageProcessor.ImageSplitter = new ImageSplitter(imageProcessor.DisplayedImage, widthFragmentation, heightFragmentation);

                        imageProcessor.CurrentImageState = en_ImageStates.mainImageSplitting;
                        imageProcessor.CallUpdateSplittingStateNotify(isSplitting: true);

                        BackgroundWorker worker = new BackgroundWorker();
                        worker.WorkerReportsProgress = true;
                        worker.WorkerSupportsCancellation = true;
                        worker.DoWork += worker_Splitting;
                        worker.ProgressChanged += worker_SplittingProgressChanged;
                        worker.RunWorkerCompleted += worker_SplittingCompleted;
                        worker.RunWorkerAsync(imageProcessor.ImageSplitter);
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
                        DisplayImageHeight = SetDisplayImageSize(DisplayImageWidth, 1 / imageProcessor.MainImageRatio);
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
                        DisplayImageWidth = SetDisplayImageSize(DisplayImageHeight, imageProcessor.MainImageRatio);
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

        private RelayCommand fragmentProportionChanged;
        public RelayCommand FragmentProportionChanged
        {
            get
            {
                return fragmentProportionChanged ??
                    (fragmentProportionChanged = new RelayCommand(obj =>
                    {
                        if (IsSquareFragment)
                        {
                            DisplayImageFragmentHeight = DisplayImageFragmentWidth;
                        }
                    }));
            }
        }

        private RelayCommand keepRatioOfMainImageChanged;
        public RelayCommand KeepRatioOfMainImageChanged
        {
            get
            {
                return keepRatioOfMainImageChanged ??
                    (keepRatioOfMainImageChanged = new RelayCommand(obj =>
                    {
                        if (KeepRatioOfMainImage)
                        {
                            DisplayImageWidth = imageProcessor.MainImageWidth;
                            DisplayImageHeight = imageProcessor.MainImageHeight;
                            DisplayImageFragmentWidth = 1;
                            DisplayImageFragmentHeight = 1;
                            KeepValue = true;
                        }
                        else
                        {
                            KeepValue = false;
                        }
                    }));
            }
        }

        #endregion

        public PropertyAreaViewModel(ImageProcessor imageProcessor)
        {
            this.imageProcessor = imageProcessor;

            SettingsIsEnabled = false;
            DisplayImageFragmentWidth = 1.0;
            DisplayImageFragmentHeight = 1.0;
            DisplayImageWidth = 100.0;
            DisplayImageHeight = 100.0;
            KeepRatioOfMainImage = false;
            IsSquareFragment = true;
            SplittingProcessVisibility = Visibility.Hidden;
            imageProcessor.UpdateSplittingStateNotify += UpdateSplittingState;
        }

        public void UpdateForNewImage(double mainImageRatio)
        {
            SetSettingsIsEnabled(true);
            DisplayImageFragmentWidth = DisplayImageFragmentHeight = 1;
            DisplayImageWidth = 100 * DisplayImageFragmentWidth;
            DisplayImageHeight = DisplayImageWidth / mainImageRatio;
            SplittingProcessVisibility = Visibility.Hidden;
        }

        private void SetSettingsIsEnabled(bool imageIsLoaded) => SettingsIsEnabled = imageIsLoaded;

        private double SetDisplayImageSize(int newFragmentCountDimension, double fragmentDimension)
        {
            return newFragmentCountDimension * fragmentDimension;
        }

        private double SetDisplayImageSize(double otherInputSize, double mainImageRatio = 1.0)
        {
            return otherInputSize * mainImageRatio;
        }

        private Fragmentation SetFragmentCount(double imageSizeInput, double fragmentSizeInput, double fragmentSizePx, double imageSizePx, double mainImageRatio = 1.0)
        {
            Fragmentation fragmentation = new Fragmentation();
            int fragmentCount = (int)Math.Round(imageSizeInput * mainImageRatio / fragmentSizeInput);
            int fragmentSizePxInt = (int)Math.Round(fragmentSizePx);
            int deltaCount = (int)(fragmentCount * fragmentSizePxInt - imageSizePx);

            fragmentation.mainCount = fragmentCount - Math.Abs(deltaCount);
            fragmentation.secondaryCount = Math.Abs(deltaCount);
            fragmentation.mainSize = fragmentSizePxInt;
            fragmentation.secondarySize = fragmentSizePxInt - Math.Sign(deltaCount);

            return fragmentation;
        }

        private void worker_Splitting(object sender, DoWorkEventArgs e)
        {
            imageProcessor.ImageSplitter = (ImageSplitter)e.Argument;

            int progressPercentage = 0;
            for (int i = 0; i < FragmentCountWidth; i++)
            {
                for (int j = 0; j < FragmentCountHeight; j++)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        imageProcessor.ImageSplitter.SplitImage(i, j);
                    }));

                    progressPercentage = Convert.ToInt32((double)(i * FragmentCountHeight + j) / (FragmentCountWidth * FragmentCountHeight) * 100);
                    if (progressPercentage >= 100)
                        progressPercentage = 99;
                    (sender as BackgroundWorker).ReportProgress(progressPercentage);
                }
            }
        }

        private void worker_SplittingProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SplittingProcessValue = e.ProgressPercentage;
            if (SplittingProcessValue == 99)
                SplittingProcessName = "Подготовка изображения...";
        }

        private void worker_SplittingCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            imageProcessor.CallUpdateSplittingStateNotify(isSplitting: false);
            imageProcessor.ImageSplitter.DrawSplittedImage();
            imageProcessor.CallUpdateImageNotify(imageProcessor.ImageSplitter.GridBitmapImage, imageWasBroken: true);
            imageProcessor.GridLinesVis = true;
            imageProcessor.CurrentImageState = en_ImageStates.resultImageNotSaved;
            imageProcessor.CallUpdateImageSaving();

            SplittingProcessName = "Готово!";
            SplittingProcessValue = 100;

            if (imageProcessor.ImageSplitter.isAverageColor)
            {
                MessageBox.Show("Не добавлено ни одного цвета\nЦвета определены автоматически", "Внимание", MessageBoxButton.OK);
            }
        }

        private void UpdateSplittingState()
        {
            SettingsIsEnabled = !imageProcessor.IsSplitting;
        }
    }
}
