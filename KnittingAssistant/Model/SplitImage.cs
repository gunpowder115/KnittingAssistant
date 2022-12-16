using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System;

namespace KnittingAssistant.Model
{
    public class SplitImage
    {
        public ImageFragment[,] imageFragments { get; }
        private Image mainImage;
        private BitmapImage mainBitmapImage;
        private int fragmentWidthInPixels, fragmentHeightInPixels;
        private delegate void copyPixels(Int32Rect sourceRect, Array pixels, int stride, int offset);

        public SplitImage(Image mainImage, int fragmentCountWidth, int fragmentCountHeight,
            int fragmentWidthInPixels, int fragmentHeightInPixels)
        {
            this.mainImage = mainImage;
            this.fragmentWidthInPixels = fragmentWidthInPixels;
            this.fragmentHeightInPixels = fragmentHeightInPixels;

            imageFragments = new ImageFragment[fragmentCountWidth, fragmentCountHeight]; //массив фрагментов изображения

            mainBitmapImage = (BitmapImage)mainImage.Source; //главный bitmap
        }

        public WriteableBitmap DoSplitImage(int currentWidthFragment, int currentHeightFragment)
        {
            //временный bitmap для фрагмента
            WriteableBitmap tempBitmapFragment = new WriteableBitmap(fragmentWidthInPixels, fragmentHeightInPixels,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY,
                mainBitmapImage.Format, mainBitmapImage.Palette);

            //шаг и размер массива
            int stride = (int)tempBitmapFragment.PixelWidth * tempBitmapFragment.Format.BitsPerPixel / 8;
            byte[] fragmentPixels = new byte[tempBitmapFragment.PixelHeight * stride];

            Int32Rect pixelsRect = new Int32Rect(currentWidthFragment * fragmentWidthInPixels, currentHeightFragment * fragmentHeightInPixels,
                fragmentWidthInPixels, fragmentHeightInPixels);

            //копировать пиксели прямоугольника-фрагмента в массив byte[]
            mainBitmapImage.CopyPixels(pixelsRect, fragmentPixels, stride, 0);

            //записать пиксели фрагмента в временный bitmap для фрагмента
            tempBitmapFragment.WritePixels(new Int32Rect(0, 0, fragmentWidthInPixels, fragmentHeightInPixels), fragmentPixels, stride, 0);

            //создать новый ImageFragment
            imageFragments[currentWidthFragment, currentHeightFragment] = new ImageFragment(tempBitmapFragment, fragmentWidthInPixels, fragmentHeightInPixels);

            return imageFragments[currentWidthFragment, currentHeightFragment].GetResultFragment();
        }
    }
}
