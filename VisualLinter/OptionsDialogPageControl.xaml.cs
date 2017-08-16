using System.Windows.Controls;

namespace jwldnr.VisualLinter
{
    /// <summary>
    ///     Interaction logic for OptionsDialogPageControl.xaml
    /// </summary>
    public partial class OptionsDialogPageControl
    {
        internal bool UseGlobalConfig
        {
            get => UseGlobalConfigCheckBox.IsChecked ?? false;
            set => UseGlobalConfigCheckBox.IsChecked = value;
        }

        internal OptionsDialogPageControl()
        {
            InitializeComponent();
        }
    }
}