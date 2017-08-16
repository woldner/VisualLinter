using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
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
            // todo
            return null;
        }

        internal static string GetLocalConfigPath(string filePath)
        {
            // never traverse further than the solution root
            var end = new DirectoryInfo(GetSolutionPath());
            if (null == end)
                throw new Exception("could not get directory info for solution");

            var workingDirectory = Path.GetDirectoryName(filePath);
            if (null == workingDirectory)
                throw new Exception("could not get working directory name");

            var start = new DirectoryInfo(workingDirectory);
            if (null == start)
                throw new Exception("could not get directory info for working directory");

            while (start != null && start.FullName != end.FullName)
            {
                var configs = start.EnumerateFiles(".eslintrc*")
                    .ToList();

                if (configs.Any())
                {
                    // only gets file name
                    //var validFiles = ValidConfigNames.Intersect(files.Select(file => file.Name))
                    //    .OrderBy(file => Array.IndexOf(ValidConfigNames, file));

                    var validConfigs = configs
                        .Where(file => -1 != Array.IndexOf(ValidConfigNames, file.Name))
                        .OrderBy(file => Array.IndexOf(ValidConfigNames, file.Name));

                    return validConfigs.First().FullName;
                }

                start = start.Parent;
            }

            // no config found
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

            return Path.GetDirectoryName(solution?.FullName);
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
                    + $"\n.. with reason: {e.Message}");
            }

            return null;
        }

        private static Solution GetSolution()
        {
            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            return dte?.Solution;
        }

        private static string GetUserFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }
}