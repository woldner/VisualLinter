using jwldnr.VisualLinter.Helpers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace jwldnr.VisualLinter
{
    /// <summary>
    /// Interaction logic for OptionsDialogPageControl.xaml
    /// </summary>
    public partial class OptionsDialogPageControl
    {
        internal bool UseGlobalConfig
        {
            get => UsePersonalConfigCheckBox.IsChecked ?? false;
            set => UsePersonalConfigCheckBox.IsChecked = value;
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

        private void SuggestNewFeatures_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void UseGlobalLinter_OnClick(object sender, RoutedEventArgs e)
        {
            var value = UseGlobalLinter.ToString().ToLowerInvariant();
            OutputWindowHelper.WriteLine($"always use global linter option set to: {value}");
        }

        private void UsePersonalConfig_OnClick(object sender, RoutedEventArgs e)
        {
            var value = UseGlobalConfig.ToString().ToLowerInvariant();
            OutputWindowHelper.WriteLine($"use personal config option set to: {value}");
        }
    }
}