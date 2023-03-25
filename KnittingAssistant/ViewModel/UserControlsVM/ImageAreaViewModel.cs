using KnittingAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
                        mainImageParams.LoadMainImageByClick(userControlParams);
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
