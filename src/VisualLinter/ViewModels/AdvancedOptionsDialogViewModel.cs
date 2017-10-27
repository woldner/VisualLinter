using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace jwldnr.VisualLinter.ViewModels
{
    internal class AdvancedOptionsDialogViewModel : ViewModelBase
    {
        internal static readonly DependencyProperty EslintConfigOverridePathProperty =
            DependencyProperty.Register(
                nameof(EslintConfigOverridePath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        internal static readonly DependencyProperty EslintIgnoreOverridePathProperty =
            DependencyProperty.Register(
                nameof(EslintIgnoreOverridePath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        internal static readonly DependencyProperty EslintOverridePathProperty =
            DependencyProperty.Register(
                nameof(EslintOverridePath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        internal static readonly DependencyProperty ShouldOverrideEslintConfigProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideEslintConfig),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty ShouldOverrideEslintIgnoreProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideEslintIgnore),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty ShouldOverrideEslintProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideEslint),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        private readonly IVisualLinterOptions _options;

        private ICommand _browseEslintConfigFileCommand;
        private ICommand _browseEslintFileCommand;
        private ICommand _browseEslintIgnoreConfigFileCommand;

        public ICommand BrowseEslintConfigFileCommand
        {
            get => _browseEslintConfigFileCommand
                ?? (_browseEslintConfigFileCommand = new RelayCommand(BrowseEslintConfigFile));
            set => _browseEslintConfigFileCommand = value;
        }

        public ICommand BrowseEslintFileCommand
        {
            get => _browseEslintFileCommand
                ?? (_browseEslintFileCommand = new RelayCommand(BrowseEslintFile));
            set => _browseEslintFileCommand = value;
        }

        public ICommand BrowseEslintIgnoreFileCommand
        {
            get => _browseEslintIgnoreConfigFileCommand
                ?? (_browseEslintIgnoreConfigFileCommand = new RelayCommand(BrowseEslintIgnoreFile));
            set => _browseEslintIgnoreConfigFileCommand = value;
        }

        internal string EslintConfigOverridePath
        {
            get => GetPropertyValue<string>(EslintConfigOverridePathProperty);
            set => SetPropertyValue(EslintConfigOverridePathProperty, value);
        }

        internal string EslintIgnoreOverridePath
        {
            get => GetPropertyValue<string>(EslintIgnoreOverridePathProperty);
            set => SetPropertyValue(EslintIgnoreOverridePathProperty, value);
        }

        internal string EslintOverridePath
        {
            get => GetPropertyValue<string>(EslintOverridePathProperty);
            set => SetPropertyValue(EslintOverridePathProperty, value);
        }

        internal bool ShouldOverrideEslint
        {
            get => GetPropertyValue<bool>(ShouldOverrideEslintProperty);
            set => SetPropertyValue(ShouldOverrideEslintProperty, value);
        }

        internal bool ShouldOverrideEslintConfig
        {
            get => GetPropertyValue<bool>(ShouldOverrideEslintConfigProperty);
            set => SetPropertyValue(ShouldOverrideEslintConfigProperty, value);
        }

        internal bool ShouldOverrideEslintIgnore
        {
            get => GetPropertyValue<bool>(ShouldOverrideEslintIgnoreProperty);
            set => SetPropertyValue(ShouldOverrideEslintIgnoreProperty, value);
        }

        internal AdvancedOptionsDialogViewModel()
        {
            _options = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>()
                ?? throw new Exception("fatal: unable to retrieve options");
        }

        internal void Apply()
        {
            _options.ShouldOverrideEslint = ShouldOverrideEslint;
            _options.EslintOverridePath = EslintOverridePath;

            _options.ShouldOverrideEslintConfig = ShouldOverrideEslintConfig;
            _options.EslintConfigOverridePath = EslintConfigOverridePath;

            _options.ShouldOverrideEslintIgnore = ShouldOverrideEslintIgnore;
            _options.EslintIgnoreOverridePath = EslintIgnoreOverridePath;
        }

        internal void Initiailize()
        {
            LoadOptions();

            RaiseAllPropertiesChanged();
        }

        private static string GetDialogValue(string filter, string initialDirectory)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    CheckFileExists = true,
                    Filter = filter,
                    InitialDirectory = initialDirectory
                };

                var result = dialog.ShowDialog() ?? false;

                return false == result
                    ? null
                    : dialog.FileName;
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine("error: unable to open file browse dialog:");
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
        }

        private void BrowseEslintConfigFile()
        {
            const string filter = "(*.js, *.yaml, *.yml, *.json, *.eslintrc)|*.cmd;*.exe;*.yaml;*.yml;*.json;*.eslintrc";
            var initialDirectory = Path.GetDirectoryName(EslintConfigOverridePath.NullIfEmpty())
                ?? EnvironmentHelper.GetUserDirectoryPath();

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            EslintConfigOverridePath = value;
        }

        private void BrowseEslintFile()
        {
            const string filter = "(*.cmd, *.exe)|*.cmd;*.exe";
            var initialDirectory = Path.GetDirectoryName(EslintOverridePath.NullIfEmpty())
                ?? EnvironmentHelper.GetUserDirectoryPath();

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            EslintOverridePath = value;
        }

        private void BrowseEslintIgnoreFile()
        {
            const string filter = "(*.eslintignore)|*.eslintignore";
            var initialDirectory = Path.GetDirectoryName(EslintIgnoreOverridePath.NullIfEmpty())
                ?? EnvironmentHelper.GetUserDirectoryPath();

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            EslintIgnoreOverridePath = value;
        }

        private void LoadOptions()
        {
            ShouldOverrideEslint = _options.ShouldOverrideEslint;
            EslintOverridePath = _options.EslintOverridePath;

            ShouldOverrideEslintConfig = _options.ShouldOverrideEslintConfig;
            EslintConfigOverridePath = _options.EslintConfigOverridePath;

            ShouldOverrideEslintIgnore = _options.ShouldOverrideEslintIgnore;
            EslintIgnoreOverridePath = _options.EslintIgnoreOverridePath;
        }

        private void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged(nameof(ShouldOverrideEslint));
            RaisePropertyChanged(nameof(EslintOverridePath));

            RaisePropertyChanged(nameof(ShouldOverrideEslintConfig));
            RaisePropertyChanged(nameof(EslintConfigOverridePath));

            RaisePropertyChanged(nameof(ShouldOverrideEslintIgnore));
            RaisePropertyChanged(nameof(EslintIgnoreOverridePath));
        }
    }
}