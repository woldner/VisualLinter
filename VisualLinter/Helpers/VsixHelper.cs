using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class VsixHelper
    {
        internal static string GetProjectName(string filePath)
        {
            try
            {
                var dte = Package.GetGlobalService(typeof(DTE)) as DTE;

                var item = dte?.Solution.FindProjectItem(filePath);

                return item?.ContainingProject.Name;
            }
            catch (Exception)
            {
                OutputWindowHelper.WriteLine("unable to get project name for file: "
                    + Path.GetFileName(filePath));
            }

            return null;
        }
    }
}