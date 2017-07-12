using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace jwldnr.VisualLinter
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version, IconResourceID = 400)]
    [Guid(PackageGuids.GuidVisualLinterPackageString)]
    internal sealed class VisualLinterPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            await PromptForDownload();
        }

        private static async Task PromptForDownload()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (true == new ESLintInstallDialog().ShowDialog())
                new ESLintInstaller().Show();
        }
    }
}