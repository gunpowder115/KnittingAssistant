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

        private bool m_IsColorRemoving;
        public bool IsColorRemoving
        {
            get { return m_IsColorRemoving; }
            set
            {
                m_IsColorRemoving = value;
                OnPropertyChanged("IsColorRemoving");
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

        #endregion

        private const int addedColorsGridWidth = 11;
        private const int addedColorsGridHeight = 3;
        private ColorStorage colorStorage;
        private Color selectedColorForAdding;
        private int selectedColorIndex;
        private LinkedListNode<Color> selectedLinkedListNode;
        private ArrayToGridIndexConverter arrayToGridIndexConverter;
        public Image PaletteAreaImage { get; set; }
        public Border SelectedColor { get; set; }

        public ColorsViewModel()
        {
            RedSelectedColorValue = 0;
            GreenSelectedColorValue = 0;
            BlueSelectedColorValue = 0;

            IsColorAdding = false;
            IsColorRemoving = false;

            arrayToGridIndexConverter = new ArrayToGridIndexConverter(addedColorsGridWidth, addedColorsGridHeight);

            colorStorage = new ColorStorage();
            colorStorage.ReadColorsFromFile();

            AddedColors = CreateColorGrid(colorStorage.ColorList);
        }

        public void SetSelectedColorForAddingCommand(object sender, MouseButtonEventArgs e)
        {
            PaletteAreaImage.Source = new BitmapImage(new Uri("D:/Development/KnittingAssistant/KnittingAssistant/View/resources/square_palette_image.png"));

            BitmapImage tempBitmap = (BitmapImage)PaletteAreaImage.Source;
            WriteableBitmap bitmapPalette = new WriteableBitmap(tempBitmap.PixelWidth, tempBitmap.PixelHeight,
                tempBitmap.DpiX, tempBitmap.DpiY,
                tempBitmap.Format, tempBitmap.Palette);

            Point position = e.GetPosition(PaletteAreaImage);
            if (position.X <= bitmapPalette.PixelWidth && position.X >= 0 && position.Y <= bitmapPalette.PixelHeight && position.Y >= 0)
            {
                int stride = (int)bitmapPalette.PixelWidth * bitmapPalette.Format.BitsPerPixel / 8;

                byte[] currentPixel = new byte[4];
                tempBitmap.CopyPixels(new Int32Rect((int)position.X, (int)position.Y, 1, 1), currentPixel, stride, 0);

                BlueSelectedColorValue = currentPixel[0];
                GreenSelectedColorValue = currentPixel[1];
                RedSelectedColorValue = currentPixel[2];

                selectedColorForAdding = Color.FromRgb((byte)RedSelectedColorValue, (byte)GreenSelectedColorValue, (byte)BlueSelectedColorValue);

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
            SelectedColor.Background = new SolidColorBrush(selectedColor);
            BlueSelectedColorValue = selectedColor.B;
            GreenSelectedColorValue = selectedColor.G;
            RedSelectedColorValue = selectedColor.R;

            if (SelectedColor.Child != null)
                SelectedColor.Child = null;
        }

        public void ShowSelectedColor(LinkedListNode<Color> selectedColorNode)
        {
            if (selectedColorNode != null)
                ShowSelectedColor(selectedColorNode.Value);
            else
            {
                Image emptyColorImage = new Image();
                emptyColorImage.Source = new BitmapImage(new Uri("D:/Development/KnittingAssistant/KnittingAssistant/View/resources/large_empty_image.png"));
                SelectedColor.Child = emptyColorImage;
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
