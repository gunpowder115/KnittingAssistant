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

        #region Dependency Property



        #endregion

        #region Relay Commands



        #endregion

        public ImageAreaViewModel(MainImageParams mainImageParams, UserControlParams userControlParams)
        {
            this.mainImageParams = mainImageParams;
            this.userControlParams = userControlParams;
        }

        public void LoadMainImageByDropCommand(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (mainImageParams.ImageLoader.IsSupportedFormat(imageFilename))
            {
                mainImageParams.LoadImageOnForm(imageFilename, userControlParams);
            }
        }
    }
}
