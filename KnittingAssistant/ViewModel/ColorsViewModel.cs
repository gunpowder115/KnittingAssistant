using KnittingAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KnittingAssistant.ViewModel
{
    public class ColorsViewModel : ViewModelBase
    {
        private const int addedColorsGridWidth = 11;
        private const int addedColorsGridHeight = 3;
        private const string DefaultPaletteFilename = "D:/Development/KnittingAssistant/KnittingAssistant/View/resources/square_palette_image_full.png";
        private const string EmptySelectedColorFilename = "D:/Development/KnittingAssistant/KnittingAssistant/View/resources/large_empty_image.png";
        private ColorStorage colorStorage;
        private Color selectedColorForAdding;
        private int selectedColorIndex;
        private LinkedListNode<Color> selectedLinkedListNode;
        private ArrayToGridIndexConverter arrayToGridIndexConverter;

        #region Dependency Properties

        private Grid addedColors;
        public Grid AddedColors
        {
            get { return addedColors; }
            set
            {
                addedColors = value;
                OnPropertyChanged("AddedColors");
            }
        }

        private double blueSelectedColorValue;
        public double BlueSelectedColorValue
        {
            get { return blueSelectedColorValue; }
            set
            {
                blueSelectedColorValue = value;
                OnPropertyChanged("BlueSelectedColorValue");
            }
        }

        private double greenSelectedColorValue;
        public double GreenSelectedColorValue
        {
            get { return greenSelectedColorValue; }
            set
            {
                greenSelectedColorValue = value;
                OnPropertyChanged("GreenSelectedColorValue");
            }
        }

        private double redSelectedColorValue;
        public double RedSelectedColorValue
        {
            get { return redSelectedColorValue; }
            set
            {
                redSelectedColorValue = value;
                OnPropertyChanged("RedSelectedColorValue");
            }
        }

        private bool isColorAdding;
        public bool IsColorAdding
        {
            get { return isColorAdding; }
            set
            {
                isColorAdding = value;
                OnPropertyChanged("IsColorAdding");
            }
        }

        private bool isColorRemoving;
        public bool IsColorRemoving
        {
            get { return isColorRemoving; }
            set
            {
                isColorRemoving = value;
                OnPropertyChanged("IsColorRemoving");
            }
        }

        private WriteableBitmap paletteAreaImage;
        public WriteableBitmap PaletteAreaImage
        {
            get { return paletteAreaImage; }
            set
            {
                paletteAreaImage = value;
                OnPropertyChanged("PaletteAreaImage");
            }
        }

        private WriteableBitmap selectedColorImage;
        public WriteableBitmap SelectedColorImage
        {
            get { return selectedColorImage; }
            set
            {
                selectedColorImage = value;
                OnPropertyChanged("SelectedColorImage");
            }
        }

        private SolidColorBrush selectedColorBackground;
        public SolidColorBrush SelectedColorBackground
        {
            get { return selectedColorBackground; }
            set
            {
                selectedColorBackground = value;
                OnPropertyChanged("SelectedColorBackground");
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
                        colorStorage.AddColor(selectedColorForAdding);
                        AddedColors = CreateColorGrid(colorStorage.ColorList);
                    }));
            }
        }

        private RelayCommand removeColorCommand;
        public RelayCommand RemoveColorCommand
        {
            get
            {
                return removeColorCommand ??
                    (removeColorCommand = new RelayCommand(obj =>
                    {
                        LinkedListNode<Color> nextSelectedColorLinkedListNode;

                        if (selectedColorIndex == colorStorage.ColorsCount - 1)
                        {
                            selectedColorIndex--;
                            nextSelectedColorLinkedListNode = selectedLinkedListNode.Previous;
                        }
                        else if (selectedColorIndex == 0 && colorStorage.ColorsCount == 1)
                        {
                            selectedColorIndex = -1;
                            nextSelectedColorLinkedListNode = null;
                        }
                        else
                        {
                            nextSelectedColorLinkedListNode = selectedLinkedListNode.Next;
                        }

                        colorStorage.RemoveColor(selectedLinkedListNode);
                        AddedColors = CreateColorGrid(colorStorage.ColorList);

                        selectedLinkedListNode = nextSelectedColorLinkedListNode;

                        ShowSelectedColor(selectedLinkedListNode);
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
                        AddedColors = CreateColorGrid(colorStorage.ColorList);
                        selectedLinkedListNode = null;
                        ShowSelectedColor(selectedLinkedListNode);
                    }));
            }
        }

        private RelayCommand saveColorsCommand;
        public RelayCommand SaveColorsCommand
        {
            get
            {
                return saveColorsCommand ??
                    (saveColorsCommand = new RelayCommand(obj =>
                    {
                        colorStorage.WriteColorsToFile();
                    }));
            }
        }

        private RelayCommand notSaveColorsCommand;
        public RelayCommand NotSaveColorsCommand
        {
            get
            {
                return notSaveColorsCommand ??
                    (notSaveColorsCommand = new RelayCommand(obj =>
                    {

                    }));
            }
        }

        private RelayCommand sliderRgbValueChangedCommand;
        public RelayCommand SliderRgbValueChangedCommand
        {
            get
            {
                return sliderRgbValueChangedCommand ??
                    (sliderRgbValueChangedCommand = new RelayCommand(obj =>
                    {
                        selectedColorForAdding = Color.FromRgb((byte)RedSelectedColorValue,
                            (byte)GreenSelectedColorValue, (byte)BlueSelectedColorValue);
                        ShowSelectedColor(selectedColorForAdding);
                        IsColorAdding = true;
                        IsColorRemoving = false;
                    }));
            }
        }

        #endregion

        public ColorsViewModel()
        {
            PaletteAreaImage = SetWriteableBitmap(DefaultPaletteFilename);
            SelectedColorImage = SetWriteableBitmap(EmptySelectedColorFilename);
            RedSelectedColorValue = 0;
            GreenSelectedColorValue = 0;
            BlueSelectedColorValue = 0;

            arrayToGridIndexConverter = new ArrayToGridIndexConverter(addedColorsGridWidth, addedColorsGridHeight);
            colorStorage = new ColorStorage();
            colorStorage.ReadColorsFromFile();
            AddedColors = CreateColorGrid(colorStorage.ColorList);

            IsColorAdding = false;
            IsColorRemoving = false;
        }

        public void SetSelectedColorForAddingCommand(object sender, MouseButtonEventArgs e)
        {
            Image senderImage = sender as Image;
            WriteableBitmap senderBitmap = (WriteableBitmap)((sender as Image)?.Source);
            int pixelWidth = senderBitmap.PixelWidth;
            int pixelHeight = senderBitmap.PixelHeight;
            byte[] pixels = new byte[pixelWidth * pixelHeight * 4];
            senderBitmap.CopyPixels(pixels, pixelWidth * 4, 0);

            Point position = e.GetPosition((IInputElement)sender);

            if (position.X <= pixelWidth && position.Y <= pixelHeight)
            {
                int offset = ((int)position.Y * pixelWidth + (int)position.X) * 4;
                RedSelectedColorValue = pixels[offset + 2];
                GreenSelectedColorValue = pixels[offset + 1];
                BlueSelectedColorValue = pixels[offset + 0];

                selectedColorForAdding = Color.FromArgb(pixels[offset + 3],
                    (byte)RedSelectedColorValue,
                    (byte)GreenSelectedColorValue,
                    (byte)BlueSelectedColorValue);

                ShowSelectedColor(selectedColorForAdding);

                IsColorAdding = true;
                IsColorRemoving = false;
            }
        }

        public void SetSelectedColorForRemovingCommand(object sender, MouseButtonEventArgs e)
        {
            Border selectedColorBorder = sender as Border;
            selectedColorIndex = (selectedColorBorder.Parent as Grid).Children.IndexOf(selectedColorBorder);
            selectedLinkedListNode = colorStorage.GetNodeByIndex(selectedColorIndex);

            ShowSelectedColor(selectedLinkedListNode);

            IsColorAdding = false;
            IsColorRemoving = true;
        }

        public void ShowSelectedColor(Color selectedColor)
        {
            SelectedColorBackground = new SolidColorBrush(selectedColor);
            BlueSelectedColorValue = selectedColor.B;
            GreenSelectedColorValue = selectedColor.G;
            RedSelectedColorValue = selectedColor.R;

            if (SelectedColorImage != null)
                SelectedColorImage = null;
        }

        public void ShowSelectedColor(LinkedListNode<Color> selectedColorNode)
        {
            if (selectedColorNode != null)
                ShowSelectedColor(selectedColorNode.Value);
            else
            {
                SelectedColorImage = SetWriteableBitmap(EmptySelectedColorFilename);
                BlueSelectedColorValue = GreenSelectedColorValue = RedSelectedColorValue = 0;
            }
        }

        private Grid CreateColorGrid(LinkedList<Color> colorList)
        {
            GridIndex gridSize = arrayToGridIndexConverter.GetGridSize(colorStorage.ColorList.Count);
            int columnCount = gridSize.column;
            int rowCount = gridSize.row;

            Grid addedColors = new Grid();
            ColumnDefinition[] colDef = new ColumnDefinition[columnCount];
            RowDefinition[] rowDef = new RowDefinition[rowCount];
            for (int i = 0; i < columnCount; i++)
            {
                colDef[i] = new ColumnDefinition();
                colDef[i].MaxWidth = 30;
                addedColors.ColumnDefinitions.Add(colDef[i]);
            }
            for (int j = 0; j < rowCount; j++)
            {
                rowDef[j] = new RowDefinition();
                rowDef[j].MinHeight = 30;
                addedColors.RowDefinitions.Add(rowDef[j]);
            }

            Border addedColorBorder;
            int colorIndex = 0;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    addedColorBorder = new Border();
                    addedColorBorder.Width = addedColorBorder.Height = 20;
                    if (colorIndex < colorList.Count)
                    {
                        addedColorBorder.BorderBrush = Brushes.Gray;
                        addedColorBorder.Background = new SolidColorBrush(colorList.ElementAt(colorIndex++));
                    }

                    addedColorBorder.BorderThickness = new Thickness(2);
                    Grid.SetColumn(addedColorBorder, j);
                    Grid.SetRow(addedColorBorder, i);
                    addedColorBorder.PreviewMouseLeftButtonDown += SetSelectedColorForRemovingCommand;

                    InputGesture mouseGesture = new MouseGesture(MouseAction.LeftDoubleClick);
                    InputBinding mouseBinding = new InputBinding(RemoveColorCommand, mouseGesture);
                    addedColorBorder.InputBindings.Add(mouseBinding);

                    addedColors.Children.Add(addedColorBorder);
                }
            }

            return addedColors;
        }

        private WriteableBitmap SetWriteableBitmap(string imageFilename)
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(imageFilename));
            WriteableBitmap wbImage = new WriteableBitmap((BitmapSource)image.Source);

            return wbImage;
        }
    }

    public class ArrayToGridIndexConverter
    {
        private int defaultColumnCount;
        private int defaultRowCount;

        public ArrayToGridIndexConverter(int defaultColumnCount, int defaultRowCount)
        {
            this.defaultColumnCount = defaultColumnCount;
            this.defaultRowCount = defaultRowCount;
        }

        public GridIndex GetGridSize(int arrayLength)
        {
            GridIndex gridSize = ConvertArrayIndexToGrid(arrayLength - 1);
            int columnCount = gridSize.column;
            int rowCount = gridSize.row;

            if (arrayLength > defaultColumnCount)
                columnCount = defaultColumnCount;
            else
                columnCount++;

            if (++rowCount < defaultRowCount) rowCount = defaultRowCount;

            return new GridIndex { column = columnCount, row = rowCount };
        }

        public GridIndex ConvertArrayIndexToGrid(int arrayIndex)
        {
            int column = arrayIndex % defaultColumnCount;
            int row = arrayIndex / defaultColumnCount;
            return new GridIndex { column = column, row = row };
        }
    }

    public struct GridIndex
    {
        public int column;
        public int row;
    }
}
