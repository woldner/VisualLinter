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

        internal static readonly DependencyProperty DisableIgnorePathProperty =
            DependencyProperty.Register(
                nameof(DisableIgnorePath),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(default(bool)));

        internal static readonly DependencyProperty EnableHtmlLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableHtmlLanguageSupport),
                typeof(bool),
                typeof(OptionsDialogViewModel),
                new PropertyMetadata(default(bool)));

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
                new PropertyMetadata(default(bool)));

        internal static readonly DependencyProperty EnableVueLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableVueLanguageSupport),
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
            _options.DisableIgnorePath = DisableIgnorePath;
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
            EnableHtmlLanguageSupport = _options.EnableHtmlLanguageSupport;
            EnableJsLanguageSupport = _options.EnableJsLanguageSupport;
            EnableReactLanguageSupport = _options.EnableReactLanguageSupport;
            EnableVueLanguageSupport = _options.EnableVueLanguageSupport;
            UsePersonalConfig = _options.UsePersonalConfig;
            DisableIgnorePath = _options.DisableIgnorePath;
        }

        private void RaiseAllChanged()
        {
            RaisePropertyChanged(nameof(UseGlobalEslint));
            RaisePropertyChanged(nameof(EnableHtmlLanguageSupport));
            RaisePropertyChanged(nameof(EnableJsLanguageSupport));
            RaisePropertyChanged(nameof(EnableReactLanguageSupport));
            RaisePropertyChanged(nameof(EnableVueLanguageSupport));
            RaisePropertyChanged(nameof(UsePersonalConfig));
            RaisePropertyChanged(nameof(DisableIgnorePath));
        }
    }
}