using jwldnr.VisualLinter.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace jwldnr.VisualLinter.ViewModels
{
    internal class GeneralOptionsDialogViewModel : ViewModelBase
    {
        //
        // ESLint General
        //

        internal static readonly DependencyProperty UseGlobalEslintProperty =
            DependencyProperty.Register(
                nameof(UseGlobalEslint),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty UsePersonalEslintConfigProperty =
            DependencyProperty.Register(
                nameof(UsePersonalEslintConfig),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty DisableEslintIgnoreProperty =
            DependencyProperty.Register(
                nameof(DisableEslintIgnore),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        //
        // ESLint Language Support
        //

        internal static readonly DependencyProperty EnableHtmlLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableHtmlLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableJavaScriptLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableJavaScriptLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(true));

        internal static readonly DependencyProperty EnableReactLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableReactLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableVueLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableVueLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        //
        // Stylelint General
        //

        internal static readonly DependencyProperty UseGlobalStylelintProperty =
            DependencyProperty.Register(
                nameof(UseGlobalStylelint),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty UsePersonalStylelintConfigProperty =
            DependencyProperty.Register(
                nameof(UsePersonalStylelintConfig),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty DisableStylelintIgnoreProperty =
            DependencyProperty.Register(
                nameof(DisableStylelintIgnore),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        //
        // Stylelint Language Support
        //

        internal static readonly DependencyProperty EnableCssLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableCssLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(true));

        internal static readonly DependencyProperty EnableScssLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableScssLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(true));

        internal static readonly DependencyProperty EnableSassLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableSassLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(true));

        internal static readonly DependencyProperty EnableLessLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableLessLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(true));

        //
        // ESLint General
        //

        internal bool UseGlobalEslint
        {
            get => GetPropertyValue<bool>(UseGlobalEslintProperty);
            set => SetPropertyValue(UseGlobalEslintProperty, value);
        }

        internal bool UsePersonalEslintConfig
        {
            get => GetPropertyValue<bool>(UsePersonalEslintConfigProperty);
            set => SetPropertyValue(UsePersonalEslintConfigProperty, value);
        }

        internal bool DisableEslintIgnore
        {
            get => GetPropertyValue<bool>(DisableEslintIgnoreProperty);
            set => SetPropertyValue(DisableEslintIgnoreProperty, value);
        }

        //
        // ESLint Language Support
        //

        internal bool EnableHtmlLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableHtmlLanguageSupportProperty);
            set => SetPropertyValue(EnableHtmlLanguageSupportProperty, value);
        }

        internal bool EnableJavaScriptLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableJavaScriptLanguageSupportProperty);
            set => SetPropertyValue(EnableJavaScriptLanguageSupportProperty, value);
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

        //
        // Stylelint General
        //

        internal bool UseGlobalStylelint
        {
            get => GetPropertyValue<bool>(UseGlobalStylelintProperty);
            set => SetPropertyValue(UseGlobalStylelintProperty, value);
        }

        internal bool UsePersonalStylelintConfig
        {
            get => GetPropertyValue<bool>(UsePersonalStylelintConfigProperty);
            set => SetPropertyValue(UsePersonalStylelintConfigProperty, value);
        }

        internal bool DisableStylelintIgnore
        {
            get => GetPropertyValue<bool>(DisableStylelintIgnoreProperty);
            set => SetPropertyValue(DisableStylelintIgnoreProperty, value);
        }

        //
        // Stylelint Language Support
        //

        internal bool EnableCssLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableCssLanguageSupportProperty);
            set => SetPropertyValue(EnableCssLanguageSupportProperty, value);
        }

        internal bool EnableScssLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableScssLanguageSupportProperty);
            set => SetPropertyValue(EnableScssLanguageSupportProperty, value);
        }

        internal bool EnableSassLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableSassLanguageSupportProperty);
            set => SetPropertyValue(EnableSassLanguageSupportProperty, value);
        }

        internal bool EnableLessLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableLessLanguageSupportProperty);
            set => SetPropertyValue(EnableLessLanguageSupportProperty, value);
        }

        //
        // Misc
        //

        private ICommand _suggestNewFeaturesCommand;

        public ICommand SuggestNewFeaturesCommand
        {
            get => _suggestNewFeaturesCommand ??
                (_suggestNewFeaturesCommand = new RelayCommand<string>(SuggestNewFeatures));
            set => _suggestNewFeaturesCommand = value;
        }

        internal void Load()
        {
            UseGlobalEslint = Options.UseGlobalEslint;
            UsePersonalEslintConfig = Options.UsePersonalEslintConfig;
            DisableEslintIgnore = Options.DisableEslintIgnore;

            UseGlobalStylelint = Options.UseGlobalStylelint;
            UsePersonalStylelintConfig = Options.UsePersonalStylelintConfig;
            DisableStylelintIgnore = Options.DisableStylelintIgnore;

            EnableHtmlLanguageSupport = Options.EnableHtmlLanguageSupport;
            EnableJavaScriptLanguageSupport = Options.EnableJavaScriptLanguageSupport;
            EnableReactLanguageSupport = Options.EnableReactLanguageSupport;
            EnableVueLanguageSupport = Options.EnableVueLanguageSupport;

            EnableCssLanguageSupport = Options.EnableCssLanguageSupport;
            EnableScssLanguageSupport = Options.EnableScssLanguageSupport;
            EnableSassLanguageSupport = Options.EnableSassLanguageSupport;
            EnableLessLanguageSupport = Options.EnableLessLanguageSupport;
        }

        internal void Save()
        {
            Options.UseGlobalEslint = UseGlobalEslint;
            Options.UsePersonalEslintConfig = UsePersonalEslintConfig;
            Options.DisableEslintIgnore = DisableEslintIgnore;

            Options.UseGlobalStylelint = UseGlobalStylelint;
            Options.UsePersonalStylelintConfig = UsePersonalStylelintConfig;
            Options.DisableStylelintIgnore = DisableStylelintIgnore;

            Options.EnableHtmlLanguageSupport = EnableHtmlLanguageSupport;
            Options.EnableJavaScriptLanguageSupport = EnableJavaScriptLanguageSupport;
            Options.EnableReactLanguageSupport = EnableReactLanguageSupport;
            Options.EnableVueLanguageSupport = EnableVueLanguageSupport;

            Options.EnableCssLanguageSupport = EnableCssLanguageSupport;
            Options.EnableScssLanguageSupport = EnableScssLanguageSupport;
            Options.EnableSassLanguageSupport = EnableSassLanguageSupport;
            Options.EnableLessLanguageSupport = EnableLessLanguageSupport;
        }

        private static void SuggestNewFeatures(object url)
        {
            try
            {
                Process.Start(new ProcessStartInfo($"{url}"));
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine($"exception: could not open {url}");
                OutputWindowHelper.WriteLine(e.Message);
            }
        }
    }
}
