using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace jwldnr.VisualLinter.ViewModels
{
    internal class OptionsDialogViewModel : ViewModelBase
    {
        public ICommand SuggestNewFeaturesCommand
        {
            get => _suggestNewFeaturesCommand
                ?? (_suggestNewFeaturesCommand = new RelayCommand<string>(SuggestNewFeatures));
            set => _suggestNewFeaturesCommand = value;
        }

        internal bool DisableIgnorePath
        {
            get => GetPropertyValue<bool>(DisableIgnorePathProperty);
            set => SetPropertyValue(DisableIgnorePathProperty, value);
        }

        internal bool UseGlobalEslint
        {
            get => GetPropertyValue<bool>(UseGlobalEslintProperty);
            set => SetPropertyValue(UseGlobalEslintProperty, value);
        }

        internal bool UsePersonalConfig
        {
            get => GetPropertyValue<bool>(UsePersonalConfigProperty);
            set => SetPropertyValue(UsePersonalConfigProperty, value);
        }

        internal static readonly DependencyProperty DisableIgnorePathProperty =
            DependencyProperty.Register(
                nameof(DisableIgnorePath),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(default(bool)));

        internal static readonly DependencyProperty UseGlobalEslintProperty =
            DependencyProperty.Register(
                nameof(UseGlobalEslint),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(default(bool)));

        internal static readonly DependencyProperty UsePersonalConfigProperty =
            DependencyProperty.Register(
                nameof(UsePersonalConfig),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(default(bool)));

        private readonly IVisualLinterOptions _options;
        private ICommand _suggestNewFeaturesCommand;

        internal OptionsDialogViewModel()
        {
            _options = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>()
                ?? throw new Exception("fatal: unable to retrieve options");
        }

        internal void Apply()
        {
            _options.UseGlobalEslint = UseGlobalEslint;
            _options.UsePersonalConfig = UsePersonalConfig;
            _options.DisableIgnorePath = DisableIgnorePath;
        }

        internal void Initiailize()
        {
            LoadOptions();
            RaiseAllChanged();
        }

        private static void SuggestNewFeatures(object url)
        {
            try
            {
                Process.Start(new ProcessStartInfo($"{url}"));
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }
        }

        private void LoadOptions()
        {
            UseGlobalEslint = _options.UseGlobalEslint;
            UsePersonalConfig = _options.UsePersonalConfig;
            DisableIgnorePath = _options.DisableIgnorePath;
        }

        private void RaiseAllChanged()
        {
            RaisePropertyChanged(nameof(UseGlobalEslint));
            RaisePropertyChanged(nameof(UsePersonalConfig));
            RaisePropertyChanged(nameof(DisableIgnorePath));
        }
    }
}