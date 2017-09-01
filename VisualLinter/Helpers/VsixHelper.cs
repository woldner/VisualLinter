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
            return GetProject(filePath)?.Name;
        }

        internal static string GetSolutionPath()
        {
            var solution = GetSolution();

            return Path.GetDirectoryName(solution?.FullName)
                ?? throw new Exception("fatal: could not get solution directory info.");
        }

        private static Project GetProject(string filePath)
        {
            var solution = GetSolution();

            var item = solution?.FindProjectItem(filePath);

            return item?.ContainingProject;
        }

        private static Solution GetSolution()
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            return dte?.Solution;
        }
    }
}