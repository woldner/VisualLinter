using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.IO;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class VsixHelper
    {
        private static readonly string[] ValidConfigNames = {
            ".eslintrc.js",
            ".eslintrc.yaml",
            ".eslintrc.yml",
            ".eslintrc.json",
            ".eslintrc"
        };

        internal static string GetGlobalConfigPath()
        {
            var userDirectoryPath = GetUserDirectoryPath();

            var foundConfigs = Directory.EnumerateFiles(userDirectoryPath, ".eslintrc*", SearchOption.TopDirectoryOnly)
                .Where(filePath => -1 != Array.IndexOf(ValidConfigNames, Path.GetFileName(filePath)))
                .OrderBy(filePath => Array.IndexOf(ValidConfigNames, Path.GetFileName(filePath)));

            return foundConfigs.Any()
                ? foundConfigs.First()
                : null;
        }

        internal static string GetLocalConfigPath(string filePath)
        {
            // never traverse directory further than the solution root
            var solutionFolder = new DirectoryInfo(GetSolutionPath());

            var filePathDirectory = Path.GetDirectoryName(filePath)
                ?? throw new Exception("could not get working directory name");

            var currentDirectory = new DirectoryInfo(filePathDirectory);

            // todo: instead of comparing length.. ugh
            while (currentDirectory != null && currentDirectory.FullName.Length >= solutionFolder.FullName.Length)
            {
                var foundConfigs = currentDirectory.EnumerateFiles(".eslintrc*")
                    .Where(fileInfo => -1 != Array.IndexOf(ValidConfigNames, fileInfo.Name))
                    .OrderBy(fileInfo => Array.IndexOf(ValidConfigNames, fileInfo.Name));

                if (foundConfigs.Any())
                    return foundConfigs.First().FullName;

                currentDirectory = currentDirectory.Parent;
            }

            return null;
        }

        internal static string GetProjectName(string filePath)
        {
            try
            {
                return GetProject(filePath).Name;
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine($"Unable to get project name for file: {Path.GetFileName(filePath)}"
                    + $"\nError message: {e.Message}");
            }

            return null;
        }

        internal static string GetSolutionPath()
        {
            var solution = GetSolution();

            return Path.GetDirectoryName(solution?.FullName)
                ?? throw new Exception("could not get directory info for solution");
        }

        private static Project GetProject(string filePath)
        {
            try
            {
                var solution = GetSolution();

                var item = solution?.FindProjectItem(filePath);

                return item?.ContainingProject;
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine($"Unable to get project for file: {Path.GetFileName(filePath)}"
                    + $"\nError message: {e.Message}");
            }

            return null;
        }

        private static Solution GetSolution()
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            return dte?.Solution;
        }

        private static string GetUserDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }
}