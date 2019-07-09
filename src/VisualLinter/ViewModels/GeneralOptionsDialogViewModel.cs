using jwldnr.VisualLinter.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace jwldnr.VisualLinter.ViewModels
{
    internal class GeneralOptionsDialogViewModel : ViewModelBase
    {
        internal static readonly DependencyProperty DisableEslintIgnoreProperty =
            DependencyProperty.Register(
                nameof(DisableEslintIgnore),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

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

        internal static readonly DependencyProperty EnableTypeScriptLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableTypeScriptLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableTypeScriptReactLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableTypeScriptReactLanguageSupport),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty UseGlobalEslintProperty =
            DependencyProperty.Register(
                nameof(UseGlobalEslint),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty UsePersonalConfigProperty =
            DependencyProperty.Register(
                nameof(UsePersonalConfig),
                typeof(bool),
                typeof(GeneralOptionsDialogViewModel),
                new PropertyMetadata(false));

        private ICommand _suggestNewFeaturesCommand;

        public ICommand SuggestNewFeaturesCommand
        {
            get => _suggestNewFeaturesCommand ??
                (_suggestNewFeaturesCommand = new RelayCommand<string>(SuggestNewFeatures));
            set => _suggestNewFeaturesCommand = value;
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

        internal bool EnableTypeScriptLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableTypeScriptLanguageSupportProperty);
            set => SetPropertyValue(EnableTypeScriptLanguageSupportProperty, value);
        }

        internal bool EnableTypeScriptReactLanguageSupport
        {
            get => GetPropertyValue<bool>(EnableTypeScriptReactLanguageSupportProperty);
            set => SetPropertyValue(EnableTypeScriptReactLanguageSupportProperty, value);
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

        internal void Load()
        {
            UseGlobalEslint = Options.UseGlobalEslint;
            EnableHtmlLanguageSupport = Options.EnableHtmlLanguageSupport;
            EnableJavaScriptLanguageSupport = Options.EnableJavaScriptLanguageSupport;
            EnableReactLanguageSupport = Options.EnableReactLanguageSupport;
            EnableVueLanguageSupport = Options.EnableVueLanguageSupport;
            EnableTypeScriptLanguageSupport = Options.EnableTypeScriptLanguageSupport;
            EnableTypeScriptReactLanguageSupport = Options.EnableTypeScriptReactLanguageSupport;
            UsePersonalConfig = Options.UsePersonalConfig;
            DisableEslintIgnore = Options.DisableIgnorePath;
        }

        internal void Save()
        {
            Options.DisableIgnorePath = DisableEslintIgnore;
            Options.EnableHtmlLanguageSupport = EnableHtmlLanguageSupport;
            Options.EnableJavaScriptLanguageSupport = EnableJavaScriptLanguageSupport;
            Options.EnableReactLanguageSupport = EnableReactLanguageSupport;
            Options.EnableVueLanguageSupport = EnableVueLanguageSupport;
            Options.EnableTypeScriptLanguageSupport = EnableTypeScriptLanguageSupport;
            Options.EnableTypeScriptReactLanguageSupport = EnableTypeScriptReactLanguageSupport;
            Options.UseGlobalEslint = UseGlobalEslint;
            Options.UsePersonalConfig = UsePersonalConfig;
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
