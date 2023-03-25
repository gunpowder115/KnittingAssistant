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
using System.IO;

namespace KnittingAssistant.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Dependency Properties

        #endregion

        #region Relay Commands

        #endregion

        private double FragmentRatio = 1.0;

        private MainImageParams mainImageParams;
        public UserControlParams UserControlParams { get; set; }
        public PropertyAreaViewModel PropertyAreaViewModel { get; set; }
        public ImageAreaViewModel ImageAreaViewModel { get; set; }
        public ToolbarAreaViewModel ToolbarAreaViewModel { get; set; }

        public MainViewModel()
        {
            mainImageParams = new MainImageParams();
            UserControlParams = new UserControlParams();
            PropertyAreaViewModel = new PropertyAreaViewModel(mainImageParams, UserControlParams);
            ImageAreaViewModel = new ImageAreaViewModel(mainImageParams, UserControlParams);
            ToolbarAreaViewModel = new ToolbarAreaViewModel(mainImageParams, UserControlParams);
        }
    }

    public enum en_ImageStates
    {
        emptyImage,
        mainImageLoaded,
        mainImageSplitting,
        resultImageNotSaved,
        resultImageSaved
    }

    public struct Fragmentation
    {
        public int mainCount;
        public int mainSize;
        public int secondaryCount;
        public int secondarySize;
        public int SumCount => mainCount + secondaryCount;
    }
}
