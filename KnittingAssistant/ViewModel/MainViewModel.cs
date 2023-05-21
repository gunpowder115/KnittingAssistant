using KnittingAssistant.Model;

namespace KnittingAssistant.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Dependency Properties

        #endregion

        #region Relay Commands

        private RelayCommand pasteMainImageGestureCommand;
        public RelayCommand PasteMainImageGestureCommand
        {
            get
            {
                return pasteMainImageGestureCommand ??
                    (pasteMainImageGestureCommand = new RelayCommand(obj =>
                        ImageAreaViewModel.PasteMainImageCommand.Execute(obj)));
            }
        }

        private RelayCommand copyMainImageGestureCommand;
        public RelayCommand CopyMainImageGestureCommand
        {
            get
            {
                return copyMainImageGestureCommand ??
                    (copyMainImageGestureCommand = new RelayCommand(obj =>
                        ImageAreaViewModel.CopyMainImageCommand.Execute(obj)));
            }
        }

        #endregion

        private double FragmentRatio = 1.0;
        private ImageProcessor imageProcessor;

        public PropertyAreaViewModel PropertyAreaViewModel { get; }
        public ImageAreaViewModel ImageAreaViewModel { get; }
        public ToolbarAreaViewModel ToolbarAreaViewModel { get; }

        public MainViewModel()
        {
            imageProcessor = new ImageProcessor();
            PropertyAreaViewModel = new PropertyAreaViewModel(imageProcessor);
            ImageAreaViewModel = new ImageAreaViewModel(imageProcessor);
            ToolbarAreaViewModel = new ToolbarAreaViewModel(imageProcessor);
        }
    }
}
