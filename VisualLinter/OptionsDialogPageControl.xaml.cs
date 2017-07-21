using System.Windows.Controls;

namespace jwldnr.VisualLinter
{
    /// <summary>
    ///     Interaction logic for OptionsDialogPageControl.xaml
    /// </summary>
    public partial class OptionsDialogPageControl
    {
        internal bool UseLocalConfig
        {
            get => UseLocalConfigCheckBox.IsChecked ?? false;
            set => UseLocalConfigCheckBox.IsChecked = value;
        }

        internal OptionsDialogPageControl()
        {
            InitializeComponent();
        }
    }
}