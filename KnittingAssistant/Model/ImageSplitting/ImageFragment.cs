﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.Model
{
    public class ImageFragment
    {
        private Color nearestColor; //значение ближайшего к среднему цвета из хранилища
        private WriteableBitmap fragmentBitmap; //кусок исходного изображения, представленный в фрагменте
        private int fragmentWidthInPixels, fragmentHeightInPixels; //ширина, высота фрагмента в пикселях
        private ColorStorage colorStorage; //хранилище цветов
        private int[,] colorBytes;
        private int[] averageColorBytes;

        public static bool IsAverageColor { get; private set; }
        public Color AverageColor { get; private set; } //среднее значение цвета фрагмента по 3 каналам R, G, B

        public ImageFragment(WriteableBitmap fragmentBitmap)
        {
            this.fragmentBitmap = fragmentBitmap;
            this.fragmentWidthInPixels = fragmentBitmap.PixelWidth;
            this.fragmentHeightInPixels = fragmentBitmap.PixelHeight;

            colorStorage = new ColorStorage();
            colorStorage.ReadColorsFromFile();
            IsAverageColor = colorStorage.ColorsCount == 0;
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

            AverageColor = Color.FromRgb((byte)averageColorBytes[0], (byte)averageColorBytes[1], (byte)averageColorBytes[2]);
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
            if (IsAverageColor)
            {
                nearestColor = AverageColor;
            }
            else
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

                nearestColor = Color.FromRgb((byte)colorBytes[currColorIndex, 0], (byte)colorBytes[currColorIndex, 1], (byte)colorBytes[currColorIndex, 2]);
            }
        }

        //вернуть результирующий фрагмент выбранного цвета
        public Color GetResultFragmentColor()
        {
            FindAverageColor();
            SelectNearestColor();

            return nearestColor;
        }
    }
}
