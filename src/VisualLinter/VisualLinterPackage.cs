using jwldnr.VisualLinter.Views;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace jwldnr.VisualLinter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(OptionsDialogPage), Vsix.Name, OptionsDialogPage.PageName, 1001, 1002, true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.GuidVisualLinterPackageString)]
    internal sealed class VisualLinterPackage : AsyncPackage
    {
    }
}