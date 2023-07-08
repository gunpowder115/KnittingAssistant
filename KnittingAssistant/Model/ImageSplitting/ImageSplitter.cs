using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.Model
{
    public class ImageSplitter
    {
        private int fragmentWidthMain, fragmentHeightMain;
        private int fragmentWidthSecondary, fragmentHeightSecondary;
        private int fragmentCountWidthMain, fragmentCountHeightMain;
        private int fragmentCountWidthSecondary, fragmentCountHeightSecondary;
        private bool keepMainImageRatio;
        private WriteableBitmap mainBitmapImage;

        private Color[,] resultFragmentColors;
        private int sumR, sumG, sumB;
        private ColorStorage colorStorage;

        public ImageFragment[,] imageFragments { get; }
        public WriteableBitmap SplittedBitmapImage { get; }
        public WriteableBitmap GridBitmapImage { get; }
        public bool isAverageColor => ImageFragment.IsAverageColor;

        public ImageSplitter(WriteableBitmap mainImage, Fragmentation widthFragmentation, Fragmentation heightFragmentation)
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

            mainBitmapImage = mainImage; //главный bitmap

            int bitmapWidth = fragmentCountWidthMain * fragmentWidthMain + fragmentCountWidthSecondary * fragmentWidthSecondary;
            int bitmapHeight = fragmentCountHeightMain * fragmentHeightMain + fragmentCountHeightSecondary * fragmentHeightSecondary;
            SplittedBitmapImage = new WriteableBitmap((fragmentCountWidthMain + fragmentCountWidthSecondary) * fragmentWidthMain,
                (fragmentCountHeightMain + fragmentCountHeightSecondary) * fragmentHeightMain,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);
            GridBitmapImage = new WriteableBitmap((fragmentCountWidthMain + fragmentCountWidthSecondary) * fragmentWidthMain,
                (fragmentCountHeightMain + fragmentCountHeightSecondary) * fragmentHeightMain,
                mainBitmapImage.DpiX, mainBitmapImage.DpiY, mainBitmapImage.Format, mainBitmapImage.Palette);

            keepMainImageRatio = fragmentWidthMain == 1 && fragmentHeightMain == 1;
            resultFragmentColors = new Color[fragmentCountWidthMain + fragmentCountWidthSecondary,
                fragmentCountHeightMain + fragmentCountHeightSecondary];
            sumR = sumG = sumB = 0;

            colorStorage = ColorStorage.GetInstance();
            colorStorage.ReadColorsFromFile();
            colorStorage.ClearColorsCounting();
            if (colorStorage.ColorsCount != 0)
                colorStorage.RefreshColorsCounting();
        }

        public void SplitImage(int currentWidthFragment, int currentHeightFragment)
        {
            try
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
                imageFragments[currentWidthFragment, currentHeightFragment] = new ImageFragment(tempBitmapFragment, colorStorage);

                resultFragmentColors[currentWidthFragment, currentHeightFragment] = imageFragments[currentWidthFragment, currentHeightFragment].GetResultFragmentColor();

                if (!colorStorage.ColorsCounting.ContainsKey(resultFragmentColors[currentWidthFragment, currentHeightFragment]))
                    colorStorage.ColorsCounting[resultFragmentColors[currentWidthFragment, currentHeightFragment]] = 0;
                colorStorage.ColorsCounting[resultFragmentColors[currentWidthFragment, currentHeightFragment]]++;

                sumR += imageFragments[currentWidthFragment, currentHeightFragment].AverageColor.R;
                sumG += imageFragments[currentWidthFragment, currentHeightFragment].AverageColor.G;
                sumB += imageFragments[currentWidthFragment, currentHeightFragment].AverageColor.B;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void DrawSplittedImage()
        {
            Color gridColor = SelectGridColor();
            for (int i = 0; i < fragmentCountWidthMain + fragmentCountWidthSecondary; i++)
            {
                for (int j = 0; j < fragmentCountHeightMain + fragmentCountHeightSecondary; j++)
                {
                    DrawFragmentOnSplittedImage(resultFragmentColors[i, j], gridColor, i, j, fragmentWidthMain, fragmentHeightMain);
                }
            }
        }

        private Color SelectGridColor()
        {
            int fragmentCount = (fragmentCountWidthMain + fragmentCountWidthSecondary) * (fragmentCountHeightMain + fragmentCountHeightSecondary);
            return SelectNegativeRgbColor(Color.FromArgb(0, (byte)(sumR / fragmentCount), (byte)(sumG / fragmentCount), (byte)(sumB / fragmentCount)));
        }

        private Color SelectNegativeRgbColor(Color color)
        {
            Color negativeColor;
            negativeColor.R = (byte)(255 - color.R);
            negativeColor.G = (byte)(255 - color.G);
            negativeColor.B = (byte)(255 - color.B);
            return negativeColor;
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
    }
}
