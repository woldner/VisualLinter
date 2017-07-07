using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class OutputWindowHelper
    {
        private static IVsOutputWindowPane _outputWindowPane;

        private static IVsOutputWindowPane OutputWindowPane =>
            _outputWindowPane ?? (_outputWindowPane = GetOutputWindowPane());

        private static IVsOutputWindowPane GetOutputWindowPane()
        {
            var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (null == outputWindow)
                return null;

            var outputPaneGuid = new Guid(PackageGuids.GuidVisualLinterPackageOutputPane.ToByteArray());

            outputWindow.CreatePane(ref outputPaneGuid, Vsix.Name, 1, 1);
            outputWindow.GetPane(ref outputPaneGuid, out IVsOutputWindowPane windowPane);

            return windowPane;
        }

        internal static void WriteLine(string message)
        {
            var outputWindowPane = OutputWindowPane;
            outputWindowPane?.OutputString($"[{Vsix.Name}]: {message + Environment.NewLine}");
        }
    }
}