using KnittingAssistant.Model;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.ViewModel
{
    public class ImageAreaViewModel : ViewModelBase
    {
        private readonly PropertyAreaViewModel propertyAreaViewModel;
        private ImageProcessor imageProcessor;

        private const string DefaultFilename = "D:/Development/KnittingAssistant/KnittingAssistant/View/resources/default_image.png";

        #region Dependency Property

        private WriteableBitmap displayedImage;
        public WriteableBitmap DisplayedImage
        {
            get { return displayedImage; }
            set
            {
                displayedImage = value;
                OnPropertyChanged("DisplayedImage");
            }
        }

        #endregion

        #region Relay Commands

        private RelayCommand loadMainImageByClickCommand;
        public RelayCommand LoadMainImageByClickCommand
        {
            get
            {
                return loadMainImageByClickCommand ??
                    (loadMainImageByClickCommand = new RelayCommand(obj =>
                    {
                        bool loadNewImage = imageProcessor.LoadMainImageByClick();
                        if (loadNewImage)
                            propertyAreaViewModel.UpdateForNewImage(imageProcessor.MainImageRatio);
                    }));
            }
        }

        #endregion

        public ImageAreaViewModel(PropertyAreaViewModel propertyAreaViewModel, ImageProcessor imageProcessor)
        {
            this.propertyAreaViewModel = propertyAreaViewModel;
            this.imageProcessor = imageProcessor;
            imageProcessor.UpdateImageNotify += UpdateImage;

            DisplayedImage = imageProcessor.UpdateMainImage(DefaultFilename, en_ImageStates.emptyImage);
        }

        public void LoadMainImageByDropCommand(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (imageProcessor.ImageLoader.IsSupportedFormat(imageFilename))
            {
                imageProcessor.LoadImageOnForm(imageFilename);
                propertyAreaViewModel.UpdateForNewImage(imageProcessor.MainImageRatio);
            }
        }

        private void UpdateImage(WriteableBitmap wbImage)
        {
            DisplayedImage = wbImage;
        }
    }
}
