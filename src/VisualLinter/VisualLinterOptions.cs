using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace jwldnr.VisualLinter
{
    public interface IVisualLinterOptions
    {
        //
        // ESLint General
        //

        bool UseGlobalEslint { get; set; }
        bool UsePersonalEslintConfig { get; set; }
        bool DisableEslintIgnore { get; set; }

        //
        // ESLint Language Support
        //

        bool EnableHtmlLanguageSupport { get; set; }
        bool EnableJavaScriptLanguageSupport { get; set; }
        bool EnableReactLanguageSupport { get; set; }
        bool EnableVueLanguageSupport { get; set; }

        //
        // Stylelint General
        //

        bool UseGlobalStylelint { get; set; }
        bool UsePersonalStylelintConfig { get; set; }
        bool DisableStylelintIgnore { get; set; }

        //
        // Stylelint Language Support
        //

        bool EnableCssLanguageSupport { get; set; }
        bool EnableScssLanguageSupport { get; set; }
        bool EnableSassLanguageSupport { get; set; }
        bool EnableLessLanguageSupport { get; set; }

        //
        // ESLint Advanced
        //

        bool ShouldOverrideEslintPath { get; set; }
        bool ShouldOverrideEslintConfigPath { get; set; }
        bool ShouldOverrideEslintIgnorePath { get; set; }
        string EslintOverridePath { get; set; }
        string EslintOverrideConfigPath { get; set; }
        string EslintOverrideIgnorePath { get; set; }

        //
        // Stylelint Advanced
        //

        bool ShouldOverrideStylelintPath { get; set; }
        bool ShouldOverrideStylelintConfigPath { get; set; }
        bool ShouldOverrideStylelintIgnorePath { get; set; }
        string StylelintOverridePath { get; set; }
        string StylelintOverrideConfigPath { get; set; }
        string StylelintOverrideIgnorePath { get; set; }

        //
        // Advanced
        //

        bool ShowDebugInformation { get; set; }
    }

    [Export(typeof(IVisualLinterOptions))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterOptions : IVisualLinterOptions
    {
        private const string CollectionPath = "jwldnr.VisualLinter";

        private readonly WritableSettingsStore _writableSettingsStore;

        //
        // ESLint General
        //

        public bool UseGlobalEslint
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UseGlobalEslint), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UseGlobalEslint), value);
        }

        public bool UsePersonalEslintConfig
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UsePersonalEslintConfig), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UsePersonalEslintConfig), value);
        }

        public bool DisableEslintIgnore
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(DisableEslintIgnore), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(DisableEslintIgnore), value);
        }

        //
        // ESLint Language Support
        //

        public bool EnableHtmlLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableHtmlLanguageSupport), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableHtmlLanguageSupport), value);
        }

        public bool EnableJavaScriptLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableJavaScriptLanguageSupport), true);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableJavaScriptLanguageSupport), value);
        }

        public bool EnableReactLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableReactLanguageSupport), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableReactLanguageSupport), value);
        }

        public bool EnableVueLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableVueLanguageSupport), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableVueLanguageSupport), value);
        }

        //
        // Stylelint General
        //

        public bool UseGlobalStylelint
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UseGlobalStylelint), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UseGlobalStylelint), value);
        }

        public bool UsePersonalStylelintConfig
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UsePersonalStylelintConfig), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UsePersonalStylelintConfig), value);
        }

        public bool DisableStylelintIgnore
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(DisableStylelintIgnore), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(DisableStylelintIgnore), value);
        }

        //
        // Stylelint Language Support
        //

        public bool EnableCssLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableCssLanguageSupport), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableCssLanguageSupport), value);
        }

        public bool EnableScssLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableScssLanguageSupport), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableScssLanguageSupport), value);
        }

        public bool EnableSassLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableSassLanguageSupport), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableSassLanguageSupport), value);
        }

        public bool EnableLessLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableLessLanguageSupport), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableLessLanguageSupport), value);
        }

        //
        // ESLint Advanced
        //

        public bool ShouldOverrideEslintPath
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideEslintPath), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideEslintPath), value);
        }

        public bool ShouldOverrideEslintConfigPath
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideEslintConfigPath), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideEslintConfigPath), value);
        }

        public bool ShouldOverrideEslintIgnorePath
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideEslintIgnorePath), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideEslintIgnorePath), value);
        }

        public string EslintOverridePath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(EslintOverridePath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(EslintOverridePath), value);
        }

        public string EslintOverrideConfigPath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(EslintOverrideConfigPath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(EslintOverrideConfigPath), value);
        }

        public string EslintOverrideIgnorePath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(EslintOverrideIgnorePath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(EslintOverrideIgnorePath), value);
        }

        //
        // Stylelint Advanced
        //

        public bool ShouldOverrideStylelintPath
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideStylelintPath), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideStylelintPath), value);
        }

        public bool ShouldOverrideStylelintConfigPath
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideStylelintConfigPath), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideStylelintConfigPath), value);
        }

        public bool ShouldOverrideStylelintIgnorePath
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideStylelintIgnorePath), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideStylelintIgnorePath), value);
        }

        public string StylelintOverridePath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(StylelintOverridePath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(StylelintOverridePath), value);
        }

        public string StylelintOverrideConfigPath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(StylelintOverrideConfigPath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(StylelintOverrideConfigPath), value);
        }

        public string StylelintOverrideIgnorePath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(StylelintOverrideIgnorePath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(StylelintOverrideIgnorePath), value);
        }

        //
        // Advanced
        //

        public bool ShowDebugInformation
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShowDebugInformation), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShowDebugInformation), value);
        }

        [ImportingConstructor]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        internal VisualLinterOptions([Import] SVsServiceProvider serviceProvider)
        {
            var settingsManager = new ShellSettingsManager(serviceProvider);

            _writableSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (null == _writableSettingsStore || _writableSettingsStore.CollectionExists(CollectionPath))
                return;

            _writableSettingsStore.CreateCollection(CollectionPath);
        }
    }
}
