using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace jwldnr.VisualLinter
{
    internal interface IVisualLinterSettings
    {
        bool Value { get; }
    }

    [Export(typeof(IVisualLinterSettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterSettings : IVisualLinterSettings
    {
        public bool Value => true;

        [ImportingConstructor]
        internal VisualLinterSettings(SVsServiceProvider serviceProvider)
            : this(new ShellSettingsManager(serviceProvider))
        {
        }

        internal VisualLinterSettings(SettingsManager settingsManager)
        {
            if (null == settingsManager)
                throw new ArgumentNullException(nameof(settingsManager));


        }
    }
}