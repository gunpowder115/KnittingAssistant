using System;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using KnittingAssistant.View.userControls;

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

        #endregion

        public double MainImageWidth = 1.0;
        public double MainImageHeight = 1.0;
        public double MainImageRatio => MainImageWidth / MainImageHeight;

        public double FragmentRatio = 1.0;
        public int FragmentCountWidth;
        public int FragmentCountHeight;
        public double FragmentWidthInPixels => MainImageWidth * DisplayImageFragmentWidth / DisplayImageWidth;
        public double FragmentHeightInPixels => MainImageHeight * DisplayImageFragmentHeight / DisplayImageHeight;

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
    }
}
