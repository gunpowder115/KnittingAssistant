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
        public static readonly DependencyProperty SettingsIsEnabledProperty =
            DependencyProperty.Register("SettingsIsEnabled", typeof(bool), typeof(MainViewModel));

        public double MainImageWidth
        {
            get { return (double)GetValue(MainImageWidthProperty); }
            set { SetValue(MainImageWidthProperty, value); }
        }
        public static readonly DependencyProperty MainImageWidthProperty =
            DependencyProperty.Register("MainImageWidth", typeof(double), typeof(MainViewModel));

        public double MainImageHeight
        {
            get { return (double)GetValue(MainImageHeightProperty); }
            set { SetValue(MainImageHeightProperty, value); }
        }
        public static readonly DependencyProperty MainImageHeightProperty =
            DependencyProperty.Register("MainImageHeight", typeof(double), typeof(MainViewModel));

        #endregion

        public void SetSettingsIsEnabled(bool imageIsLoaded)
        {
            SettingsIsEnabled = imageIsLoaded;
        }

        public void SetMainImageWidth()
        {

        }

        public void SetMainImageHeight()
        {

        }
    }
}
