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

        public SplitImage(Image mainImage, int fragmentCountWidth, int fragmentCountHeight, int fragmentWidthInPixels, int fragmentHeightInPixels)
        {
            this.fragmentWidthInPixels = fragmentWidthInPixels;
            this.fragmentHeightInPixels = fragmentHeightInPixels;

            imageFragments = new ImageFragment[fragmentCountWidth, fragmentCountHeight]; //массив фрагментов изображения

            mainBitmapImage = (BitmapImage)mainImage.Source; //главный bitmap

            SplittedBitmapImage = new WriteableBitmap(fragmentWidthInPixels * fragmentCountWidth, fragmentHeightInPixels * fragmentCountHeight,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);
            GridBitmapImage = new WriteableBitmap(fragmentWidthInPixels * fragmentCountWidth, fragmentHeightInPixels * fragmentCountHeight,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);
        }

        private void DrawFragmentOnSplittedImage(Color fragmentColor, Color gridColor, int currentWidthFragment, int currentHeightFragment)
        {
            int stride = 4 * fragmentWidthInPixels; //4 is bytes per pixel
            byte[] colorData = new byte[fragmentHeightInPixels * stride];
            byte[] gridColorData = new byte[fragmentHeightInPixels * stride];
            Int32Rect rect = new Int32Rect(currentWidthFragment * fragmentWidthInPixels, 
                currentHeightFragment * fragmentHeightInPixels, fragmentWidthInPixels, fragmentHeightInPixels);
            for (int i = 0; i < colorData.Length - 3; i += 4)
            {
                if ((i >= 0 && i < fragmentWidthInPixels * 4) || (i % fragmentWidthInPixels * 4 == 0))
                {
                    gridColorData[i] = gridColor.B;
                    gridColorData[i + 1] = gridColor.G;
                    gridColorData[i + 2] = gridColor.R;
                }
                else
                {
                    gridColorData[i] = fragmentColor.B;
                    gridColorData[i + 1] = fragmentColor.G;
                    gridColorData[i + 2] = fragmentColor.R;
                }
                gridColorData[i + 3] = 0;

                colorData[i] = fragmentColor.B;
                colorData[i + 1] = fragmentColor.G;
                colorData[i + 2] = fragmentColor.R;
                colorData[i + 3] = 0;
            }

            SplittedBitmapImage.WritePixels(rect, colorData, stride, 0);
            GridBitmapImage.WritePixels(rect, gridColorData, stride, 0);
        }

        public Color DoSplitImage(int currentWidthFragment, int currentHeightFragment)
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

            Color resultColor = imageFragments[currentWidthFragment, currentHeightFragment].GetResultFragmentColor();
            Color gridColor = Color.FromArgb(0, 0, 0, 0);

            DrawFragmentOnSplittedImage(resultColor, gridColor, currentWidthFragment, currentHeightFragment);

            return resultColor;
        }
    }
}
