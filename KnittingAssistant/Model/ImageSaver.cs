using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.Model
{
    enum ImageFormat
    {
        Jpg,
        Png,
        Tiff,
        Gif,
        Bmp
    }

    public class ImageSaver
    {
        private SaveFileDialog fileDialog;
        private BitmapEncoder bitmapEncoder;

        public ImageSaver()
        {
            fileDialog = new SaveFileDialog();
            fileDialog.Filter = @"JPG|*.jpg;*.jpeg" +
                "|PNG|*.png" +
                "|TIFF|*.tif;*.tiff" + 
                "|GIF|*gif" + 
                "|BMP|*.bmp";
            fileDialog.FilterIndex = 1;
            fileDialog.DefaultExt = ".jpg";
            fileDialog.FileName = "Image";
        }

        public void SaveImage(WriteableBitmap writeableBitmap)
        {
            if (fileDialog.ShowDialog() == true)
            {
                string filename = fileDialog.FileName;
                SelectBitmapEncoder();
                bitmapEncoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
                using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                    bitmapEncoder.Save(fileStream);
            }
        }

        private ImageFormat SelectImageFormat()
        {
            string lowerFilename = fileDialog.FileName.ToLower();
            if (lowerFilename.Contains(".jpg") || lowerFilename.Contains(".jpeg"))
                return ImageFormat.Jpg;
            else if (lowerFilename.Contains(".png"))
                return ImageFormat.Png;
            else if (lowerFilename.Contains(".gif"))
                return ImageFormat.Gif;
            else if (lowerFilename.Contains(".tif") || lowerFilename.Contains(".tiff"))
                return ImageFormat.Tiff;
            else if (lowerFilename.Contains(".bmp"))
                return ImageFormat.Bmp;
            else
                return ImageFormat.Jpg;
        }

        private void SelectBitmapEncoder()
        {
            switch(SelectImageFormat())
            {
                case ImageFormat.Jpg:
                    bitmapEncoder = new JpegBitmapEncoder();
                    break;
                case ImageFormat.Png:
                    bitmapEncoder = new PngBitmapEncoder();
                    break;
                case ImageFormat.Tiff:
                    bitmapEncoder = new TiffBitmapEncoder();
                    break;
                case ImageFormat.Gif:
                    bitmapEncoder = new GifBitmapEncoder();
                    break;
                case ImageFormat.Bmp:
                    bitmapEncoder = new BmpBitmapEncoder();
                    break;
                default:
                    bitmapEncoder = new JpegBitmapEncoder();
                    break;
            }
        }
    }
}
