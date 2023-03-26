using System.Windows;

namespace KnittingAssistant.ViewModel
{
    public class ImageAreaViewModel : ViewModelBase
    {
        private MainImageParams mainImageParams;
        private UserControlParams userControlParams;

        private readonly PropertyAreaViewModel propertyAreaViewModel;

        #region Dependency Property



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
                        bool loadNewImage = mainImageParams.LoadMainImageByClick(userControlParams);
                        if (loadNewImage)
                            propertyAreaViewModel.UpdateForNewImage(mainImageParams.MainImageRatio);
                    }));
            }
        }

        #endregion

        public ImageAreaViewModel(MainImageParams mainImageParams,
            UserControlParams userControlParams, PropertyAreaViewModel propertyAreaViewModel)
        {
            this.mainImageParams = mainImageParams;
            this.userControlParams = userControlParams;
            this.propertyAreaViewModel = propertyAreaViewModel;
        }

        public void LoadMainImageByDropCommand(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (mainImageParams.ImageLoader.IsSupportedFormat(imageFilename))
            {
                mainImageParams.LoadImageOnForm(imageFilename, userControlParams);
                propertyAreaViewModel.UpdateForNewImage(mainImageParams.MainImageRatio);
            }
        }
    }
}
