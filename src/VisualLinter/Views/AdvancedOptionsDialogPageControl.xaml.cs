using jwldnr.VisualLinter.ViewModels;

namespace jwldnr.VisualLinter.Views
{
    /// <summary>
    /// Interaction logic for AdvancedOptionsDialogPageControl.xaml
    /// </summary>
    public partial class AdvancedOptionsDialogPageControl
    {
        internal AdvancedOptionsDialogViewModel ViewModel = _viewModel
            ?? (_viewModel = new AdvancedOptionsDialogViewModel());

        private static AdvancedOptionsDialogViewModel _viewModel;

        public AdvancedOptionsDialogPageControl()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }
    }
}