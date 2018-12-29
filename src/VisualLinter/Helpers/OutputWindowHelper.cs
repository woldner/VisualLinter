
using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class OutputWindowHelper
    {
        internal static void WriteLine(IServiceProvider serviceProvider, string message)
        {
            if (null == serviceProvider)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var outputWindowPane = GetOutputWindowPane(serviceProvider);
            if (null != outputWindowPane)
            {
                WriteLineToPane(outputWindowPane, message);
            }
        }

        private static void WriteLineToPane(IVsOutputWindowPane outputWindowPane, string message)
        {
            var hr = outputWindowPane.OutputStringThreadSafe(message + Environment.NewLine);
            Debug.Assert(ErrorHandler.Succeeded(hr), $"OutputStringThreadSafe failed: {hr}");
        }

        private static IVsOutputWindowPane GetOutputWindowPane(IServiceProvider serviceProvider)
        {
            var outputWindow = serviceProvider.GetService<SVsOutputWindow, IVsOutputWindow>();
            if (null == outputWindow)
            {
                Debug.Fail("could not get IVsOutputWindow");
                return null;
            }

            var outputPaneGuid = PackageGuids.GuidVisualLinterPackageOutputPane;

            var hrGetPane = outputWindow.GetPane(ref outputPaneGuid, out var windowPane);
            if (ErrorHandler.Succeeded(hrGetPane))
            {
                return windowPane;
            }

            var hrCreatePane = outputWindow.CreatePane(ref outputPaneGuid, Vsix.Name, 1, 1);
            Debug.Assert(ErrorHandler.Succeeded(hrCreatePane), $"outputWindow.CreatePane failed: {hrCreatePane}");

            hrGetPane = outputWindow.GetPane(ref outputPaneGuid, out windowPane);
            Debug.Assert(ErrorHandler.Succeeded(hrGetPane), $"outputWindow.GetPane failed: {hrGetPane}");

            return windowPane;
        }
    }
}
