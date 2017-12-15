using System;
using System.IO;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class EslintHelper
    {
        private const string ExecutableName = "eslint.cmd";
        private const string VariableName = "eslint";

        private static readonly string[] SupportedConfigs =
        {
            ".eslintrc.js",
            ".eslintrc.yaml",
            ".eslintrc.yml",
            ".eslintrc.json",
            ".eslintrc"
        };

        internal static string GetGlobalEslintPath()
        {
            try
            {
                return EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.User)
                    ?? EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.Machine);
            }
            catch (Exception exception)
            {
                OutputWindowHelper.WriteLine(exception.Message);
            }

            return null;
        }

        internal static string GetIgnorePath(string filePath)
        {
            var solutionPath = VsixHelper.GetSolutionPath()
                ?? throw new Exception("error: could not get solution path");

            var directoryPath = Path.GetDirectoryName(filePath)
                ?? throw new Exception($"error: could not get directory for file {filePath}");

            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, solutionPath, ResolveIgnorePath);
        }

        internal static string GetLocalConfigPath(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath)
                ?? throw new Exception($"error: could not get directory for file {filePath}");

            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, workingDirectory.Root.FullName, ResolveConfigPath);
        }

        internal static string GetLocalEslintPath(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath)
                ?? throw new Exception($"error: could not get directory for file {filePath}");

            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, workingDirectory.Root.FullName, ResolveEslintPath);
        }

        internal static string GetPersonalConfigPath()
        {
            var directory = new DirectoryInfo(EnvironmentHelper.GetUserDirectoryPath());

            return ResolveConfigPath(directory);
        }

        private static string FindRecursive(DirectoryInfo workingDirectory, string end, Func<DirectoryInfo, string> fn)
        {
            do
            {
                var found = fn(workingDirectory);

                if (null != found)
                    return found;

                workingDirectory = workingDirectory.Parent;

                if (null == workingDirectory)
                    return null;
            } while (-1 != workingDirectory.FullName.IndexOf(end, StringComparison.OrdinalIgnoreCase));

            return null;
        }

        private static string ResolveConfigPath(FileSystemInfo directory)
        {
            return SupportedConfigs
                .Select(config => Path.Combine(directory.FullName, config))
                .FirstOrDefault(File.Exists);
        }

        private static string ResolveEslintPath(DirectoryInfo directory)
        {
            var directories = directory.EnumerateDirectories("node_modules", SearchOption.TopDirectoryOnly);

            return directories
                .Select(subDirectory => subDirectory.EnumerateFiles(ExecutableName, SearchOption.AllDirectories))
                .Select(executables => executables.FirstOrDefault()?.FullName)
                .FirstOrDefault();
        }

        private static string ResolveIgnorePath(DirectoryInfo directory)
        {
            return directory
                .EnumerateFiles(".eslintignore", SearchOption.TopDirectoryOnly)
                .FirstOrDefault()?
                .FullName;
        }
    }
}