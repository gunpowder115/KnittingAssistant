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

        #endregion

        #region Relay Commands


        #endregion

        private const int addedColorsGridWidth = 12;
        private const int addedColorsGridHeight = 6;
        public Image PaletteAreaImage { get; set; }
        private RenderTargetBitmap bitmapImage;

        public ColorsViewModel()
        {
            InitAddedColorsGrid();
        }

        public void SetSelectedColorCommand(object sender, MouseButtonEventArgs e)
        {

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
                    addedColorBorder.BorderThickness = new System.Windows.Thickness(2);
                    Grid.SetColumn(addedColorBorder, i);
                    Grid.SetRow(addedColorBorder, j);
                    AddedColors.Children.Add(addedColorBorder);
                }
            }
        }
    }
}
