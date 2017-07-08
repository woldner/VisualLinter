using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class ProjectHelper
    {
        internal static string GetProjectName(string fileName)
        {
            try
            {
                var dte = Package.GetGlobalService(typeof(DTE)) as DTE;

                var item = dte?.Solution.FindProjectItem(fileName);

                return item?.ContainingProject.Name;
            }
            catch (Exception)
            {
                OutputWindowHelper.WriteLine($"could not get project name for file: {fileName}");
            }

            return null;
        }
    }
}