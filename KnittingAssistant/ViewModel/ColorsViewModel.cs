using KnittingAssistant.Model;
using KnittingAssistant.View;
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
        public EventHandler CloseHandler;

        private const int ADDED_COLORS_GRID_WIDTH = 11;
        private const int ADDED_COLORS_GRID_HEIGHT = 3;
        private const string PALETTE_FILENAME = "pack://application:,,,/Resources/Images/square_palette_image_full.png";
        private const string EMPTY_SELECTED_COLOR_FILENAME = "pack://application:,,,/Resources/Images/large_empty_image.png";
        private const double BIG_BORDER_SIZE = 30;
        private const double NORMAL_BORDER_SIZE = 20;

        private ColorStorage colorStorage;
        private Color selectedColorForAdding;
        private int selectedColorIndex;
        private LinkedListNode<Color> selectedLinkedListNode;
        private ArrayToGridIndexConverter arrayToGridIndexConverter;
        private Border lastSelectedColorBorder;
        private bool ignoreRgbChanged;
        private SelectedColorModes selectedColorMode;

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

        private bool clearingColorsIsEnabled;
        public bool ClearingColorsIsEnabled
        {
            get { return clearingColorsIsEnabled; }
            set
            {
                clearingColorsIsEnabled = value;
                OnPropertyChanged("ClearingColorsIsEnabled");
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

        private Visibility circleVisibility;
        public Visibility CircleVisibility
        {
            get { return circleVisibility; }
            set
            {
                circleVisibility = value;
                OnPropertyChanged("CircleVisibility");
            }
        }

        private Point circleCenterPoint;
        public Point CircleCenterPoint
        {
            get { return circleCenterPoint; }
            set
            {
                circleCenterPoint = value;
                OnPropertyChanged("CircleCenterPoint");
            }
        }

        private SolidColorBrush circleFillColor;
        public SolidColorBrush CircleFillColor
        {
            get { return circleFillColor; }
            set
            {
                circleFillColor = value;
                OnPropertyChanged("CircleFillColor");
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
                        ClearingColorsIsEnabled = true;
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

                        if (selectedColorIndex == 0 && colorStorage.ColorsCount == 1)
                        {
                            selectedColorIndex = -1;
                            nextSelectedColorLinkedListNode = null;
                            ClearingColorsIsEnabled = false;
                            selectedColorMode = SelectedColorModes.notSelected;
                            SetButtonsEnabling();
                        }
                        else if (selectedColorIndex == colorStorage.ColorsCount - 1)
                        {
                            selectedColorIndex--;
                            nextSelectedColorLinkedListNode = selectedLinkedListNode.Previous;
                        }
                        else
                        {
                            nextSelectedColorLinkedListNode = selectedLinkedListNode.Next;
                        }

                        colorStorage.RemoveColor(selectedLinkedListNode);
                        AddedColors = CreateColorGrid(colorStorage.ColorList);

                        selectedLinkedListNode = nextSelectedColorLinkedListNode;

                        ignoreRgbChanged = true;
                        ShowSelectedColor(selectedLinkedListNode);
                        ignoreRgbChanged = false;
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
                        if (isColorRemoving)
                        {
                            selectedColorMode = SelectedColorModes.notSelected;
                            ignoreRgbChanged = true;
                            ShowSelectedColor(selectedLinkedListNode);
                            ignoreRgbChanged = false;
                        }
                        ClearingColorsIsEnabled = false;
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
                        CloseColorsWindow();
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
                        CloseColorsWindow();
                    }));
            }
        }

        private RelayCommand openYarnCountingWindowCommand;
        public RelayCommand OpenYarnCountingWindowCommand
        {
            get
            {
                return openYarnCountingWindowCommand ??
                    (openYarnCountingWindowCommand = new RelayCommand(obj =>
                    {
                        YarnCountingWindow yarnCountingWindow = new YarnCountingWindow(colorStorage);
                        yarnCountingWindow.ShowDialog();
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
                        if (!ignoreRgbChanged)
                        {
                            selectedColorForAdding = Color.FromRgb((byte)RedSelectedColorValue,
                                (byte)GreenSelectedColorValue, (byte)BlueSelectedColorValue);
                            selectedColorMode = SelectedColorModes.forAdding;
                            ShowSelectedColor(selectedColorForAdding);
                            DisablePaletteCircle();
                            SetButtonsEnabling();

                            lastSelectedColorBorder.Width = lastSelectedColorBorder.Height = NORMAL_BORDER_SIZE;
                            //if (!ignoreRgbChanged)
                            //{
                            //    lastSelectedColorBorder.Width = lastSelectedColorBorder.Height = NormalBorderSize;
                            //}
                        }
                    }));
            }
        }

        #endregion

        public ColorsViewModel()
        {
            PaletteAreaImage = SetWriteableBitmap(PALETTE_FILENAME);
            SelectedColorImage = SetWriteableBitmap(EMPTY_SELECTED_COLOR_FILENAME);
            CircleVisibility = Visibility.Collapsed;
            CircleCenterPoint = new Point();
            CircleFillColor = new SolidColorBrush();
            RedSelectedColorValue = 0;
            GreenSelectedColorValue = 0;
            BlueSelectedColorValue = 0;
            selectedColorMode = SelectedColorModes.notSelected;

            selectedColorIndex = -1;
            lastSelectedColorBorder = new Border();
            arrayToGridIndexConverter = new ArrayToGridIndexConverter(ADDED_COLORS_GRID_WIDTH, ADDED_COLORS_GRID_HEIGHT);
            colorStorage = new ColorStorage();
            colorStorage.ReadColorsFromFile();
            AddedColors = CreateColorGrid(colorStorage.ColorList);
            ClearingColorsIsEnabled = !(colorStorage.ColorsCount == 0);

            SetButtonsEnabling();
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

                lastSelectedColorBorder = new Border();
                selectedColorMode = SelectedColorModes.forAdding;
                ignoreRgbChanged = true;
                ShowSelectedColor(selectedColorForAdding);
                ignoreRgbChanged = false;
                ShowPaletteCircle(position, selectedColorForAdding);
                SetButtonsEnabling();
            }
        }

        public void SetSelectedColorForRemovingCommand(object sender, MouseButtonEventArgs e)
        {
            Border selectedColorBorder = sender as Border;
            selectedColorIndex = (selectedColorBorder.Parent as Grid).Children.IndexOf(selectedColorBorder);
            selectedLinkedListNode = colorStorage.GetNodeByIndex(selectedColorIndex);

            lastSelectedColorBorder.Width = lastSelectedColorBorder.Height = NORMAL_BORDER_SIZE;
            selectedColorBorder.Width = selectedColorBorder.Height = BIG_BORDER_SIZE;
            lastSelectedColorBorder = selectedColorBorder;

            selectedColorMode = SelectedColorModes.forRemoving;
            ignoreRgbChanged = true;
            ShowSelectedColor(selectedLinkedListNode);
            ignoreRgbChanged = false;
            DisablePaletteCircle();
            SetButtonsEnabling();
        }

        public void ShowSelectedColor(Color selectedColor)
        {
            SelectedColorBackground = new SolidColorBrush(selectedColor);
            BlueSelectedColorValue = selectedColor.B;
            GreenSelectedColorValue = selectedColor.G;
            RedSelectedColorValue = selectedColor.R;

            if (SelectedColorImage != null && selectedColorMode != SelectedColorModes.notSelected)
                SelectedColorImage = null;
        }

        public void ShowSelectedColor(LinkedListNode<Color> selectedColorNode)
        {
            if (selectedColorNode != null)
                ShowSelectedColor(selectedColorNode.Value);
            else
            {
                SelectedColorImage = SetWriteableBitmap(EMPTY_SELECTED_COLOR_FILENAME);
                BlueSelectedColorValue = GreenSelectedColorValue = RedSelectedColorValue = 0;
            }
        }

        public void AddColorByDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddColorCommand.Execute(e);
        }

        public void CloseColorsWindow()
        {
            CloseHandler?.Invoke(this, EventArgs.Empty);
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
                    if (colorIndex == selectedColorIndex &&
                        selectedColorMode == SelectedColorModes.forRemoving)
                    {
                        addedColorBorder.Width = addedColorBorder.Height = BIG_BORDER_SIZE;
                        lastSelectedColorBorder = addedColorBorder;
                    }
                    else
                    {
                        addedColorBorder.Width = addedColorBorder.Height = NORMAL_BORDER_SIZE;
                    }

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

        private void ShowPaletteCircle(Point position, Color selectedColorForAdding)
        {
            CircleVisibility = Visibility.Visible;
            CircleCenterPoint = position;
            CircleFillColor = new SolidColorBrush(selectedColorForAdding);
        }

        private void DisablePaletteCircle()
        {
            CircleVisibility = Visibility.Collapsed;
        }

        private void SetButtonsEnabling()
        {
            IsColorAdding = selectedColorMode == SelectedColorModes.forAdding;
            IsColorRemoving = selectedColorMode == SelectedColorModes.forRemoving;
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

    enum SelectedColorModes
    {
        notSelected,
        forAdding,
        forRemoving
    }
}
