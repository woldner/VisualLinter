using jwldnr.VisualLinter.ViewModels;

namespace jwldnr.VisualLinter.Views
{
    /// <summary>
    /// Interaction logic for GeneralOptionsDialogPageControl.xaml
    /// </summary>
    public partial class GeneralOptionsDialogPageControl
    {
        internal GeneralOptionsDialogViewModel ViewModel = _viewModel ??
            (_viewModel = new GeneralOptionsDialogViewModel());

        private static GeneralOptionsDialogViewModel _viewModel;

        public GeneralOptionsDialogPageControl()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }
    }
}
