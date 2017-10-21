using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace jwldnr.VisualLinter
{
    public interface IVisualLinterOptions
    {
        bool DisableIgnorePath { get; set; }
        bool EnableHtmlLanguageSupport { get; set; }
        bool EnableJsLanguageSupport { get; set; }
        bool EnableReactLanguageSupport { get; set; }
        bool EnableVueLanguageSupport { get; set; }
        bool UseGlobalEslint { get; set; }
        bool UsePersonalConfig { get; set; }
    }

    [Export(typeof(IVisualLinterOptions))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterOptions : IVisualLinterOptions
    {
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

        public bool EnableJsLanguageSupport
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(EnableJsLanguageSupport), true);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(EnableJsLanguageSupport), value);
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

        private const string CollectionPath = "jwldnr.VisualLinter";

        private readonly WritableSettingsStore _writableSettingsStore;

        [ImportingConstructor]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        internal VisualLinterOptions([Import] SVsServiceProvider serviceProvider)
        {
            var settingsManager = new ShellSettingsManager(serviceProvider);

            _writableSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings)
                ?? throw new ArgumentNullException(nameof(settingsManager));

            if (null == _writableSettingsStore || _writableSettingsStore.CollectionExists(CollectionPath))
                return;

            _writableSettingsStore.CreateCollection(CollectionPath);
        }
    }
}