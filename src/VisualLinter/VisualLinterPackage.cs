using System.Runtime.InteropServices;
using jwldnr.VisualLinter.Settings;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace jwldnr.VisualLinter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(GeneralSettingsDialogPage), Vsix.Name, GeneralSettingsDialogPage.PageName, 0, 0, true)]
    [ProvideOptionPage(typeof(AdvancedSettingsDialogPage), Vsix.Name, AdvancedSettingsDialogPage.PageName, 0, 0, true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.GuidVisualLinterPackageString)]
    internal sealed class VisualLinterPackage : AsyncPackage
    {
    }
}
