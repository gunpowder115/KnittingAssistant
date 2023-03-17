using KnittingAssistant.Model;
using System.Windows.Controls;

namespace KnittingAssistant.ViewModel
{
    public class MainImageParams
    {
        public double MainImageWidth { get; set; }
        public double MainImageHeight { get; set; }
        public double MainImageRatio => MainImageWidth / MainImageHeight;
        public Image MainImage { get; set; }
        public bool? GridLinesVis { get; set; }
        public en_ImageStates CurrentImageState { get; set; }
        public ImageLoader ImageLoader { get; set; }
        public ImageSaver ImageSaver { get; set; }
        public  ImageSplitter ImageSplitter { get; set; }

        public MainImageParams(double mainImageWidth, double mainImageHeight)
        {
            this.MainImageWidth = mainImageWidth;
            this.MainImageHeight = mainImageHeight;
            GridLinesVis = null;
            CurrentImageState = en_ImageStates.emptyImage;
            ImageLoader = new ImageLoader();
            ImageSaver = new ImageSaver();
        }

        public MainImageParams() : this(1.0, 1.0) { }
    }
}
