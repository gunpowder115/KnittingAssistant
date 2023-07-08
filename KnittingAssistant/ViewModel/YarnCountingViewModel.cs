using KnittingAssistant.Model;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KnittingAssistant.ViewModel
{
    public class YarnCountingViewModel : ViewModelBase
    {
        private const int YARN_COUNTING_GRID_WIDTH = 2;

        private ColorStorage colorStorage;

        #region Dependency Properties

        private Grid colorYarnCounters;
        public Grid ColorYarnCounters
        {
            get { return colorYarnCounters; }
            set
            {
                colorYarnCounters = value;
                OnPropertyChanged(nameof(ColorYarnCounters));
            }
        }

        #endregion

        #region Relay Commands



        #endregion

        public YarnCountingViewModel(ColorStorage colorStorage)
        {
            this.colorStorage = colorStorage;

            ColorYarnCounters = CreateYarnCountingGrid();
        }

        private Grid CreateYarnCountingGrid()
        {
            Grid yarnCountingGrid = new Grid();

            int columnCount = YARN_COUNTING_GRID_WIDTH;
            int rowCount = colorStorage.CountingColorsCount + 1;
            ColumnDefinition[] colDef = new ColumnDefinition[columnCount];
            RowDefinition[] rowDef = new RowDefinition[rowCount];
            for (int i = 0; i < columnCount; i++)
            {
                colDef[i] = new ColumnDefinition();
                yarnCountingGrid.ColumnDefinitions.Add(colDef[i]);
            }
            colDef[1].Width = new GridLength(90);
            for (int j = 0; j < rowCount; j++)
            {
                rowDef[j] = new RowDefinition();
                yarnCountingGrid.RowDefinitions.Add(rowDef[j]);
            }
            rowDef[0].Height = new GridLength(50);

            TextBlock textBlock = new TextBlock();
            textBlock.Text = "Цвет";
            Grid.SetColumn(textBlock, 0);
            Grid.SetRow(textBlock, 0);
            yarnCountingGrid.Children.Add(textBlock);

            textBlock = new TextBlock();
            textBlock.Text = "Кол-во\nфрагментов";
            textBlock.TextAlignment = TextAlignment.Center;
            Grid.SetColumn(textBlock, 1);
            Grid.SetRow(textBlock, 0);
            yarnCountingGrid.Children.Add(textBlock);

            Border colorBorder;
            for (int i = 0; i < colorStorage.CountingColorsCount; i++)
            {
                colorBorder = new Border();
                textBlock = new TextBlock();
                var keys = colorStorage.ColorsCounting.Keys;
                Color color = colorStorage.ColorsCounting.ElementAt(i).Key;
                colorBorder.Width = colorBorder.Height = 20;
                colorBorder.BorderBrush = Brushes.Gray;
                colorBorder.Background = new SolidColorBrush(color);
                colorBorder.BorderThickness = new Thickness(2);

                int colorCount = colorStorage.ColorsCounting.ContainsKey(color) ? colorStorage.ColorsCounting[color] : 0;
                textBlock.Text = colorCount.ToString();

                Grid.SetColumn(colorBorder, 0);
                Grid.SetRow(colorBorder, i + 1);
                yarnCountingGrid.Children.Add(colorBorder);

                Grid.SetColumn(textBlock, 1);
                Grid.SetRow(textBlock, i + 1);
                yarnCountingGrid.Children.Add(textBlock);
            }

            yarnCountingGrid.ShowGridLines = true;

            return yarnCountingGrid;
        }
    }

    public class ColorYarnCounter
    {
        public Border ColorBorder { get; }
        public int ColorCount { get; }

        public ColorYarnCounter(Border colorBorder, int colorCount)
        {
            ColorBorder = colorBorder;
            ColorCount = colorCount;
        }
    }
}
