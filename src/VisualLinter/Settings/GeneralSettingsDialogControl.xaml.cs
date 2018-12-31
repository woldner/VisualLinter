using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using jwldnr.VisualLinter.Helpers;

namespace jwldnr.VisualLinter.Settings
{
    /// <summary>
    ///     Interaction logic for GeneralSettingsDialogControl.xaml
    /// </summary>
    public partial class GeneralSettingsDialogControl
    {
        internal static readonly DependencyProperty DisableEslintIgnoreProperty =
            DependencyProperty.Register(
                nameof(DisableEslintIgnore),
                typeof(bool),
                typeof(GeneralSettingsDialogControl),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableHtmlLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableHtmlLanguageSupport),
                typeof(bool),
                typeof(GeneralSettingsDialogControl),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableJavaScriptLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableJavaScriptLanguageSupport),
                typeof(bool),
                typeof(GeneralSettingsDialogControl),
                new PropertyMetadata(true));

        internal static readonly DependencyProperty EnableReactLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableReactLanguageSupport),
                typeof(bool),
                typeof(GeneralSettingsDialogControl),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty EnableVueLanguageSupportProperty =
            DependencyProperty.Register(
                nameof(EnableVueLanguageSupport),
                typeof(bool),
                typeof(GeneralSettingsDialogControl),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty UseGlobalEslintProperty =
            DependencyProperty.Register(
                nameof(UseGlobalEslint),
                typeof(bool),
                typeof(GeneralSettingsDialogControl),
                new PropertyMetadata(false));

        internal static readonly DependencyProperty UsePersonalConfigProperty =
            DependencyProperty.Register(
                nameof(UsePersonalConfig),
                typeof(bool),
                typeof(GeneralSettingsDialogControl),
                new PropertyMetadata(false));

        private readonly ILogger _logger;
        private readonly IVisualLinterSettings _settings;

        private ICommand _suggestNewFeaturesCommand;

        internal GeneralSettingsDialogControl(
            IVisualLinterSettings settings,
            ILogger logger)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();

            DataContext = this;
        }

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
            UseGlobalEslint = _settings.UseGlobalEslint;
            EnableHtmlLanguageSupport = _settings.EnableHtmlLanguageSupport;
            EnableJavaScriptLanguageSupport = _settings.EnableJavaScriptLanguageSupport;
            EnableReactLanguageSupport = _settings.EnableReactLanguageSupport;
            EnableVueLanguageSupport = _settings.EnableVueLanguageSupport;
            UsePersonalConfig = _settings.UsePersonalConfig;
            DisableEslintIgnore = _settings.DisableIgnorePath;
        }

        internal void Save()
        {
            _settings.DisableIgnorePath = DisableEslintIgnore;
            _settings.EnableHtmlLanguageSupport = EnableHtmlLanguageSupport;
            _settings.EnableJavaScriptLanguageSupport = EnableJavaScriptLanguageSupport;
            _settings.EnableReactLanguageSupport = EnableReactLanguageSupport;
            _settings.EnableVueLanguageSupport = EnableVueLanguageSupport;
            _settings.UseGlobalEslint = UseGlobalEslint;
            _settings.UsePersonalConfig = UsePersonalConfig;
        }

        private void SuggestNewFeatures(object url)
        {
            try
            {
                Process.Start(new ProcessStartInfo($"{url}"));
            }
            catch (Exception e)
            {
                _logger.WriteLine($"exception: could not open {url}");
                _logger.WriteLine(e.Message);
            }
        }
    }
}
