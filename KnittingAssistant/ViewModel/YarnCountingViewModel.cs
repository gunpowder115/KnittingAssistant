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
            int rowCount = colorStorage.ColorsCount;
            ColumnDefinition[] colDef = new ColumnDefinition[columnCount];
            RowDefinition[] rowDef = new RowDefinition[rowCount + 1];
            for (int i = 0; i < columnCount; i++)
            {
                colDef[i] = new ColumnDefinition();
                //colDef[i].MaxWidth = 60;
                yarnCountingGrid.ColumnDefinitions.Add(colDef[i]);
            }
            for (int j = 0; j < rowCount; j++)
            {
                rowDef[j] = new RowDefinition();
                //rowDef[j].MaxHeight = 30;
                yarnCountingGrid.RowDefinitions.Add(rowDef[j]);
            }

            Border colorBorder;
            for (int i = 0; i < colorStorage.ColorsCount; i++)
            {
                colorBorder = new Border();
                TextBlock textBlock = new TextBlock();
                Color color = colorStorage.ColorList.ElementAt(i);
                //colorBorder.Width = colorBorder.Height = 20;
                //colorBorder.BorderBrush = Brushes.Gray;
                colorBorder.Background = new SolidColorBrush(color);
                //colorBorder.BorderThickness = new Thickness(2);

                int colorCount = colorStorage.ColorsCounting.ContainsKey(color) ? colorStorage.ColorsCounting[color] : 0;
                textBlock.Text = colorCount.ToString();
                //textBlock.FontSize = 14;
                //textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                //textBlock.VerticalAlignment = VerticalAlignment.Center;

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

        //private ColorYarnCounter[] CreateYarnCountingGrid()
        //{
        //    ColorYarnCounter[] colorYarnCounters = new ColorYarnCounter[colorStorage.ColorsCount];

        //    for (int i = 0; i < colorStorage.ColorsCount; i++)
        //    {
        //        Border colorBorder = new Border();
        //        Color color = colorStorage.ColorList.ElementAt(i);
        //        colorBorder.Width = colorBorder.Height = 20;
        //        colorBorder.BorderBrush = Brushes.Gray;
        //        colorBorder.Background = new SolidColorBrush(color);
        //        colorBorder.BorderThickness = new Thickness(2);

        //        int colorCount = colorStorage.ColorsCounting.ContainsKey(color) ? colorStorage.ColorsCounting[color] : 0;

        //        colorYarnCounters[i] = new ColorYarnCounter(colorBorder, colorCount);
        //    }

        //    return colorYarnCounters;
        //}
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
