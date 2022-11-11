using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        #endregion

        public double MainImageRatio { get; set; }
        public double ImageFragmentRatio { get; set; }

        private double lastDisplayImageWidth, lastDisplayImageHeight;
        private double lastDisplayImageFragmentWidth, lastDisplayImageFragmentHeight;

        public MainViewModel()
        {
            SettingsIsEnabled = false;
            DisplayImageWidth = lastDisplayImageWidth = 100.0;
            DisplayImageHeight = lastDisplayImageHeight = 100.0;
        }

        public void SetSettingsIsEnabled(bool imageIsLoaded)
        {
            SettingsIsEnabled = imageIsLoaded;
        }

        public void SetDisplayImageSize(double displayImageWidth, double displayImageHeight)
        {
            bool widthChanged = displayImageWidth != lastDisplayImageWidth;
            bool heightChanged = displayImageHeight != lastDisplayImageHeight;

            if (widthChanged && heightChanged)
                heightChanged = false;

            if (widthChanged)
            {
                DisplayImageHeight = lastDisplayImageHeight = displayImageWidth / MainImageRatio;
            }
            if (heightChanged)
            {
                DisplayImageWidth = lastDisplayImageWidth = displayImageHeight * MainImageRatio;
            }
        }

        public void SetDisplayImageFragmentSize(double displayImageFragmentWidth, double displayImageFragmentHeight)
        {
            bool widthChanged = displayImageFragmentWidth != lastDisplayImageFragmentWidth;
            bool heightChanged = displayImageFragmentHeight != lastDisplayImageFragmentHeight;

            if (widthChanged && heightChanged)
                heightChanged = false;

            if (widthChanged)
            {
                DisplayImageFragmentHeight = lastDisplayImageFragmentHeight = displayImageFragmentWidth / ImageFragmentRatio;
            }
            if (heightChanged)
            {
                DisplayImageFragmentWidth = lastDisplayImageFragmentWidth = displayImageFragmentHeight * ImageFragmentRatio;
            }
        }
    }
}
