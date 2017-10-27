using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace jwldnr.VisualLinter.ViewModels
{
    internal class OptionsDialogViewModel : ViewModelBase
    {
        internal static readonly DependencyProperty DisableEslintIgnoreProperty =
            DependencyProperty.Register(
                nameof(DisableEslintIgnore),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableHtmlLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableHtmlLanguageSupport),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableJsLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableJsLanguageSupport),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(true));

        internal static readonly DependencyProperty EnableReactLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableReactLanguageSupport),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableVueLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableVueLanguageSupport),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty UseGlobalEslintProperty =
            DependencyProperty.Register(
                nameof(UseGlobalEslint),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty UsePersonalConfigProperty =
            DependencyProperty.Register(
                nameof(UsePersonalConfig),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty OverrideGlobalEslintPathProperty =
            DependencyProperty.Register(
                nameof(OverrideGlobalEslintPath),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty OverridePersonalConfigPathProperty =
            DependencyProperty.Register(
                nameof(OverridePersonalConfigPath),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty GlobalEslintOverridePathProperty =
            DependencyProperty.Register(
                nameof(GlobalEslintOverridePath),
                typeof(string),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(null));

        internal static readonly DependencyProperty PersonalConfigOverridePathProperty =
            DependencyProperty.Register(
                nameof(PersonalConfigOverridePath),
                typeof(string),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(null));

        private readonly IVisualLinterOptions _options;

        private ICommand _suggestNewFeaturesCommand;
        private ICommand _browseGlobalEslintFileCommand;
        private ICommand _browsePersonalConfigFileCommand;

        public ICommand SuggestNewFeaturesCommand
        {
            get => _suggestNewFeaturesCommand ?? (_suggestNewFeaturesCommand = new RelayCommand<string>(SuggestNewFeatures));
            set => _suggestNewFeaturesCommand = value;
        }

        public ICommand BrowseGlobalEslintFileCommand
        {
            get => _browseGlobalEslintFileCommand ?? (_browseGlobalEslintFileCommand = new RelayCommand(BrowseGlobalEslintFile));
            set => _browseGlobalEslintFileCommand = value;
        }

        public ICommand BrowsePersonalConfigFileCommand
        {
            get => _browsePersonalConfigFileCommand ?? (_browsePersonalConfigFileCommand = new RelayCommand(BrowsePersonalConfigFile));
            set => _browsePersonalConfigFileCommand = value;
        }

        internal bool DisableEslintIgnore
        {
            get => GetPropertyValue<bool>(DisableEslintIgnoreProperty);
            set => SetPropertyValue(DisableEslintIgnoreProperty, value);
        }

        internal bool EnableHtmlLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableHtmlLanguageSupportProperty);
            set => SetPropertyValue(EnableHtmlLanguageSupportProperty, value);
        }

        internal bool EnableJsLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableJsLanguageSupportProperty);
            set => SetPropertyValue(EnableJsLanguageSupportProperty, value);
        }

        internal bool EnableReactLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableReactLanguageSupportProperty);
            set => SetPropertyValue(EnableReactLanguageSupportProperty, value);
        }

        internal bool EnableVueLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableVueLanguageSupportProperty);
            set => SetPropertyValue(EnableVueLanguageSupportProperty, value);
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

        internal bool OverrideGlobalEslintPath
        {
            get => GetPropertyValue<bool>(OverrideGlobalEslintPathProperty);
            set => SetPropertyValue(OverrideGlobalEslintPathProperty, value);
        }

        internal bool OverridePersonalConfigPath
        {
            get => GetPropertyValue<bool>(OverridePersonalConfigPathProperty);
            set => SetPropertyValue(OverridePersonalConfigPathProperty, value);
        }

        internal string GlobalEslintOverridePath
        {
            get => GetPropertyValue<string>(GlobalEslintOverridePathProperty);
            set => SetPropertyValue(GlobalEslintOverridePathProperty, value);
        }

        internal string PersonalConfigOverridePath
        {
            get => GetPropertyValue<string>(PersonalConfigOverridePathProperty);
            set => SetPropertyValue(PersonalConfigOverridePathProperty, value);
        }

        internal OptionsDialogViewModel()
        {
            _options = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>()
                ?? throw new Exception("fatal: unable to retrieve options");
        }

        internal void Apply()
        {
            _options.DisableIgnorePath = DisableEslintIgnore;
            _options.EnableHtmlLanguageSupport = EnableHtmlLanguageSupport;
            _options.EnableJsLanguageSupport = EnableJsLanguageSupport;
            _options.EnableReactLanguageSupport = EnableReactLanguageSupport;
            _options.EnableVueLanguageSupport = EnableVueLanguageSupport;
            _options.UseGlobalEslint = UseGlobalEslint;
            _options.UsePersonalConfig = UsePersonalConfig;
        }

        internal void Initiailize()
        {
            LoadOptions();
            RaiseAllPropertiesChanged();
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

        private void BrowseGlobalEslintFile()
        {
            try
            {
                var dir = Path.GetDirectoryName(GlobalEslintOverridePath.NullIfEmpty())
                    ?? EnvironmentHelper.GetUserDirectoryPath();

                var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = "(*.cmd, *.exe)|*.cmd;*.exe",
                    InitialDirectory = dir
                };

                var result = dialog.ShowDialog() ?? false;
                if (false == result)
                    return;

                GlobalEslintOverridePath = dialog.FileName;
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine("error: unable to open global eslint file browse dialog:");
                OutputWindowHelper.WriteLine(e.Message);
            }
        }

        private void BrowsePersonalConfigFile()
        {
            try
            {
                var dir = Path.GetDirectoryName(PersonalConfigOverridePath.NullIfEmpty())
                    ?? EnvironmentHelper.GetUserDirectoryPath();

                var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = "(*.js, *.yaml, *.yml, *.json, *.eslintrc)|*.cmd;*.exe;*.yaml;*.yml;*.json;*.eslintrc",
                    InitialDirectory = dir
                };

                var result = dialog.ShowDialog() ?? false;
                if (false == result)
                    return;

                PersonalConfigOverridePath = dialog.FileName;
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine("error: unable to open personal config file browse dialog:");
                OutputWindowHelper.WriteLine(e.Message);
            }
        }

        private void LoadOptions()
        {
            UseGlobalEslint = _options.UseGlobalEslint;
            EnableHtmlLanguageSupport = _options.EnableHtmlLanguageSupport;
            EnableJsLanguageSupport = _options.EnableJsLanguageSupport;
            EnableReactLanguageSupport = _options.EnableReactLanguageSupport;
            EnableVueLanguageSupport = _options.EnableVueLanguageSupport;
            UsePersonalConfig = _options.UsePersonalConfig;
            DisableEslintIgnore = _options.DisableIgnorePath;
        }

        private void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged(nameof(UseGlobalEslint));
            RaisePropertyChanged(nameof(EnableHtmlLanguageSupport));
            RaisePropertyChanged(nameof(EnableJsLanguageSupport));
            RaisePropertyChanged(nameof(EnableReactLanguageSupport));
            RaisePropertyChanged(nameof(EnableVueLanguageSupport));
            RaisePropertyChanged(nameof(UsePersonalConfig));
            RaisePropertyChanged(nameof(DisableEslintIgnore));
        }
    }
}