using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace jwldnr.VisualLinter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(VisualLinterOptions), Vsix.Name, "General", 101, 111, true, ProvidesLocalizedCategoryName = false)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.GuidVisualLinterPackageString)]
    internal sealed class VisualLinterPackage : AsyncPackage
    {
    }
}