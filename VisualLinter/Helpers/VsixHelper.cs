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
                var solution = GetSolution();

                var item = solution?.FindProjectItem(fileName);

                return item?.ContainingProject.Name;
            }
            catch (Exception)
            {
                OutputWindowHelper.WriteLine("unable to get project name for file: "
                    + Path.GetFileName(filePath));
            }

            return null;
        }

        internal static string GetSolutionPath()
        {
            var solution = GetSolution();

            return Path.GetDirectoryName(solution?.FullName);
        }

        private static Solution GetSolution()
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            return dte?.Solution;
        }
    }
}
