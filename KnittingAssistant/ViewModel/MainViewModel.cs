using System;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using KnittingAssistant.View.userControls;
using KnittingAssistant.Model;
using System.Windows.Controls;
using KnittingAssistant.View;

namespace KnittingAssistant.ViewModel
{
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

        private UserControl m_PropertyArea;
        public UserControl PropertyArea
        {
            get { return m_PropertyArea; }
            set
            {
                m_PropertyArea = value;
                OnPropertyChanged("PropertyArea");
            }
        }

        private UserControl m_ImageArea;
        public UserControl ImageArea
        {
            get { return m_ImageArea; }
            set
            {
                m_ImageArea = value;
                OnPropertyChanged("ImageArea");
            }
        }

        private UserControl m_ToolbarArea;
        public UserControl ToolbarArea
        {
            get { return m_ToolbarArea; }
            set
            {
                m_ToolbarArea = value;
                OnPropertyChanged("ToolbarArea");
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
                        splitImage = new SplitImage(mainImage, FragmentCountWidth, FragmentCountHeight,
                            (int)FragmentWidthInPixels, (int)FragmentHeightInPixels);
                        resultImageFragments = splitImage.imageFragments;

                        Grid fragments = CreateGridForFragments();
                        ImageArea.Content = fragments;
                    }));
            }
        }

        #endregion

        public double MainImageWidth = 1.0;
        public double MainImageHeight = 1.0;
        public double MainImageRatio => MainImageWidth / MainImageHeight;

        public double FragmentRatio = 1.0;
        public int FragmentCountWidth;
        public int FragmentCountHeight;
        public double FragmentWidthInPixels => MainImageWidth * DisplayImageFragmentWidth / DisplayImageWidth;
        public double FragmentHeightInPixels => MainImageHeight * DisplayImageFragmentHeight / DisplayImageHeight;

        public Image mainImage;
        private SplitImage splitImage;
        private ImageFragment[,] resultImageFragments;

        public MainViewModel()
        {
            SettingsIsEnabled = false;
            DisplayImageFragmentWidth = 1.0;
            DisplayImageFragmentHeight = 1.0;
            DisplayImageWidth = 100.0;
            DisplayImageHeight = 100.0;

            KeepRatioOfMainImage = true;
            IsSquareFragment = true;
        }

        public double SetDisplayImageDimension(int newFragmentCountDimension, double fragmentDimension)
        {
            return newFragmentCountDimension * fragmentDimension;
        }

        public int SetFragmentCountDimension(double imageDimensionInput, double fragmentDimension, double ratio = 1.0)
        {
            return (int)Math.Round(imageDimensionInput * ratio / fragmentDimension);
        }

        public void SetSettingsIsEnabled(bool imageIsLoaded)
        {
            SettingsIsEnabled = imageIsLoaded;
        }

        public Grid CreateGridForFragments()
        {
            Grid fragmentsGrid = new Grid();
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

            for (int i = 0; i < FragmentCountWidth; i++)
            {
                for (int j = 0; j < FragmentCountHeight; j++)
                {
                    Image fragment = new Image();
                    fragment.Source = resultImageFragments[i, j].GetResultFragment();

                    Grid.SetColumn(fragment, i);
                    Grid.SetRow(fragment, j);

                    fragmentsGrid.Children.Add(fragment);
                }
            }

            return fragmentsGrid;
        }
    }
}
