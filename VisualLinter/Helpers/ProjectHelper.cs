using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class ProjectHelper
    {
        internal static string GetProjectName(string fileName)
        {
            try
            {
                var solution = GetSolution();

                var item = solution?.FindProjectItem(fileName);

                return item?.ContainingProject.Name;
            }
            catch (Exception)
            {
                OutputWindowHelper.WriteLine($"could not get project name for file: {fileName}");
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