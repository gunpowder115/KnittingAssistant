using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace KnittingAssistant.Model
{
    public class SplitImage
    {
        public ImageFragment[,] imageFragments { get; }

        public SplitImage(Image mainImage, int fragmentCountWidth, int fragmentCountHeight, 
            int fragmentWidthInPixels, int fragmentHeightInPixels)
        {
            imageFragments = new ImageFragment[fragmentCountWidth, fragmentCountHeight]; //массив фрагментов изображения
            BitmapImage mainBitmapImage = (BitmapImage)mainImage.Source; //главный bitmap

            for (int i = 0; i < fragmentCountWidth; i++)
            {
                for (int j = 0; j < fragmentCountHeight; j++)
                {
                    //временный bitmap для фрагмента
                    WriteableBitmap tempBitmapFragment = new WriteableBitmap(fragmentWidthInPixels, fragmentHeightInPixels, 
                        (mainImage.Source as BitmapSource).DpiX, (mainImage.Source as BitmapSource).DpiY,
                        (mainImage.Source as BitmapSource).Format, (mainImage.Source as BitmapSource).Palette);

                    //шаг и размер массива верные
                    int stride = (int)tempBitmapFragment.PixelWidth * (tempBitmapFragment.Format.BitsPerPixel + 0) / 8;
                    byte[] fragmentPixels = new byte[tempBitmapFragment.PixelHeight * stride];

                    Int32Rect pixelsRect = new Int32Rect(i * fragmentWidthInPixels, j * fragmentHeightInPixels,
                        fragmentWidthInPixels, fragmentHeightInPixels);

                    //копировать пиксели прямоугольника-фрагмента в массив byte[]
                    mainBitmapImage.CopyPixels(pixelsRect, fragmentPixels, stride, 0);

                    //записать пиксели фрагмента в временный bitmap для фрагмента
                    tempBitmapFragment.WritePixels(new Int32Rect(0, 0, fragmentWidthInPixels, fragmentHeightInPixels), fragmentPixels, stride, 0);

                    //создать новый ImageFragment
                    imageFragments[i, j] = new ImageFragment(tempBitmapFragment, fragmentWidthInPixels, fragmentHeightInPixels);
                }
            }
        }

        //взять фрагмент из исходного изображения
        public void TakeImageFragment()
        {
            
        }
    }
}
