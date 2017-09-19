using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class OutputWindowHelper
    {
        private static IVsOutputWindowPane OutputWindowPane => _outputWindowPane
            ?? (_outputWindowPane = GetOutputWindowPane());

        private static IVsOutputWindowPane _outputWindowPane;

        internal static void WriteLine(object message)
        {
            var outputWindowPane = OutputWindowPane;

            outputWindowPane?.OutputString($"{message + Environment.NewLine}");
        }

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
    }
}