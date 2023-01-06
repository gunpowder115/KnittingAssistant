using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KnittingAssistant.Model
{
    struct GridIndex
    {
        public int column;
        public int row;
    }

    class ColorStorage
    {
        private string filename = "colors.txt";
        private int defaultColumnCount = 12;
        private int defaultRowCount = 3;
        private MouseButtonEventHandler setSelectedColorForDeleting;

        public ColorStorage(string filename, int defaultColumnCount, int defaultRowCount,
            MouseButtonEventHandler setSelectedColorForDeleting)
        {
            this.filename = filename;
            this.defaultColumnCount = defaultColumnCount;
            this.defaultRowCount = defaultRowCount;
            this.setSelectedColorForDeleting = setSelectedColorForDeleting;
        }

        public void AddColor(Color color)
        {
            StreamWriter sw = new StreamWriter(filename, true);

            string writeableColor = color.R.ToString() + " " + color.G.ToString() + " " + color.B.ToString();
            sw.WriteLine(writeableColor);

            sw.Close();
        }

        public void DeleteColor(int id, bool allColors = false)
        {
            var storageContent = File.ReadAllLines(filename).ToList();
            storageContent.RemoveAt(id);
            File.WriteAllLines(filename, storageContent.ToArray());
        }

        public void ClearColors()
        {
            var storageContent = File.ReadAllLines(filename).ToList();
            storageContent.Clear();
            File.WriteAllLines(filename, storageContent.ToArray());
        }

        public Grid GetColorGridFromFile()
        {
            var storageContent = File.ReadAllLines(filename);
            byte[,] colorBytes = new byte[storageContent.Length, 3];
            Color[] colors = new Color[storageContent.Length];

            for (int i = 0; i < storageContent.Length; i++)
            {
                var colorString = storageContent[i].Split(" ");
                colorBytes[i, 0] = byte.Parse(colorString[0]);
                colorBytes[i, 1] = byte.Parse(colorString[1]);
                colorBytes[i, 2] = byte.Parse(colorString[2]);

                colors[i] = Color.FromRgb(colorBytes[i, 0], colorBytes[i, 1], colorBytes[i, 2]);
            }

            GridIndex gridSize = GetGridSize(storageContent.Length);

            return CreateColorGrid(gridSize.column, gridSize.row, colors);
        }

        private GridIndex ConvertIndexArrayToGrid(int arrayIndex)
        {
            int column = arrayIndex % defaultColumnCount;
            int row = arrayIndex / defaultColumnCount;
            return new GridIndex { column = column, row = row };
        }

        private GridIndex GetGridSize(int arrayLength)
        {
            GridIndex gridSize = ConvertIndexArrayToGrid(arrayLength - 1);
            int columnCount = gridSize.column;
            int rowCount = gridSize.row;

            if (arrayLength > defaultColumnCount)
                columnCount = defaultColumnCount;
            else
                columnCount++;

            if (++rowCount < defaultRowCount) rowCount = defaultRowCount;

            return new GridIndex { column = columnCount, row = rowCount };
        }

        private Grid CreateColorGrid(int columnCount, int rowCount, Color[] colors)
        {
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
                    if (colorIndex < colors.Length)
                    {
                        addedColorBorder.BorderBrush = Brushes.Gray;
                        addedColorBorder.Background = new SolidColorBrush(colors[colorIndex++]);
                    }
                    addedColorBorder.BorderThickness = new Thickness(2);
                    Grid.SetColumn(addedColorBorder, j);
                    Grid.SetRow(addedColorBorder, i);
                    addedColorBorder.PreviewMouseLeftButtonDown += setSelectedColorForDeleting;
                    addedColors.Children.Add(addedColorBorder);
                }
            }

            return addedColors;
        }
    }
}