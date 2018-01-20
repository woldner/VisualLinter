using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.IO;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class VsixHelper
    {
        internal static ProjectItem GetProjectItem(string filePath)
        {
            var solution = GetSolution();

            return solution?.FindProjectItem(filePath);
        }

        internal static string GetProjectName(string filePath)
        {
            var project = GetProject(filePath);

            return project?.Name;
        }

        internal static string GetSolutionPath()
        {
            var solution = GetSolution();

            return Path.GetDirectoryName(solution?.FullName);
        }

        private static Project GetProject(string filePath)
        {
            var item = GetProjectItem(filePath);

            return item?.ContainingProject;
        }

        private static Solution GetSolution()
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            return dte?.Solution;
        }
    }
}
