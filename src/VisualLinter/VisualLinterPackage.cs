using jwldnr.VisualLinter.Views;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace jwldnr.VisualLinter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(GeneralOptionsDialogPage), Vsix.Name, GeneralOptionsDialogPage.PageName, 0, 0, true)]
    [ProvideOptionPage(typeof(AdvancedOptionsDialogPage), Vsix.Name, AdvancedOptionsDialogPage.PageName, 0, 0, true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.GuidVisualLinterPackageString)]
    internal sealed class VisualLinterPackage : AsyncPackage
    {
    }
}
