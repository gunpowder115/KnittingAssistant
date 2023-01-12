using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System;

namespace KnittingAssistant.Model
{
    public class ImageFragment
    {
        private Color averageColor; //среднее значение цвета фрагмента по 3 каналам R, G, B
        private Color nearestColor; //значение ближайшего к среднему цвета из хранилища
        private WriteableBitmap fragmentBitmap; //кусок исходного изображения, представленный в фрагменте
        private int fragmentWidthInPixels, fragmentHeightInPixels; //ширина, высота фрагмента в пикселях
        private ColorStorage colorStorage; //хранилище цветов
        private int[,] colorBytes;
        private int[] averageColorBytes;

        public ImageFragment(WriteableBitmap fragmentBitmap, int fragmentWidthInPixels, int fragmentHeightInPixels)
        {
            this.fragmentBitmap = fragmentBitmap;
            this.fragmentWidthInPixels = fragmentWidthInPixels;
            this.fragmentHeightInPixels = fragmentHeightInPixels;

            colorStorage = new ColorStorage();
            colorStorage.ReadColorsFromFile();
            colorBytes = new int[colorStorage.ColorsCount, 3];
            averageColorBytes = new int[3];
            for (int i = 0; i < colorStorage.ColorsCount; i++)
            {
                colorBytes[i, 0] = colorStorage.GetNodeByIndex(i).Value.R;
                colorBytes[i, 1] = colorStorage.GetNodeByIndex(i).Value.G;
                colorBytes[i, 2] = colorStorage.GetNodeByIndex(i).Value.B;
            }
        }

        //найти средний цвет полученного фрагмента
        private void FindAverageColor()
        {
            int sumB = 0, sumG = 0, sumR = 0;
            int numPixels = fragmentWidthInPixels * fragmentHeightInPixels;
            for (int i = 0; i < fragmentWidthInPixels; i++)
            {
                for (int j = 0; j < fragmentHeightInPixels; j++)
                {
                    int stride = (int)fragmentBitmap.PixelWidth * fragmentBitmap.Format.BitsPerPixel / 8;

                    byte[] currentPixel = new byte[4];
                    fragmentBitmap.CopyPixels(new Int32Rect(i, j, 1, 1), currentPixel, stride, 0);
                    sumB += currentPixel[0];
                    sumG += currentPixel[1];
                    sumR += currentPixel[2];
                }
            }
            averageColorBytes[2] = sumB / numPixels;
            averageColorBytes[1] = sumG / numPixels;
            averageColorBytes[0] = sumR / numPixels;

            averageColor = Color.FromArgb(averageColorBytes[0], averageColorBytes[1], averageColorBytes[2]);
        }

        //заполнить фрагмент найденным/выбранным цветом
        private void FillFragmentByColor(Color color)
        {
            byte B = color.B;
            byte G = color.G;
            byte R = color.R;
            byte[] colorData = { B, G, R, 0 };
            for (int i = 0; i < fragmentWidthInPixels; i++)
            {
                for (int j = 0; j < fragmentHeightInPixels; j++)
                {
                    Int32Rect rect = new Int32Rect(i, j, 1, 1);
                    fragmentBitmap.WritePixels(rect, colorData, 4, 0);
                }
            }
        }

        //выбрать ближайший к среднему цвет из хранилища
        private void SelectNearestColor()
        {
            int currColorIndex = 0;
            int currColorKoef = 30 * (int)Math.Pow(colorBytes[currColorIndex, 0] - averageColorBytes[0], 2) +
                    59 * (int)Math.Pow(colorBytes[currColorIndex, 1] - averageColorBytes[1], 2) +
                    11 * (int)Math.Pow(colorBytes[currColorIndex, 2] - averageColorBytes[2], 2);
            
            for (int i = 1; i < colorStorage.ColorsCount; i++)
            {
                int colorKoef = 30 * (int)Math.Pow(colorBytes[i, 0] - averageColorBytes[0], 2) + 
                    59 * (int)Math.Pow(colorBytes[i, 1] - averageColorBytes[1], 2) +
                    11 * (int)Math.Pow(colorBytes[i, 2] - averageColorBytes[2], 2);

                if (colorKoef < currColorKoef)
                {
                    currColorKoef = colorKoef;
                    currColorIndex = i;
                }
            }

            nearestColor = Color.FromArgb(colorBytes[currColorIndex, 0], colorBytes[currColorIndex, 1], colorBytes[currColorIndex, 2]);
        }
        
        private void SelectNearestColor_My()
        {
            int[] indexNearest = new int[3];
            int[] colorByteDelta = new int[3];

            for (int colorByte = 0; colorByte < 3; colorByte++)
            {
                indexNearest[colorByte] = 0;
                colorByteDelta[colorByte] = 255;
                for (int i = 0; i < colorStorage.ColorsCount; i++)
                {
                    int delta = Math.Abs(colorBytes[i, colorByte] - averageColorBytes[colorByte]);
                    if (delta < colorByteDelta[colorByte])
                    {
                        indexNearest[colorByte] = i;
                        colorByteDelta[colorByte] = delta;
                    }
                }
            }

            int indexR = indexNearest[0];
            int indexG = indexNearest[1];
            int indexB = indexNearest[2];

            int selectedIndex = 0;
            if (indexR == indexG && indexG == indexB)
                selectedIndex = indexR;
            else if ((indexR == indexG) || (indexG == indexB) || (indexB == indexR))
            {
                if (indexR == indexG) selectedIndex = indexR;
                else if (indexG == indexB) selectedIndex = indexG;
                else selectedIndex = indexB;
            }
            else
                selectedIndex = FindIndexOfMin(colorByteDelta);

            nearestColor = Color.FromArgb(colorBytes[selectedIndex, 0], colorBytes[selectedIndex, 1], colorBytes[selectedIndex, 2]);
        }

        private int FindIndexOfMin(int[] numbers)
        {
            int index = 0;
            int value = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                if (numbers[i] < value)
                    index = i;
            }
            return index;
        }

        //вернуть результирующий фрагмент выбранного цвета
        public WriteableBitmap GetResultFragment()
        {
            FindAverageColor();
            SelectNearestColor();
            //FillFragmentByColor(averageColor);
            FillFragmentByColor(nearestColor);

            return fragmentBitmap;
        }
    }
}
