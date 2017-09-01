using System;
using System.IO;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class LinterHelper
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

        internal static string GetGlobalLinterPath()
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
            var directoryPath = Path.GetDirectoryName(filePath);

            return null != directoryPath
                ? FindRecursive(directoryPath, FindConfig)
                : null;
        }

        internal static string GetLocalLinterPath(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);

            return null != directoryPath
                ? FindRecursive(directoryPath, FindExecutable)
                : null;
        }

        internal static string GetPersonalConfigPath()
        {
            var directory = new DirectoryInfo(GetUserDirectoryPath());

            return FindConfig(directory);
        }

        private static string FindConfig(FileSystemInfo directory)
        {
            return SupportedConfigs
                .Select(config => Path.Combine(directory.FullName, config))
                .FirstOrDefault(File.Exists);
        }

        private static string FindExecutable(DirectoryInfo directory)
        {
            var directories = directory.EnumerateDirectories("node_modules", SearchOption.TopDirectoryOnly);

            return directories
                .Select(subDirectory => subDirectory.EnumerateFiles(ExecutableName, SearchOption.AllDirectories))
                .Select(executables => executables.FirstOrDefault()?.FullName)
                .FirstOrDefault();
        }

        private static string FindRecursive(string path, Func<DirectoryInfo, string> findFile)
        {
            var directory = new DirectoryInfo(path);

            while (null != directory && directory.Root.Name != directory.Name)
            {
                var file = findFile(directory);

                if (null != file)
                    return file;

                directory = directory.Parent;
            }

            return null;
        }

        private static string GetUserDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }
}