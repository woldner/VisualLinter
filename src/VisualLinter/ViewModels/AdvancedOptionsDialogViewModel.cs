using jwldnr.VisualLinter.Helpers;
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

        internal static readonly DependencyProperty ShowDebugInformationProperty =
            DependencyProperty.Register(
                nameof(ShowDebugInformation),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        private ICommand _browseEslintConfigFileCommand;
        private ICommand _browseEslintFileCommand;
        private ICommand _browseEslintIgnoreConfigFileCommand;

        public ICommand BrowseEslintConfigFileCommand
        {
            get => _browseEslintConfigFileCommand ??
                (_browseEslintConfigFileCommand = new RelayCommand(BrowseEslintConfigFile));
            set => _browseEslintConfigFileCommand = value;
        }

        public ICommand BrowseEslintFileCommand
        {
            get => _browseEslintFileCommand ??
                (_browseEslintFileCommand = new RelayCommand(BrowseEslintFile));
            set => _browseEslintFileCommand = value;
        }

        public ICommand BrowseEslintIgnoreFileCommand
        {
            get => _browseEslintIgnoreConfigFileCommand ??
                (_browseEslintIgnoreConfigFileCommand = new RelayCommand(BrowseEslintIgnoreFile));
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

        internal bool ShowDebugInformation
        {
            get => GetPropertyValue<bool>(ShowDebugInformationProperty);
            set => SetPropertyValue(ShowDebugInformationProperty, value);
        }

        internal void Load()
        {
            ShouldOverrideEslint = Options.ShouldOverrideEslint;
            EslintOverridePath = Options.EslintOverridePath;
            ShouldOverrideEslintConfig = Options.ShouldOverrideEslintConfig;
            EslintConfigOverridePath = Options.EslintConfigOverridePath;
            ShouldOverrideEslintIgnore = Options.ShouldOverrideEslintIgnore;
            EslintIgnoreOverridePath = Options.EslintIgnoreOverridePath;
            ShowDebugInformation = Options.ShowDebugInformation;
        }

        internal void Save()
        {
            Options.ShouldOverrideEslint = ShouldOverrideEslint;
            Options.EslintOverridePath = EslintOverridePath;
            Options.ShouldOverrideEslintConfig = ShouldOverrideEslintConfig;
            Options.EslintConfigOverridePath = EslintConfigOverridePath;
            Options.ShouldOverrideEslintIgnore = ShouldOverrideEslintIgnore;
            Options.EslintIgnoreOverridePath = EslintIgnoreOverridePath;
            Options.ShowDebugInformation = ShowDebugInformation;
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
                OutputWindowHelper.WriteLine("exception: unable to open file browse dialog");
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
        }

        private void BrowseEslintConfigFile()
        {
            const string filter = "(*.js, *.yaml, *.yml, *.json, *.eslintrc)|*.cmd;*.exe;*.yaml;*.yml;*.json;*.eslintrc";

            var initialDirectory = string.IsNullOrEmpty(EslintConfigOverridePath)
                ? EnvironmentHelper.GetUserDirectoryPath()
                : Path.GetDirectoryName(EslintConfigOverridePath);

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            EslintConfigOverridePath = value;
        }

        private void BrowseEslintFile()
        {
            const string filter = "(*.cmd, *.exe)|*.cmd;*.exe";

            var initialDirectory = string.IsNullOrEmpty(EslintOverridePath)
                ? EnvironmentHelper.GetUserDirectoryPath()
                : Path.GetDirectoryName(EslintOverridePath);

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            EslintOverridePath = value;
        }

        private void BrowseEslintIgnoreFile()
        {
            const string filter = "(*.eslintignore)|*.eslintignore";

            var initialDirectory = string.IsNullOrEmpty(EslintIgnoreOverridePath)
                ? EnvironmentHelper.GetUserDirectoryPath()
                : Path.GetDirectoryName(EslintIgnoreOverridePath);

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            EslintIgnoreOverridePath = value;
        }
    }
}
