using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace jwldnr.VisualLinter
{
    internal interface IVisualLinterOptions
    {
        bool UseEnvironmentVariables { get; }

        //string ESLintPath { get; set; }
    }

    [Export(typeof(IVisualLinterOptions))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VisualLinterOptions : DialogPage, IVisualLinterOptions
    {
        [Category("General")]
        [DisplayName("Environment Variables")]
        [Description("Use Environment Variables to find ESLint")]
        [DefaultValue(true)]
        public bool UseEnvironmentVariables => true;

        //[Category("General")]
        //[DisplayName("ESLint Path")]
        //[Description("Path to ESLint executable (value will be ignored if option 'Environment Variables' is set to true)")]
        //[DefaultValue("")]
        //public string ESLintPath { get; set; }
    }
}