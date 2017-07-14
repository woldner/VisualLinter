using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace jwldnr.VisualLinter
{
    internal interface IVisualLinterOptions
    {
        bool UseEnvironmentVariables { get; set; }
    }

    [Export(typeof(IVisualLinterOptions))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterOptions : DialogPage, IVisualLinterOptions
    {
        [Category("General")]
        [DisplayName("Environment Variables")]
        [Description("Use Environment Variables to find ESLint")]
        [DefaultValue(true)]
        public bool UseEnvironmentVariables { get; set; }
    }
}