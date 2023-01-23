using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace KnittingAssistant.Model
{
    public class SplitImage
    {
        public ImageFragment[,] imageFragments { get; }
        private BitmapImage mainBitmapImage;
        public WriteableBitmap SplittedBitmapImage { get; }
        public WriteableBitmap GridBitmapImage { get; }
        private int fragmentWidthInPixels, fragmentHeightInPixels;
        private int fragmentCountWidth, fragmentCountHeight;

        public SplitImage(Image mainImage, int fragmentCountWidth, int fragmentCountHeight, int fragmentWidthInPixels, int fragmentHeightInPixels)
        {
            this.fragmentWidthInPixels = fragmentWidthInPixels;
            this.fragmentHeightInPixels = fragmentHeightInPixels;
            this.fragmentCountWidth = fragmentCountWidth;
            this.fragmentCountHeight = fragmentCountHeight;

            imageFragments = new ImageFragment[fragmentCountWidth, fragmentCountHeight]; //массив фрагментов изображения

            mainBitmapImage = (BitmapImage)mainImage.Source; //главный bitmap

            SplittedBitmapImage = new WriteableBitmap(fragmentWidthInPixels * fragmentCountWidth, fragmentHeightInPixels * fragmentCountHeight,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);
            GridBitmapImage = new WriteableBitmap(fragmentWidthInPixels * fragmentCountWidth, fragmentHeightInPixels * fragmentCountHeight,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);
        }

        private void DrawFragmentOnSplittedImage(Color fragmentColor, Color gridColor, int currentWidthFragment, int currentHeightFragment, int width, int height)
        {
            int stride = 4 * width; //4 is bytes per pixel
            byte[,] colorData = new byte[height * width, 4];
            byte[,] gridColorData = new byte[height * width, 4];
            Int32Rect rect = new Int32Rect(currentWidthFragment * width, 
                currentHeightFragment * height, width, height);

            for (int i = 0; i < height * width; i++)
            {
                if ((i >= 0 && i < width) || (i % width == 0))
                {
                    gridColorData[i, 0] = gridColor.B;
                    gridColorData[i, 1] = gridColor.G;
                    gridColorData[i, 2] = gridColor.R;
                }
                else
                {
                    gridColorData[i, 0] = fragmentColor.B;
                    gridColorData[i, 1] = fragmentColor.G;
                    gridColorData[i, 2] = fragmentColor.R;
                }
                gridColorData[i, 3] = 0;

                colorData[i, 0] = fragmentColor.B;
                colorData[i, 1] = fragmentColor.G;
                colorData[i, 2] = fragmentColor.R;
                colorData[i, 3] = 0;
            }

            SplittedBitmapImage.WritePixels(rect, colorData, stride, 0);
            GridBitmapImage.WritePixels(rect, gridColorData, stride, 0);
        }

        public Color DoSplitImage(int currentWidthFragment, int currentHeightFragment)
        {
            bool isRightEdge = (currentWidthFragment + 1) * fragmentWidthInPixels > mainBitmapImage.PixelWidth;
            bool isBottomEdge = (currentHeightFragment + 1) * fragmentHeightInPixels > mainBitmapImage.PixelHeight;
            bool isRightFragment = currentWidthFragment == fragmentCountWidth - 1 && isRightEdge;
            bool isBottomFragment = currentHeightFragment == fragmentCountHeight - 1 && isBottomEdge;
            int fragmentPixelWidth, fragmentPixelHeight;

            if (isRightFragment && isBottomFragment)
            {
                fragmentPixelWidth = (currentWidthFragment + 1) * fragmentWidthInPixels - mainBitmapImage.PixelWidth;
                fragmentPixelHeight = (currentHeightFragment + 1) * fragmentHeightInPixels - mainBitmapImage.PixelHeight;
            }
            else if (isRightFragment)
            {
                fragmentPixelWidth = (currentWidthFragment + 1) * fragmentWidthInPixels - mainBitmapImage.PixelWidth;
                fragmentPixelHeight = fragmentHeightInPixels;
            }
            else if (isBottomFragment)
            {
                fragmentPixelWidth = fragmentWidthInPixels;
                fragmentPixelHeight = (currentHeightFragment + 1) * fragmentHeightInPixels - mainBitmapImage.PixelHeight;
            }
            else
            {
                fragmentPixelWidth = fragmentWidthInPixels;
                fragmentPixelHeight = fragmentHeightInPixels;
            }


            //временный bitmap для фрагмента
            WriteableBitmap tempBitmapFragment = new WriteableBitmap(fragmentPixelWidth, fragmentPixelHeight,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);

            //шаг и размер массива
            int stride = (int)tempBitmapFragment.PixelWidth * tempBitmapFragment.Format.BitsPerPixel / 8;
            byte[] fragmentPixels = new byte[tempBitmapFragment.PixelHeight * stride];

            Int32Rect pixelsRect = new Int32Rect(currentWidthFragment * tempBitmapFragment.PixelWidth, currentHeightFragment * tempBitmapFragment.PixelHeight,
                tempBitmapFragment.PixelWidth, tempBitmapFragment.PixelHeight);

            //копировать пиксели прямоугольника-фрагмента в массив byte[]
            mainBitmapImage.CopyPixels(pixelsRect, fragmentPixels, stride, 0);

            //записать пиксели фрагмента в временный bitmap для фрагмента
            tempBitmapFragment.WritePixels(new Int32Rect(0, 0, tempBitmapFragment.PixelWidth, tempBitmapFragment.PixelHeight), fragmentPixels, stride, 0);

            //создать новый ImageFragment
            imageFragments[currentWidthFragment, currentHeightFragment] = new ImageFragment(tempBitmapFragment, fragmentWidthInPixels, fragmentHeightInPixels);

            Color resultColor = imageFragments[currentWidthFragment, currentHeightFragment].GetResultFragmentColor();
            Color gridColor = Color.FromArgb(0, 0, 0, 0);

            DrawFragmentOnSplittedImage(resultColor, gridColor, currentWidthFragment, currentHeightFragment, fragmentPixelWidth, fragmentPixelHeight);

            return resultColor;
        }
    }
}
