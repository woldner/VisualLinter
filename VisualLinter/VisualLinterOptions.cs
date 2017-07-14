using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace jwldnr.VisualLinter
{
    internal class VisualLinterOptions : DialogPage
    {
        [Category("General")]
        [DisplayName("Use Environment Variables to locate ESLint")]
        [DefaultValue(true)]
        public bool UseEnvironmentVariables => Settings.UseEnvironmentVariables;

        private IVisualLinterSettings Settings => _settings ?? (_settings = GetSettings());
        private IVisualLinterSettings _settings;

        private static IVisualLinterSettings GetSettings()
        {
            var componentModel = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            return componentModel?.DefaultExportProvider.GetExportedValue<IVisualLinterSettings>();
        }
    }
}