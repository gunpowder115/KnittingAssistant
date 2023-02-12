using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using KnittingAssistant.ViewModel;

namespace KnittingAssistant.Model
{
    public class ImageSplitter
    {
        public ImageFragment[,] imageFragments { get; }
        private BitmapImage mainBitmapImage;
        public WriteableBitmap SplittedBitmapImage { get; }
        public WriteableBitmap GridBitmapImage { get; }
        private int fragmentWidthMain, fragmentHeightMain;
        private int fragmentWidthSecondary, fragmentHeightSecondary;
        private int fragmentCountWidthMain, fragmentCountHeightMain;
        private int fragmentCountWidthSecondary, fragmentCountHeightSecondary;
        public bool isAverageColor => ImageFragment.isAverageColor;

        public ImageSplitter(Image mainImage, Fragmentation widthFragmentation, Fragmentation heightFragmentation)
        {
            this.fragmentCountWidthMain = widthFragmentation.mainCount;
            this.fragmentCountHeightMain = heightFragmentation.mainCount;
            this.fragmentCountWidthSecondary = widthFragmentation.secondaryCount;
            this.fragmentCountHeightSecondary = heightFragmentation.secondaryCount;
            this.fragmentWidthMain = widthFragmentation.mainSize;
            this.fragmentHeightMain = heightFragmentation.mainSize;
            this.fragmentWidthSecondary = widthFragmentation.secondarySize;
            this.fragmentHeightSecondary = heightFragmentation.secondarySize;

            imageFragments = new ImageFragment[fragmentCountWidthMain + fragmentCountWidthSecondary, 
                fragmentCountHeightMain + fragmentCountHeightSecondary]; //массив фрагментов изображения

            mainBitmapImage = (BitmapImage)mainImage.Source; //главный bitmap

            int bitmapWidth = fragmentCountWidthMain * fragmentWidthMain + fragmentCountWidthSecondary * fragmentWidthSecondary;
            int bitmapHeight = fragmentCountHeightMain * fragmentHeightMain + fragmentCountHeightSecondary * fragmentHeightSecondary;
            SplittedBitmapImage = new WriteableBitmap((fragmentCountWidthMain + fragmentCountWidthSecondary) * fragmentWidthMain, 
                (fragmentCountHeightMain + fragmentCountHeightSecondary) * fragmentHeightMain,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);
            GridBitmapImage = new WriteableBitmap((fragmentCountWidthMain + fragmentCountWidthSecondary) * fragmentWidthMain,
                (fragmentCountHeightMain + fragmentCountHeightSecondary) * fragmentHeightMain,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);
        }

        private void DrawFragmentOnSplittedImage(Color fragmentColor, Color gridColor, int currentWidthFragment, int currentHeightFragment, int width, int height)
        {
            int stride = 4 * width; //4 is bytes per pixel
            byte[,] colorData = new byte[height * width, 4];
            byte[,] gridColorData = new byte[height * width, 4];
            Int32Rect rect = new Int32Rect(currentWidthFragment < fragmentCountWidthMain ? currentWidthFragment * width :
                fragmentCountWidthMain * fragmentWidthMain + (currentWidthFragment - fragmentCountWidthMain) * width,
                currentHeightFragment < fragmentCountHeightMain ? currentHeightFragment * height :
                fragmentCountHeightMain * fragmentHeightMain + (currentHeightFragment - fragmentCountHeightMain) * height,
                width, height);

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
                gridColorData[i, 3] = 255;

                colorData[i, 0] = fragmentColor.B;
                colorData[i, 1] = fragmentColor.G;
                colorData[i, 2] = fragmentColor.R;
                colorData[i, 3] = 255;
            }

            SplittedBitmapImage.WritePixels(rect, colorData, stride, 0);
            GridBitmapImage.WritePixels(rect, gridColorData, stride, 0);
        }

        public Color DoSplitImage(int currentWidthFragment, int currentHeightFragment)
        {
            int fragmentPixelWidth = currentWidthFragment < fragmentCountWidthMain ? fragmentWidthMain : fragmentWidthSecondary;
            int fragmentPixelHeight = currentHeightFragment < fragmentCountHeightMain ? fragmentHeightMain : fragmentHeightSecondary;

            //временный bitmap для фрагмента
            WriteableBitmap tempBitmapFragment = new WriteableBitmap(fragmentPixelWidth, fragmentPixelHeight,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);

            //шаг и размер массива
            int stride = (int)tempBitmapFragment.PixelWidth * tempBitmapFragment.Format.BitsPerPixel / 8;
            byte[] fragmentPixels = new byte[tempBitmapFragment.PixelHeight * stride];

            Int32Rect pixelsRect = new Int32Rect(currentWidthFragment < fragmentCountWidthMain ? currentWidthFragment * fragmentPixelWidth : 
                fragmentCountWidthMain * fragmentWidthMain + (currentWidthFragment - fragmentCountWidthMain) * fragmentPixelWidth,
                currentHeightFragment < fragmentCountHeightMain ? currentHeightFragment * fragmentPixelHeight :
                fragmentCountHeightMain * fragmentHeightMain + (currentHeightFragment - fragmentCountHeightMain) * fragmentPixelHeight,
                fragmentPixelWidth, fragmentPixelHeight);

            //копировать пиксели прямоугольника-фрагмента в массив byte[]
            mainBitmapImage.CopyPixels(pixelsRect, fragmentPixels, stride, 0);

            //записать пиксели фрагмента в временный bitmap для фрагмента
            tempBitmapFragment.WritePixels(new Int32Rect(0, 0, fragmentPixelWidth, fragmentPixelHeight), fragmentPixels, stride, 0);

            //создать новый ImageFragment
            imageFragments[currentWidthFragment, currentHeightFragment] = new ImageFragment(tempBitmapFragment);

            Color resultColor = imageFragments[currentWidthFragment, currentHeightFragment].GetResultFragmentColor();
            Color gridColor = Color.FromArgb(0, 0, 0, 0);

            DrawFragmentOnSplittedImage(resultColor, gridColor, currentWidthFragment, currentHeightFragment, fragmentWidthMain, fragmentHeightMain);

            return resultColor;
        }
    }
}
