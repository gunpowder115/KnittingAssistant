﻿using System;
using System.ComponentModel;
using System.Windows;

namespace KnittingAssistant.ViewModel
{
    public class PropertyAreaViewModel : ViewModelBase
    {
        private int FragmentCountWidth;
        private int FragmentCountHeight;
        public double FragmentWidthInPixels => MainImageWidth * DisplayImageFragmentWidth / DisplayImageWidth;
        public double FragmentHeightInPixels => MainImageHeight * DisplayImageFragmentHeight / DisplayImageHeight;

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

                        Fragmentation widthFragmentation = SetFragmentCount(DisplayImageWidth, DisplayImageFragmentWidth, FragmentWidthInPixels, MainImageWidth);
                        FragmentCountWidth = widthFragmentation.SumCount;
                        DisplayImageWidth = SetDisplayImageSize(FragmentCountWidth, DisplayImageFragmentWidth);
                        Fragmentation heightFragmentation = SetFragmentCount(DisplayImageHeight, DisplayImageFragmentHeight, FragmentHeightInPixels, MainImageHeight);
                        FragmentCountHeight = heightFragmentation.SumCount;
                        DisplayImageHeight = SetDisplayImageSize(FragmentCountHeight, DisplayImageFragmentHeight);

                        resultImageBitmaps = new WriteableBitmap[FragmentCountWidth, FragmentCountHeight];
                        resultImageColors = new Color[FragmentCountWidth, FragmentCountHeight];

                        splitImage = new ImageSplitter(mainImage, widthFragmentation, heightFragmentation);

                        fragmentsGrid = InitGridForFragments();

                        currentImageState = en_ImageStates.mainImageSplitting;

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
            splitImage = (ImageSplitter)e.Argument;

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
            currentImageState = en_ImageStates.resultImageNotSaved;

            SplittingProcessName = "Готово!";
            SplittingProcessValue = 100;

            if (splitImage.isAverageColor)
            {
                MessageBox.Show("Не добавлено ни одного цвета\nЦвета определены автоматически", "Внимание", MessageBoxButton.OK);
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
                            DisplayImageWidth = MainImageWidth;
                            DisplayImageHeight = MainImageHeight;
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

        public PropertyAreaViewModel()
        {
            SettingsIsEnabled = false;
            DisplayImageFragmentWidth = 1.0;
            DisplayImageFragmentHeight = 1.0;
            DisplayImageWidth = 100.0;
            DisplayImageHeight = 100.0;
            KeepRatioOfMainImage = false;
            IsSquareFragment = true;
            SplittingProcessVisibility = Visibility.Hidden;

        }

        public void UpdateForNewImage()
        {
            SetSettingsIsEnabled(true);
            DisplayImageWidth = 100 * DisplayImageFragmentWidth;
            DisplayImageHeight = DisplayImageWidth / MainImageRatio;
            SplittingProcessVisibility = Visibility.Hidden;
        }

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
            int deltaCount = (int)(fragmentCount * fragmentSizePxInt - imageSizePx/* - fragmentSizePxInt*/);

            fragmentation.mainCount = fragmentCount - Math.Abs(deltaCount);
            fragmentation.secondaryCount = Math.Abs(deltaCount);
            fragmentation.mainSize = fragmentSizePxInt;
            fragmentation.secondarySize = fragmentSizePxInt - Math.Sign(deltaCount);

            return fragmentation;
        }

        public void SetSettingsIsEnabled(bool imageIsLoaded) => SettingsIsEnabled = imageIsLoaded;

    }
}