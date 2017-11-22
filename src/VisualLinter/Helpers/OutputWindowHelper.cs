using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class OutputWindowHelper
    {
        private static readonly IVisualLinterOptions Options;
        private static IVsOutputWindowPane _outputWindowPane;

        private static IVsOutputWindowPane OutputWindowPane => _outputWindowPane
            ?? (_outputWindowPane = GetOutputWindowPane());

        static OutputWindowHelper()
        {
            Options = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>()
                ?? throw new Exception("fatal: unable to retrieve options");
        }

        internal static void DebugLine(object message)
        {
            Debug.WriteLine(message);

            var outputWindowPane = OutputWindowPane;
            if (null == outputWindowPane)
                return;

            if (false == Options.ShowDebugInformation)
                return;

            outputWindowPane.OutputStringThreadSafe($"{message + Environment.NewLine}");
            outputWindowPane.Activate();
        }

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