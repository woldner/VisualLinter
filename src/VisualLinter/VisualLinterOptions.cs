using System;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Shell.Interop;

namespace jwldnr.VisualLinter
{
    public interface IVisualLinterOptions : IProfileManager
    {
        bool DisableIgnorePath { get; set; }
        bool EnableHtmlLanguageSupport { get; set; }
        bool EnableJavaScriptLanguageSupport { get; set; }
        bool EnableReactLanguageSupport { get; set; }
        bool EnableVueLanguageSupport { get; set; }
        string EslintConfigOverridePath { get; set; }
        string EslintIgnoreOverridePath { get; set; }
        string EslintOverridePath { get; set; }
        bool ShouldOverrideEslint { get; set; }
        bool ShouldOverrideEslintConfig { get; set; }
        bool ShouldOverrideEslintIgnore { get; set; }
        bool ShowDebugInformation { get; set; }
        bool UseGlobalEslint { get; set; }
        bool UsePersonalConfig { get; set; }
    }

    [Export(typeof(IVisualLinterOptions))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterOptions : IVisualLinterOptions
    {
        private const string CollectionPath = "jwldnr.VisualLinter";

        private readonly WritableSettingsStore _writableSettingsStore;

        public bool DisableIgnorePath
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(DisableIgnorePath), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(DisableIgnorePath), value);
        }

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

        public string EslintConfigOverridePath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(EslintConfigOverridePath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(EslintConfigOverridePath), value);
        }

        public string EslintIgnoreOverridePath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(EslintIgnoreOverridePath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(EslintIgnoreOverridePath), value);
        }

        public string EslintOverridePath
        {
            get => _writableSettingsStore.GetString(CollectionPath, nameof(EslintOverridePath), string.Empty);
            set => _writableSettingsStore.SetString(CollectionPath, nameof(EslintOverridePath), value);
        }

        public bool ShouldOverrideEslint
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideEslint), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideEslint), value);
        }

        public bool ShouldOverrideEslintConfig
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideEslintConfig), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideEslintConfig), value);
        }

        public bool ShouldOverrideEslintIgnore
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShouldOverrideEslintIgnore), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShouldOverrideEslintIgnore), value);
        }

        public bool ShowDebugInformation
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(ShowDebugInformation), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(ShowDebugInformation), value);
        }

        public bool UseGlobalEslint
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UseGlobalEslint), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UseGlobalEslint), value);
        }

        public bool UsePersonalConfig
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UsePersonalConfig), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UsePersonalConfig), value);
        }

        [ImportingConstructor]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        internal VisualLinterOptions([Import] SVsServiceProvider serviceProvider)
            : this(new ShellSettingsManager(serviceProvider))
        {
        }

        internal VisualLinterOptions(SettingsManager settingsManager)
        {
            if (null == settingsManager)
                throw new ArgumentNullException(nameof(settingsManager));

            _writableSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (null != _writableSettingsStore && !_writableSettingsStore.CollectionExists(CollectionPath))
                _writableSettingsStore.CreateCollection(CollectionPath);
        }

        public void SaveSettingsToXml(IVsSettingsWriter writer)
        {
            // noop
        }

        public void LoadSettingsFromXml(IVsSettingsReader reader)
        {
            // noop
        }

        public void SaveSettingsToStorage()
        {
            // noop
        }

        public void LoadSettingsFromStorage()
        {
            // noop
        }

        public void ResetSettings()
        {
            // noop
        }
    }
}
