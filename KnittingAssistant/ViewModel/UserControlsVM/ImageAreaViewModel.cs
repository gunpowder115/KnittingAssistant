using KnittingAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnittingAssistant.ViewModel
{
    public class ImageAreaViewModel : ViewModelBase
    {
        private ImageLoader imageLoader;

        #region Dependency Property



        #endregion

        #region Relay Commands



        #endregion

        public ImageAreaViewModel()
        {
            imageLoader = new ImageLoader();

        }

        public void LoadMainImageByDropCommand(object sender, DragEventArgs e)
        {
            string imageFilename = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (imageLoader.IsSupportedFormat(imageFilename))
            {
                LoadImageOnForm(imageFilename);
            }
        }

        private void LoadImageOnForm(string imageFilename)
        {
            bool loadNewImage = true;
            if (currentImageState == en_ImageStates.resultImageNotSaved)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Загрузить новое изображение?\nТекущее изображение не было сохранено!", "Внимание", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.No)
                {
                    loadNewImage = false;
                }
            }

            if (loadNewImage)
            {
                mainImage = CreateMainImage();
                mainImage.Source = new BitmapImage(new Uri(imageFilename));
                MainImageWidth = (mainImage.Source as BitmapSource).PixelWidth;
                MainImageHeight = (mainImage.Source as BitmapSource).PixelHeight;

                ImageArea.mainImageContainer.Children.Clear();
                Grid.SetColumn(mainImage, 0);
                Grid.SetRow(mainImage, 0);
                ImageArea.mainImageContainer.Children.Add(mainImage);

                gridLinesVis = null;
            }
        }

        private Image CreateMainImage()
        {
            Image mainImage = new Image();
            mainImage.Name = "mainImage";
            mainImage.Cursor = Cursors.Hand;
            mainImage.Stretch = Stretch.Uniform;
            currentImageState = en_ImageStates.mainImageLoaded;
            return mainImage;
        }


    }
}
