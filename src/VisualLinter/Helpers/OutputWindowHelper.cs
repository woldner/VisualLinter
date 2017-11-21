using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class OutputWindowHelper
    {
        private static IVsOutputWindowPane _outputWindowPane;

        private static IVsOutputWindowPane OutputWindowPane => _outputWindowPane
            ?? (_outputWindowPane = GetOutputWindowPane());

        internal static void WriteLine(object message)
        {
            var outputWindowPane = OutputWindowPane;
            if (null == outputWindowPane)
                return;

            outputWindowPane.OutputStringThreadSafe($"{message + Environment.NewLine}");
            outputWindowPane.Activate();
        }

        private static IVsOutputWindowPane GetOutputWindowPane()
        {
            if (!(Package.GetGlobalService(typeof(SVsOutputWindow)) is IVsOutputWindow outputWindow))
                return null;

            var outputPaneGuid = new Guid(PackageGuids.GuidVisualLinterPackageOutputPane.ToByteArray());

            outputWindow.CreatePane(ref outputPaneGuid, Vsix.Name, 1, 1);
            outputWindow.GetPane(ref outputPaneGuid, out var windowPane);

            return windowPane;
        }
    }
}