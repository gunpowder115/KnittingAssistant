using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;

namespace KnittingAssistant.Model
{
    public class ImageFragment
    {
        Color averageColor; //среднее значение цвета фрагмента по 3 каналам R, G, B
        WriteableBitmap fragmentBitmap; //кусок исходного изображения, представленный в фрагменте
        int fragmentWidthInPixels, fragmentHeightInPixels; //ширина, высота фрагмента в пикселях

        public ImageFragment(WriteableBitmap fragmentBitmap, int fragmentWidthInPixels, int fragmentHeightInPixels)
        {
            this.fragmentBitmap = fragmentBitmap;
            this.fragmentWidthInPixels = fragmentWidthInPixels;
            this.fragmentHeightInPixels = fragmentHeightInPixels;
        }

        //найти средний цвет полученного фрагмента
        private void FindAverageColor()
        {
            int sumB = 0, sumG = 0, sumR = 0, sumA = 0;
            int numPixels = fragmentWidthInPixels * fragmentHeightInPixels;
            for (int i = 0; i < fragmentWidthInPixels; i++)
            {
                for (int j = 0; j < fragmentHeightInPixels; j++)
                {
                    int stride = (int)fragmentBitmap.PixelWidth * (fragmentBitmap.Format.BitsPerPixel + 7) / 8;
                    byte[] currentPixel = new byte[4];
                    fragmentBitmap.CopyPixels(new Int32Rect(i, j, 1, 1), currentPixel, stride, 0);
                    sumB += currentPixel[0];
                    sumG += currentPixel[1];
                    sumR += currentPixel[2];
                }
            }
            int resultB = sumB / numPixels;
            int resultG = sumG / numPixels;
            int resultR = sumR / numPixels;
            averageColor = Color.FromArgb(resultR, resultG, resultB);
        }

        //заполнить фрагмент найденным средним цветом
        private void FillFragmentByAverageColor()
        {
            byte B = averageColor.B;
            byte G = averageColor.G;
            byte R = averageColor.R;
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

        //вернуть результирующий фрагмент среднего цвета
        public WriteableBitmap GetResultFragment()
        {
            FindAverageColor();
            FillFragmentByAverageColor();

            return fragmentBitmap;
        }
    }
}
