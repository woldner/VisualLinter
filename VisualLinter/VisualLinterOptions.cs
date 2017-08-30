using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace jwldnr.VisualLinter
{
    internal interface IVisualLinterOptions
    {
        bool UseGlobalConfig { get; set; }
        bool UseGlobalLinter { get; set; }
    }

    [Export(typeof(IVisualLinterOptions))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterOptions : IVisualLinterOptions
    {
        public bool UseGlobalConfig
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UseGlobalConfig), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UseGlobalConfig), value);
        }

        public bool UseGlobalLinter
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UseGlobalLinter), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UseGlobalLinter), value);
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