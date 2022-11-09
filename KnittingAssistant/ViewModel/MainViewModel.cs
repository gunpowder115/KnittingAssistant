using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KnittingAssistant.ViewModel
{
    public class MainViewModel : DependencyObject
    {
        #region Dependency Properties

        public bool SettingsIsEnabled
        {
            get { return (bool)GetValue(SettingsIsEnabledProperty); }
            set { SetValue(SettingsIsEnabledProperty, value); }
        }
        private static readonly DependencyProperty SettingsIsEnabledProperty =
            DependencyProperty.Register("SettingsIsEnabled", typeof(bool), typeof(MainViewModel));

        public double DisplayImageWidth
        {
            get { return (double)GetValue(DisplayImageWidthProperty); }
            set { SetValue(DisplayImageWidthProperty, value); }
        }
        private static readonly DependencyProperty DisplayImageWidthProperty =
            DependencyProperty.Register("DisplayImageWidth", typeof(double), typeof(MainViewModel));

        public double DisplayImageHeight
        {
            get { return (double)GetValue(DisplayImageHeightProperty); }
            set { SetValue(DisplayImageHeightProperty, value); }
        }
        private static readonly DependencyProperty DisplayImageHeightProperty =
            DependencyProperty.Register("DisplayImageHeight", typeof(double), typeof(MainViewModel));

        public double DisplayImageFragmentWidth
        {
            get { return (double)GetValue(DisplayImageFragmentWidthProperty); }
            set { SetValue(DisplayImageFragmentWidthProperty, value); }
        }
        private static readonly DependencyProperty DisplayImageFragmentWidthProperty =
            DependencyProperty.Register("DisplayImageFragmentWidth", typeof(double), typeof(MainViewModel));

        public double DisplayImageFragmentHeight
        {
            get { return (double)GetValue(DisplayImageFragmentHeightProperty); }
            set { SetValue(DisplayImageFragmentHeightProperty, value); }
        }
        private static readonly DependencyProperty DisplayImageFragmentHeightProperty =
            DependencyProperty.Register("DisplayImageFragmentHeight", typeof(double), typeof(MainViewModel));

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
