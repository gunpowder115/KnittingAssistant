using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime;

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
                m_DisplayImageWidth = SetDisplayFirstImageDimension(value, DisplayImageFragmentWidth);
                OnPropertyChanged("DisplayImageWidth");

                double newDisplayHeight = SetDisplaySecondImageDimension(value, DisplayImageFragmentHeight, 1 / MainImageRatio);
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
                m_DisplayImageHeight = SetDisplayFirstImageDimension(value, DisplayImageFragmentHeight);
                OnPropertyChanged("DisplayImageHeight");

                double newDisplayWidth = SetDisplaySecondImageDimension(value, DisplayImageFragmentWidth, MainImageRatio);
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

        public double MainImageRatio { get; set; } = 1.0;
        public double FragmentRatio { get; set; } = 1.0;

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

        public double SetDisplayFirstImageDimension(double firstImageDimensionInput, double fragmentDimension)
        {
            int newFragmentCountDimension = (int)Math.Round(firstImageDimensionInput / fragmentDimension);
            return newFragmentCountDimension * fragmentDimension;
        }

        public double SetDisplaySecondImageDimension(double firstImageDimensionInput, double fragmentDimension, double ratio)
        {
            int newFragmentCountDimension = (int)Math.Round(firstImageDimensionInput * ratio / fragmentDimension);
            return newFragmentCountDimension * fragmentDimension;
        }

        public void SetSettingsIsEnabled(bool imageIsLoaded)
        {
            SettingsIsEnabled = imageIsLoaded;
        }
    }
}
