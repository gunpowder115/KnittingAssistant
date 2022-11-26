﻿using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace KnittingAssistant.Model
{
    public class SplitImage
    {
        ImageFragment[,] imageFragments;
        int fragmentWidthInPixels, fragmentHeightInPixels;

        public SplitImage(Image mainImage, int fragmentCountWidth, int fragmentCountHeight, 
            int fragmentWidthInPixels, int fragmentHeightInPixels)
        {
            //this.fragmentWidthInPixels = fragmentWidthInPixels;
            //this.fragmentHeightInPixels = fragmentHeightInPixels;
            imageFragments = new ImageFragment[fragmentCountWidth, fragmentCountHeight]; //массив фрагментов изображения
            BitmapImage mainBitmapImage = (BitmapImage)mainImage.Source; //главный bitmap

            for (int i = 0; i < fragmentCountWidth; i++)
            {
                for (int j = 0; j < fragmentCountHeight; j++)
                {
                    //временный bitmap для фрагмента
                    WriteableBitmap tempBitmapFragment = new WriteableBitmap(fragmentWidthInPixels, fragmentHeightInPixels, 96, 96,
                        PixelFormats.Bgra32, null);

                    //шаг и размер массива верные
                    int stride = (int)tempBitmapFragment.PixelWidth * (tempBitmapFragment.Format.BitsPerPixel + 7) / 8;
                    byte[] fragmentPixels = new byte[(int)mainBitmapImage.PixelHeight * stride];

                    //копировать пиксели прямоугольника-фрагмента в массив byte[]
                    mainBitmapImage.CopyPixels(new Int32Rect(i * fragmentWidthInPixels, j * fragmentHeightInPixels,
                        fragmentWidthInPixels, fragmentHeightInPixels), fragmentPixels, stride, 0);

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