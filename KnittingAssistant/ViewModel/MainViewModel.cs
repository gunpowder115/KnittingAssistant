using KnittingAssistant.Model;

namespace KnittingAssistant.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Dependency Properties

        #endregion

        #region Relay Commands

        #endregion

        private double FragmentRatio = 1.0;
        private ImageProcessor imageProcessor;

        public PropertyAreaViewModel PropertyAreaViewModel { get; set; }
        public ImageAreaViewModel ImageAreaViewModel { get; set; }
        public ToolbarAreaViewModel ToolbarAreaViewModel { get; set; }

        public MainViewModel()
        {
            imageProcessor = new ImageProcessor();
            PropertyAreaViewModel = new PropertyAreaViewModel(imageProcessor);
            ImageAreaViewModel = new ImageAreaViewModel(PropertyAreaViewModel, imageProcessor);
            ToolbarAreaViewModel = new ToolbarAreaViewModel(PropertyAreaViewModel, imageProcessor);
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
