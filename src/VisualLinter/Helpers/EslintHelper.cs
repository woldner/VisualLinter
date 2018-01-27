using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class EslintHelper
    {
        private static readonly IVisualLinterOptions Options;

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

        static EslintHelper()
        {
            Options = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>() ??
                throw new Exception("exception: helper unable to retrieve options");
        }

        private static string GetGlobalEslintPath()
        {
            return EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.User) ??
                EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.Machine);
        }

        internal static string GetEslintPath(string filePath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideEslint: {Options.ShouldOverrideEslint}");

            // override eslint path
            if (Options.ShouldOverrideEslint)
            {
                var overridePath = GetOverrideEslintPath() ??
                    throw new Exception("exception: option 'Override ESLint path' set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override eslint @ {overridePath}");

                return overridePath;
            }

            OutputWindowHelper.DebugLine($"UseGlobalEslint: {Options.UseGlobalEslint}");

            // resolve global eslint path
            if (Options.UseGlobalEslint)
            {
                var globalPath = GetGlobalEslintPath() ??
                    throw new Exception("exception: no global eslint found-- is eslint installed globally?"); ;

                OutputWindowHelper.DebugLine($"using global eslint @ {globalPath}");

                return globalPath;
            }

            // resolve local eslint path
            var localPath = GetLocalEslintPath(filePath) ??
                throw new Exception("exception: no local eslint found-- is eslint installed locally?");

            OutputWindowHelper.DebugLine($"using local eslint @ {localPath}");

            return localPath;
        }

        private static string GetOverrideEslintPath()
        {
            var overridePath = string.IsNullOrEmpty(Options.EslintOverridePath)
                ? null
                : Options.EslintOverridePath;

            OutputWindowHelper.DebugLine($"EslintOverridePath: {overridePath ?? "null"}");

            return overridePath;
        }

        internal static string GetIgnorePath(string filePath)
        {
            var solutionPath = VsixHelper.GetSolutionPath() ??
                throw new Exception("exception: could not get solution path");

            var directoryPath = Path.GetDirectoryName(filePath) ??
                throw new Exception($"exception: could not get directory for file {filePath}");

            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, solutionPath, ResolveIgnorePath);
        }

        internal static string GetLocalConfigPath(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath) ??
                throw new Exception($"exception: could not get directory for file {filePath}");

            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, workingDirectory.Root.FullName, ResolveConfigPath);
        }

        internal static string GetLocalEslintPath(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath) ??
                throw new Exception($"exception: could not get directory for file {filePath}");

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
                .Select(config =>
                    Path.Combine(directory.FullName, config))
                .FirstOrDefault(File.Exists);
        }

        private static string ResolveEslintPath(DirectoryInfo directory)
        {
            // todo optimize if this is what we want
            var modulesPath = directory
                .EnumerateDirectories("node_modules", SearchOption.TopDirectoryOnly);

            var binPath = modulesPath
                .Select(dir =>
                    dir.EnumerateDirectories(".bin", SearchOption.TopDirectoryOnly))
                .FirstOrDefault();

            var executables = binPath?
                .Select(dir =>
                    dir.EnumerateFiles(ExecutableName, SearchOption.TopDirectoryOnly));

            var executable = executables?
                .Select(file =>
                    file.FirstOrDefault()?.FullName);

            return executable?.FirstOrDefault();
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
