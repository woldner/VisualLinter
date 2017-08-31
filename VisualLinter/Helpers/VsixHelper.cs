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
                return GetProject(filePath).Name;
            }
            catch (Exception)
            {
                OutputWindowHelper.WriteLine($"warning: unable to get project name for file: {filePath}.");
            }

            return null;
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