using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.ComponentModel.Composition;

namespace jwldnr.VisualLinter
{
    internal interface IVisualLinterSettings
    {
        bool SkipInstallDialog { get; set; }
    }

    [Export(typeof(IVisualLinterSettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterSettings : IVisualLinterSettings, IProfileManager
    {
        public bool SkipInstallDialog
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(SkipInstallDialog), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(SkipInstallDialog), value);
        }

        internal const string CollectionPath = "VisualLinter";

        private readonly WritableSettingsStore _writableSettingsStore;

        [ImportingConstructor]
        internal VisualLinterSettings(IServiceProvider serviceProvider)
        {
            var settingsManager = new ShellSettingsManager(serviceProvider);

            _writableSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (null == _writableSettingsStore || _writableSettingsStore.CollectionExists(CollectionPath))
                return;

            _writableSettingsStore.CreateCollection(CollectionPath);
        }

        public void LoadSettingsFromStorage()
        {
            // noop
        }

        public void LoadSettingsFromXml(IVsSettingsReader reader)
        {
            // noop
        }

        public void ResetSettings()
        {
            // noop
        }

        public void SaveSettingsToStorage()
        {
            // noop
        }

        public void SaveSettingsToXml(IVsSettingsWriter writer)
        {
            // noop
        }
    }
}