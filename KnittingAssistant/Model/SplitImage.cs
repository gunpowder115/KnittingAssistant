using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.Model
{
    public class SplitImage
    {
        ImageFragment[,] imageFragments;
        int fragmentWidth, fragmentHeight;

        public SplitImage(Image mainImage, int numFragmentsInWidth, int numFragmentsInHeight)
        {
            imageFragments = new ImageFragment[numFragmentsInWidth, numFragmentsInHeight];

            for (int i = 0; i < numFragmentsInWidth; i++)
            {
                for (int j = 0; j < numFragmentsInHeight; j++)
                {
                    BitmapImage tempBitmapImage = (BitmapImage)mainImage.Source;
                }
            }
        }
    }
}
