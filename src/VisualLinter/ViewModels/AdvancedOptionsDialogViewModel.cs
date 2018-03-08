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
        //
        // ESLint
        //

        internal static readonly DependencyProperty ShouldOverrideEslintPathProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideEslintPath),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EslintOverridePathProperty =
            DependencyProperty.Register(
                nameof(EslintOverridePath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        internal static readonly DependencyProperty ShouldOverrideEslintConfigPathProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideEslintConfigPath),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EslintOverrideConfigPathProperty =
            DependencyProperty.Register(
                nameof(EslintOverrideConfigPath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        internal static readonly DependencyProperty ShouldOverrideEslintIgnorePathProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideEslintIgnorePath),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EslintOverrideIgnorePathProperty =
            DependencyProperty.Register(
                nameof(EslintOverrideIgnorePath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        //
        // Stylelint
        //

        internal static readonly DependencyProperty ShouldOverrideStylelintPathProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideStylelintPath),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty StylelintOverridePathProperty =
            DependencyProperty.Register(
                nameof(StylelintOverridePath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        internal static readonly DependencyProperty ShouldOverrideStylelintConfigPathProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideStylelintConfigPath),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty StylelintOverrideConfigPathProperty =
            DependencyProperty.Register(
                nameof(StylelintOverrideConfigPath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        internal static readonly DependencyProperty ShouldOverrideStylelintIgnorePathProperty =
            DependencyProperty.Register(
                nameof(ShouldOverrideStylelintIgnorePath),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty StylelintOverrideIgnorePathProperty =
            DependencyProperty.Register(
                nameof(StylelintOverrideIgnorePath),
                typeof(string),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(string.Empty));

        //
        // Misc
        //

        internal static readonly DependencyProperty ShowDebugInformationProperty =
            DependencyProperty.Register(
                nameof(ShowDebugInformation),
                typeof(bool),
                typeof(AdvancedOptionsDialogViewModel),
                new PropertyMetadata(false));

        //
        // ESLint
        //

        internal bool ShouldOverrideEslintPath
        {
            get => GetPropertyValue<bool>(ShouldOverrideEslintPathProperty);
            set => SetPropertyValue(ShouldOverrideEslintPathProperty, value);
        }

        internal string EslintOverridePath
        {
            get => GetPropertyValue<string>(EslintOverridePathProperty);
            set => SetPropertyValue(EslintOverridePathProperty, value);
        }

        internal bool ShouldOverrideEslintConfigPath
        {
            get => GetPropertyValue<bool>(ShouldOverrideEslintConfigPathProperty);
            set => SetPropertyValue(ShouldOverrideEslintConfigPathProperty, value);
        }

        internal string EslintOverrideConfigPath
        {
            get => GetPropertyValue<string>(EslintOverrideConfigPathProperty);
            set => SetPropertyValue(EslintOverrideConfigPathProperty, value);
        }

        internal bool ShouldOverrideEslintIgnorePath
        {
            get => GetPropertyValue<bool>(ShouldOverrideEslintIgnorePathProperty);
            set => SetPropertyValue(ShouldOverrideEslintIgnorePathProperty, value);
        }

        internal string EslintOverrideIgnorePath
        {
            get => GetPropertyValue<string>(EslintOverrideIgnorePathProperty);
            set => SetPropertyValue(EslintOverrideIgnorePathProperty, value);
        }

        //
        // Stylelint
        //

        internal bool ShouldOverrideStylelintPath
        {
            get => GetPropertyValue<bool>(ShouldOverrideStylelintPathProperty);
            set => SetPropertyValue(ShouldOverrideStylelintPathProperty, value);
        }

        internal string StylelintOverridePath
        {
            get => GetPropertyValue<string>(StylelintOverridePathProperty);
            set => SetPropertyValue(StylelintOverridePathProperty, value);
        }

        internal bool ShouldOverrideStylelintConfigPath
        {
            get => GetPropertyValue<bool>(ShouldOverrideStylelintConfigPathProperty);
            set => SetPropertyValue(ShouldOverrideStylelintConfigPathProperty, value);
        }

        internal string StylelintOverrideConfigPath
        {
            get => GetPropertyValue<string>(StylelintOverrideConfigPathProperty);
            set => SetPropertyValue(StylelintOverrideConfigPathProperty, value);
        }

        internal bool ShouldOverrideStylelintIgnorePath
        {
            get => GetPropertyValue<bool>(ShouldOverrideStylelintIgnorePathProperty);
            set => SetPropertyValue(ShouldOverrideStylelintIgnorePathProperty, value);
        }

        internal string StylelintOverrideIgnorePath
        {
            get => GetPropertyValue<string>(StylelintOverrideIgnorePathProperty);
            set => SetPropertyValue(StylelintOverrideIgnorePathProperty, value);
        }

        //
        // Misc
        //

        internal bool ShowDebugInformation
        {
            get => GetPropertyValue<bool>(ShowDebugInformationProperty);
            set => SetPropertyValue(ShowDebugInformationProperty, value);
        }

        //
        // ESLint
        //

        private ICommand _browseEslintFileCommand;
        private ICommand _browseEslintConfigFileCommand;
        private ICommand _browseEslintIgnoreConfigFileCommand;

        public ICommand BrowseEslintFileCommand
        {
            get => _browseEslintFileCommand ??
                (_browseEslintFileCommand = new RelayCommand(BrowseEslintFile));
            set => _browseEslintFileCommand = value;
        }

        public ICommand BrowseEslintConfigFileCommand
        {
            get => _browseEslintConfigFileCommand ??
                (_browseEslintConfigFileCommand = new RelayCommand(BrowseEslintConfigFile));
            set => _browseEslintConfigFileCommand = value;
        }

        public ICommand BrowseEslintIgnoreFileCommand
        {
            get => _browseEslintIgnoreConfigFileCommand ??
                (_browseEslintIgnoreConfigFileCommand = new RelayCommand(BrowseEslintIgnoreFile));
            set => _browseEslintIgnoreConfigFileCommand = value;
        }

        //
        // Stylelint
        //

        private ICommand _browseStylelintFileCommand;
        private ICommand _browseStylelintConfigFileCommand;
        private ICommand _browseStylelintIgnoreConfigFileCommand;

        public ICommand BrowseStylelintFileCommand
        {
            get => _browseStylelintFileCommand ??
                (_browseStylelintFileCommand = new RelayCommand(BrowseStylelintFile));
            set => _browseStylelintFileCommand = value;
        }

        public ICommand BrowseStylelintConfigFileCommand
        {
            get => _browseStylelintConfigFileCommand ??
                (_browseStylelintConfigFileCommand = new RelayCommand(BrowseStylelintConfigFile));
            set => _browseStylelintConfigFileCommand = value;
        }

        public ICommand BrowseStylelintIgnoreFileCommand
        {
            get => _browseStylelintIgnoreConfigFileCommand ??
                (_browseStylelintIgnoreConfigFileCommand = new RelayCommand(BrowseStylelintIgnoreFile));
            set => _browseStylelintIgnoreConfigFileCommand = value;
        }

        //
        // ESLint
        //

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

        private void BrowseEslintConfigFile()
        {
            const string filter = "(*.js, *.yaml, *.yml, *.json, *.eslintrc)|*.cmd;*.exe;*.yaml;*.yml;*.json;*.eslintrc";

            var initialDirectory = string.IsNullOrEmpty(EslintOverrideConfigPath)
                ? EnvironmentHelper.GetUserDirectoryPath()
                : Path.GetDirectoryName(EslintOverrideConfigPath);

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            EslintOverrideConfigPath = value;
        }

        private void BrowseEslintIgnoreFile()
        {
            const string filter = "(*.eslintignore)|*.eslintignore";

            var initialDirectory = string.IsNullOrEmpty(EslintOverrideIgnorePath)
                ? EnvironmentHelper.GetUserDirectoryPath()
                : Path.GetDirectoryName(EslintOverrideIgnorePath);

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            EslintOverrideIgnorePath = value;
        }

        //
        // Stylelint
        //

        private void BrowseStylelintConfigFile()
        {
            const string filter = "(*.js, *.yaml, *.yml, *.json, *.stylelintrc)|*.cmd;*.exe;*.yaml;*.yml;*.json;*.stylelintrc";

            var initialDirectory = string.IsNullOrEmpty(StylelintOverrideConfigPath)
                ? EnvironmentHelper.GetUserDirectoryPath()
                : Path.GetDirectoryName(StylelintOverrideConfigPath);

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            StylelintOverrideConfigPath = value;
        }

        private void BrowseStylelintFile()
        {
            const string filter = "(*.cmd, *.exe)|*.cmd;*.exe";

            var initialDirectory = string.IsNullOrEmpty(StylelintOverridePath)
                ? EnvironmentHelper.GetUserDirectoryPath()
                : Path.GetDirectoryName(StylelintOverridePath);

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            StylelintOverridePath = value;
        }

        private void BrowseStylelintIgnoreFile()
        {
            const string filter = "(*.stylelintignore)|*.stylelintignore";

            var initialDirectory = string.IsNullOrEmpty(StylelintOverrideIgnorePath)
                ? EnvironmentHelper.GetUserDirectoryPath()
                : Path.GetDirectoryName(StylelintOverrideIgnorePath);

            var value = GetDialogValue(filter, initialDirectory);
            if (null == value)
                return;

            StylelintOverrideIgnorePath = value;
        }

        //
        // Other
        //

        internal void Load()
        {
            ShouldOverrideEslintPath = Options.ShouldOverrideEslintPath;
            EslintOverridePath = Options.EslintOverridePath;
            ShouldOverrideEslintConfigPath = Options.ShouldOverrideEslintConfigPath;
            EslintOverrideConfigPath = Options.EslintOverrideConfigPath;
            ShouldOverrideEslintIgnorePath = Options.ShouldOverrideEslintIgnorePath;
            EslintOverrideIgnorePath = Options.EslintOverrideIgnorePath;
            StylelintOverridePath = Options.StylelintOverridePath;
            StylelintOverrideConfigPath = Options.StylelintOverrideConfigPath;
            StylelintOverrideIgnorePath = Options.StylelintOverrideIgnorePath;
            ShouldOverrideStylelintPath = Options.ShouldOverrideStylelintPath;
            ShouldOverrideStylelintConfigPath = Options.ShouldOverrideStylelintConfigPath;
            ShouldOverrideStylelintIgnorePath = Options.ShouldOverrideStylelintIgnorePath;
            ShowDebugInformation = Options.ShowDebugInformation;
        }

        internal void Save()
        {
            Options.ShouldOverrideEslintPath = ShouldOverrideEslintPath;
            Options.EslintOverridePath = EslintOverridePath;
            Options.ShouldOverrideEslintConfigPath = ShouldOverrideEslintConfigPath;
            Options.EslintOverrideConfigPath = EslintOverrideConfigPath;
            Options.ShouldOverrideEslintIgnorePath = ShouldOverrideEslintIgnorePath;
            Options.EslintOverrideIgnorePath = EslintOverrideIgnorePath;
            Options.ShowDebugInformation = ShowDebugInformation;
            Options.StylelintOverridePath = StylelintOverridePath;
            Options.StylelintOverrideConfigPath = StylelintOverrideConfigPath;
            Options.StylelintOverrideIgnorePath = StylelintOverrideIgnorePath;
            Options.ShouldOverrideStylelintPath = ShouldOverrideStylelintPath;
            Options.ShouldOverrideStylelintConfigPath = ShouldOverrideStylelintConfigPath;
            Options.ShouldOverrideStylelintIgnorePath = ShouldOverrideStylelintIgnorePath;
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
    }
}
