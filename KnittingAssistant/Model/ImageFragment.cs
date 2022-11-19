using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace KnittingAssistant.Model
{
    public class ImageFragment
    {
        Color averageColor; //среднее значение цвета фрагмента по 3 каналам R, G, B
        Image mainImage; //полное исходное изображение
        BitmapImage imageFragment; //кусок исходного изображения, представленный в фрагменте
        BitmapImage resultFragment; //результирующий фрагмент, закрашенный средний цветом
        int fragmentWidth, fragmentHeight; //ширина, высота фрагмента в пикселях
        int mainImageDeltaX, mainImageDeltaY; //смещение исходного изображения относительно фрагмента в пикселях

        public ImageFragment(BitmapImage imageFragment)
        {
            this.imageFragment = imageFragment;
        }

        public void TakeImageFragment()
        {
            //взять фрагмент из исходного изображения
        }

        private void FindAverageColor()
        {
            //найти средний цвет полученного фрагмента
        }

        public void GetResultFragment()
        {
            //вернуть результирующий фрагмент среднего цвета
        }
    }
}
