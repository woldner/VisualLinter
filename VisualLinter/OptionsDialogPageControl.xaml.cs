using jwldnr.VisualLinter.Helpers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

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

        internal bool UseGlobalLinter
        {
            get => UseGlobalLinterCheckBox.IsChecked ?? false;
            set => UseGlobalLinterCheckBox.IsChecked = value;
        }

        internal OptionsDialogPageControl()
        {
            InitializeComponent();
        }

        private void SuggestFeatures_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }

        private void UseGlobalConfig_OnClick(object sender, RoutedEventArgs e)
        {
            OutputWindowHelper.WriteLine($"use global config option set to {UseGlobalConfig}");

            e.Handled = true;
        }

        private void UseGlobalLinter_OnClick(object sender, RoutedEventArgs e)
        {
            OutputWindowHelper.WriteLine($"use global linter option set to {UseGlobalLinter}");

            e.Handled = true;
        }
    }
}