using KnittingAssistant.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.ViewModel
{
    class ColorsViewModel : ViewModelBase
    {
        #region Dependency Properties

        private Grid m_AddedColors;
        public Grid AddedColors
        {
            get { return m_AddedColors; }
            set
            {
                m_AddedColors = value;
                OnPropertyChanged("AddedColors");
            }
        }

        private double m_BlueSelectedColorValue;
        public double BlueSelectedColorValue
        {
            get { return m_BlueSelectedColorValue; }
            set
            {
                m_BlueSelectedColorValue = value;
                OnPropertyChanged("BlueSelectedColorValue");
            }
        }

        private double m_GreenSelectedColorValue;
        public double GreenSelectedColorValue
        {
            get { return m_GreenSelectedColorValue; }
            set
            {
                m_GreenSelectedColorValue = value;
                OnPropertyChanged("GreenSelectedColorValue");
            }
        }

        private double m_RedSelectedColorValue;
        public double RedSelectedColorValue
        {
            get { return m_RedSelectedColorValue; }
            set
            {
                m_RedSelectedColorValue = value;
                OnPropertyChanged("RedSelectedColorValue");
            }
        }

        private bool m_IsColorAdding;
        public bool IsColorAdding
        {
            get { return m_IsColorAdding; }
            set
            {
                m_IsColorAdding = value;
                OnPropertyChanged("IsColorAdding");
            }
        }

        private bool m_IsColorDeleting;
        public bool IsColorDeleting
        {
            get { return m_IsColorDeleting; }
            set
            {
                m_IsColorDeleting = value;
                OnPropertyChanged("IsColorDeleting");
            }
        }

        #endregion

        #region Relay Commands

        private RelayCommand addColorCommand;
        public RelayCommand AddColorCommand
        {
            get
            {
                return addColorCommand ??
                    (addColorCommand = new RelayCommand(obj =>
                    {
                        colorStorage.AddColor(selectedColor);
                        AddedColors = colorStorage.GetColorGridFromFile();
                    }));
            }
        }

        private RelayCommand deleteColorCommand;
        public RelayCommand DeleteColorCommand
        {
            get
            {
                return deleteColorCommand ??
                    (deleteColorCommand = new RelayCommand(obj =>
                    {
                        
                    }));
            }
        }

        private RelayCommand clearColorsCommand;
        public RelayCommand ClearColorsCommand
        {
            get
            {
                return clearColorsCommand ??
                    (clearColorsCommand = new RelayCommand(obj =>
                    {
                        colorStorage.ClearColors();
                        AddedColors = colorStorage.GetColorGridFromFile();
                    }));
            }
        }

        #endregion

        private const int addedColorsGridWidth = 12;
        private const int addedColorsGridHeight = 3;
        private ColorStorage colorStorage;
        private Color selectedColor;
        public Image PaletteAreaImage { get; set; }
        public Border SelectedColor { get; set; }

        public ColorsViewModel()
        {
            RedSelectedColorValue = 0;
            GreenSelectedColorValue = 0;
            BlueSelectedColorValue = 0;

            IsColorAdding = false;
            IsColorDeleting = false;

            colorStorage = new ColorStorage("colors.txt", addedColorsGridWidth, addedColorsGridHeight);

            AddedColors = colorStorage.GetColorGridFromFile();
        }

        public void SetSelectedColorCommand(object sender, MouseButtonEventArgs e)
        {
            PaletteAreaImage.Source = new BitmapImage(new Uri("D:/Development/KnittingAssistant/KnittingAssistant/View/resources/square_palette_image.png"));

            BitmapImage tempBitmap = (BitmapImage)PaletteAreaImage.Source;
            WriteableBitmap bitmapPalette = new WriteableBitmap(tempBitmap.PixelWidth, tempBitmap.PixelHeight,
                tempBitmap.DpiX, tempBitmap.DpiY,
                tempBitmap.Format, tempBitmap.Palette);

            Point position = e.GetPosition(PaletteAreaImage);
            if ((position.X <= bitmapPalette.PixelWidth) && (position.Y <= bitmapPalette.PixelHeight))
            {
                int stride = (int)bitmapPalette.PixelWidth * bitmapPalette.Format.BitsPerPixel / 8;

                byte[] currentPixel = new byte[4];
                tempBitmap.CopyPixels(new Int32Rect((int)position.X, (int)position.Y, 1, 1), currentPixel, stride, 0);

                BlueSelectedColorValue = currentPixel[0];
                GreenSelectedColorValue = currentPixel[1];
                RedSelectedColorValue = currentPixel[2];
                selectedColor = Color.FromRgb((byte)RedSelectedColorValue, (byte)GreenSelectedColorValue, (byte)BlueSelectedColorValue);

                SelectedColor.Background = new SolidColorBrush(selectedColor);
                if (SelectedColor.Child != null)
                    SelectedColor.Child = null;

                IsColorAdding = true;
                IsColorDeleting = false;
            }
        }

        private void InitAddedColorsGrid()
        {
            AddedColors = new Grid();

            ColumnDefinition[] colDef = new ColumnDefinition[addedColorsGridWidth];
            RowDefinition[] rowDef = new RowDefinition[addedColorsGridHeight];            
            for (int i = 0; i < addedColorsGridWidth; i++)
            {
                colDef[i] = new ColumnDefinition();
                colDef[i].MaxWidth = 30;
                AddedColors.ColumnDefinitions.Add(colDef[i]);
            }
            for (int j = 0; j < addedColorsGridHeight; j++)
            {
                rowDef[j] = new RowDefinition();
                rowDef[j].MinHeight = 30;
                AddedColors.RowDefinitions.Add(rowDef[j]);
            }

            Border addedColorBorder;
            for (int i = 0; i < addedColorsGridWidth; i++)
            {
                for (int j = 0; j < addedColorsGridHeight; j++)
                {
                    addedColorBorder = new Border();
                    addedColorBorder.Width = addedColorBorder.Height = 20;
                    addedColorBorder.BorderBrush = Brushes.Gray;
                    addedColorBorder.BorderThickness = new Thickness(2);
                    Grid.SetColumn(addedColorBorder, i);
                    Grid.SetRow(addedColorBorder, j);
                    AddedColors.Children.Add(addedColorBorder);
                }
            }
        }
    }
}
