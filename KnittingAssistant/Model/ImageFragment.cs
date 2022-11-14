using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.Model
{
    public class ImageFragment
    {
        Color averageColor; //среднее значение цвета фрагмента по 3 каналам R, G, B
        BitmapImage imageFragment; //кусок исходного изображения, представленный в фрагменте
        BitmapImage resultFragment; //результирующий фрагмент, закрашенный средний цветом

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
