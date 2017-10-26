using jwldnr.VisualLinter.ViewModels;

namespace jwldnr.VisualLinter.Views
{
    /// <summary>
    /// Interaction logic for OptionsDialogPageControl.xaml
    /// </summary>
    public partial class OptionsDialogPageControl
    {
        internal OptionsDialogViewModel ViewModel = _viewModel
            ?? (_viewModel = new OptionsDialogViewModel());

        private static OptionsDialogViewModel _viewModel;

        public OptionsDialogPageControl()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }
    }
}