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

    public class MainViewModel : ViewModelBase
    {
        #region Dependency Properties

        #endregion

        #region Relay Commands

        #endregion

        private double MainImageWidth = 1.0;
        private double MainImageHeight = 1.0;
        public double MainImageRatio => MainImageWidth / MainImageHeight;

        private double FragmentRatio = 1.0;

        private Image mainImage;
        private ImageSplitter splitImage;
        private bool? gridLinesVis;

        private WriteableBitmap[,] resultImageBitmaps;
        private Color[,] resultImageColors;

        private en_ImageStates currentImageState;
        private ImageLoader imageLoader;
        private ImageSaver imageSaver;

        public MainViewModel()
        {
            gridLinesVis = null;
            currentImageState = en_ImageStates.emptyImage;
            imageSaver = new ImageSaver();
        }
    }
}
