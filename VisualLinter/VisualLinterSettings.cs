using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.ComponentModel.Composition;

namespace jwldnr.VisualLinter
{
    internal interface IVisualLinterSettings
    {
        bool UseEnvironmentVariables { get; }
    }

    [Export(typeof(IVisualLinterSettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterSettings : IVisualLinterSettings
    {
        public bool UseEnvironmentVariables => true;

        [ImportingConstructor]
        internal VisualLinterSettings(SVsServiceProvider serviceProvider)
            : this(new ShellSettingsManager(serviceProvider))
        {
            // provider has to be of type SVsServiceProvider
        }

        internal VisualLinterSettings(SettingsManager settingsManager)
        {
            if (null == settingsManager)
                throw new ArgumentNullException(nameof(settingsManager));
        }
    }
}