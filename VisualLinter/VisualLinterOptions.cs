using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System.ComponentModel.Composition;

namespace jwldnr.VisualLinter
{
    internal interface IVisualLinterOptions
    {
        bool UseLocalConfig { get; set; }
    }

    [Export(typeof(IVisualLinterOptions))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterOptions : IVisualLinterOptions
    {
        public bool UseLocalConfig
        {
            get => _writableSettingsStore.GetBoolean(CollectionPath, nameof(UseLocalConfig), false);
            set => _writableSettingsStore.SetBoolean(CollectionPath, nameof(UseLocalConfig), value);
        }

        private const string CollectionPath = "jwldnr.VisualLinter";

        private readonly WritableSettingsStore _writableSettingsStore;

        [ImportingConstructor]
        internal VisualLinterOptions([Import] SVsServiceProvider serviceProvider)
        {
            var settingsManager = new ShellSettingsManager(serviceProvider);

            _writableSettingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (_writableSettingsStore != null &&
                !_writableSettingsStore.CollectionExists(CollectionPath))
            {
                _writableSettingsStore.CreateCollection(CollectionPath);
            }
        }
    }
}