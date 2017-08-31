using System;
using System.IO;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class LinterHelper
    {
        private static readonly string[] SupportedConfigs =
        {
            ".eslintrc.js",
            ".eslintrc.yaml",
            ".eslintrc.yml",
            ".eslintrc.json",
            ".eslintrc"
        };

        internal static string GetLocalConfigPath(string directoryPath)
        {
            var directory = new DirectoryInfo(directoryPath);

            while (null != directory && directory.Root.Name != directory.Name)
            {
                var config = FindConfig(directory.FullName);

                if (null != config)
                    return config;

                directory = directory.Parent;
            }

            return null;
        }

        internal static string GetPersonalConfigPath()
        {
            var userPath = GetUserDirectoryPath()
                ?? throw new Exception("error: unable to get user directory.");

            return FindConfig(userPath);
        }

        private static string FindConfig(string directoryPath)
        {
            return SupportedConfigs
                .Select(config => Path.Combine(directoryPath, config))
                .FirstOrDefault(File.Exists);
        }

        private static string GetUserDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }
}