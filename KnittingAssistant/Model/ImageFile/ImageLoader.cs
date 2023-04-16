using Microsoft.Win32;

namespace KnittingAssistant.Model
{
    public class ImageLoader
    {
        private OpenFileDialog fileDialog;

        public ImageLoader()
        {
            fileDialog = new OpenFileDialog();
            fileDialog.Filter = @"JPG|*.jpg;*.jpeg" +
                "|PNG|*.png" +
                "|TIFF|*.tif;*.tiff";
            fileDialog.FilterIndex = 1;
        }

        public string GetImageFilename()
        {
            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            return string.Empty;
        }

        public bool IsSupportedFormat(string filename)
        {
            string ext = "." + filename.Split(new char[] { '.' })[^1];
            if (fileDialog.Filter.Contains(ext)) return true;
            return false;
        }
    }
}
