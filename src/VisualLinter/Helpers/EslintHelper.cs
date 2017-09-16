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
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
        }

        internal static string GetLocalConfigPath(string filePath)
        {
            return GetRecursive(filePath, ResolveConfigPath);
        }

        internal static string GetLocalEslintPath(string filePath)
        {
            return GetRecursive(filePath, ResolveEslintPath);
        }

        internal static string GetIgnorePath(string filePath)
        {
            var projectPath = VsixHelper.GetProjectPath(filePath)
                ?? throw new Exception($"error: could not get project path using file '{filePath}'.");

            var projectDirectory = new DirectoryInfo(projectPath);

            var directoryPath = Path.GetDirectoryName(filePath)
                ?? throw new Exception($"error: could not get directory path using file '{filePath}'.");

            var workingDirectory = new DirectoryInfo(directoryPath);

            do
            {
                var foundFile = ResolveIgnorePath(workingDirectory);

                if (null != foundFile)
                    return foundFile;

                workingDirectory = workingDirectory.Parent;
            } while (null != workingDirectory && projectDirectory.FullName != workingDirectory.FullName);

            return null;
        }

        internal static string GetPersonalConfigPath()
        {
            var directory = new DirectoryInfo(GetUserDirectoryPath());

            return ResolveConfigPath(directory);
        }

        private static string ResolveConfigPath(FileSystemInfo directory)
        {
            return SupportedConfigs
                .Select(config => Path.Combine(directory.FullName, config))
                .FirstOrDefault(File.Exists);
        }

        private static string ResolveIgnorePath(DirectoryInfo directory)
        {
            return directory
                .EnumerateFiles(".eslintignore", SearchOption.TopDirectoryOnly)
                .FirstOrDefault()?
                .FullName;
        }

        private static string ResolveEslintPath(DirectoryInfo directory)
        {
            var directories = directory.EnumerateDirectories("node_modules", SearchOption.TopDirectoryOnly);

            return directories
                .Select(subDirectory => subDirectory.EnumerateFiles(ExecutableName, SearchOption.AllDirectories))
                .Select(executables => executables.FirstOrDefault()?.FullName)
                .FirstOrDefault();
        }

        private static string GetRecursive(string filePath, Func<DirectoryInfo, string> fn)
        {
            var directoryPath = Path.GetDirectoryName(filePath)
                ?? throw new Exception($"error: could not get directory path using file '{filePath}'.");

            var workingDirectory = new DirectoryInfo(directoryPath);

            do
            {
                var foundFile = fn(workingDirectory);

                if (null != foundFile)
                    return foundFile;

                workingDirectory = workingDirectory.Parent;
            } while (null != workingDirectory && workingDirectory.Root.FullName != workingDirectory.FullName);

            return null;
        }

        private static string GetUserDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }
}