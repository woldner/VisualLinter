﻿using jwldnr.VisualLinter.Helpers;
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
        internal bool UseGlobalEslint
        {
            get => UseGlobalEslintCheckBox.IsChecked ?? false;
            set => UseGlobalEslintCheckBox.IsChecked = value;
        }

        internal bool UsePersonalConfig
        {
            get => UsePersonalConfigCheckBox.IsChecked ?? false;
            set => UsePersonalConfigCheckBox.IsChecked = value;
        }

        internal bool DisableIgnorePath
        {
            get => DisableIgnorePathCheckBox.IsChecked ?? false;
            set => DisableIgnorePathCheckBox.IsChecked = value;
        }

        internal OptionsDialogPageControl()
        {
            InitializeComponent();
        }

        private void SuggestNewFeatures_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
        }

        private void UseGlobalEslint_OnClick(object sender, RoutedEventArgs e)
        {
            var value = UseGlobalEslint.ToString().ToLowerInvariant();
            OutputWindowHelper.WriteLine($"use global eslint option set to {value}.");
        }

        private void UsePersonalConfig_OnClick(object sender, RoutedEventArgs e)
        {
            var value = UsePersonalConfig.ToString().ToLowerInvariant();
            OutputWindowHelper.WriteLine($"use personal config option set to {value}.");
        }

        private void DisableIgnorePath_OnClick(object sender, RoutedEventArgs e)
        {
            var value = DisableIgnorePath.ToString().ToLowerInvariant();
            OutputWindowHelper.WriteLine($"don't use .eslintignore when linting files option set to {value}.");
        }
    }
}