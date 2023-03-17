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
                LoadImageOnForm(imageFilename);
            }
        }

        private void LoadImageOnForm(string imageFilename)
        {
            bool loadNewImage = true;
            if (mainImageParams.CurrentImageState == en_ImageStates.resultImageNotSaved)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Загрузить новое изображение?\nТекущее изображение не было сохранено!", "Внимание", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    loadNewImage = false;
                }
            }

            if (loadNewImage)
            {
                mainImageParams.MainImage = CreateMainImage();
                mainImageParams.MainImage.Source = new BitmapImage(new Uri(imageFilename));
                mainImageParams.MainImageWidth = (mainImageParams.MainImage.Source as BitmapSource).PixelWidth;
                mainImageParams.MainImageHeight = (mainImageParams.MainImage.Source as BitmapSource).PixelHeight;

                userControlParams.ImageArea.mainImageContainer.Children.Clear();
                Grid.SetColumn(mainImageParams.MainImage, 0);
                Grid.SetRow(mainImageParams.MainImage, 0);
                userControlParams.ImageArea.mainImageContainer.Children.Add(mainImageParams.MainImage);

                mainImageParams.GridLinesVis = null;
            }
        }

        private Image CreateMainImage()
        {
            Image mainImage = new Image();
            mainImage.Name = "mainImage";
            mainImage.Cursor = Cursors.Hand;
            mainImage.Stretch = Stretch.Uniform;
            mainImageParams.CurrentImageState = en_ImageStates.mainImageLoaded;
            return mainImage;
        }


    }
}
