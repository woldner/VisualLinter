using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class EslintHelper
    {
        private const string ExecutableName = "eslint.cmd";
        private const string VariableName = "eslint";
        private static readonly IVisualLinterOptions Options;

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


        internal static FileInfo GetConfigPath(string relativePath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideEslintConfig: {Options.ShouldOverrideEslintConfig}");

            // resolve override eslint config path
            if (Options.ShouldOverrideEslintConfig)
            {
                var overridePath = GetOverrideEslintConfigPath() ??
                    throw new Exception("exception: option 'Override .eslintignore path' is set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override eslint config @ {overridePath}");

                if (false == File.Exists(overridePath))
                    throw new FileNotFoundException($"exception: could not find file '{overridePath}'");

                return overridePath;
            }

            OutputWindowHelper.DebugLine($"UsePersonalConfig: {Options.UsePersonalConfig}");

            // resolve personal config path
            if (Options.UsePersonalConfig)
            {
                var globalPath = GetPersonalConfigPath() ??
                    throw new Exception("exception: no personal eslint config found");

                OutputWindowHelper.DebugLine($"using personal eslint config @ {globalPath}");

                return globalPath;
            }

            // let eslint handle configs
            return null;
        }

        internal static string GetEslintPath(string relativePath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideEslint: {Options.ShouldOverrideEslint}");

            // resolve override eslint path
            if (Options.ShouldOverrideEslint)
            {
                var overridePath = GetOverrideEslintPath() ??
                    throw new Exception("exception: option 'Override ESLint path' set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override eslint @ {overridePath}");

                if (false == File.Exists(overridePath))
                    throw new FileNotFoundException($"exception: could not find file '{overridePath}'");

                return overridePath;
            }

            OutputWindowHelper.DebugLine($"UseGlobalEslint: {Options.UseGlobalEslint}");

            // resolve global eslint path
            if (Options.UseGlobalEslint)
            {
                var globalPath = GetGlobalEslintPath() ??
                    throw new Exception("exception: no global eslint found-- is eslint installed globally?");

                OutputWindowHelper.DebugLine($"using global eslint @ {globalPath}");

                return globalPath;
            }

            // resolve local eslint path
            var localPath = GetLocalEslintPath(relativePath) ??
                throw new Exception("exception: no local eslint found-- is eslint installed locally?");

            OutputWindowHelper.DebugLine($"using local eslint @ {localPath}");

            return localPath;
        }

        internal static string GetIgnorePath(string relativePath)
        {
            OutputWindowHelper.DebugLine($"DisableIgnorePath: {Options.DisableIgnorePath}");

            // disable eslint ignore
            if (Options.DisableIgnorePath)
            {
                OutputWindowHelper.DebugLine("not using eslint ignore");

                return null;
            }

            OutputWindowHelper.DebugLine($"ShouldOverrideEslintIgnore: {Options.ShouldOverrideEslintIgnore}");

            // resolve override eslint ignore path
            if (Options.ShouldOverrideEslintIgnore)
            {
                var overridePath = GetOverrideEslintIgnorePath() ??
                    throw new Exception("exception: option 'Override ESLint ignore path' is set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override eslint ignore @ {overridePath}");

                if (false == File.Exists(overridePath))
                    throw new FileNotFoundException($"exception: could not find file '{overridePath}'");

                return overridePath;
            }

            // resolve eslint ignore path
            var solutionPath = VsixHelper.GetSolutionPath() ??
                throw new Exception("exception: could not get solution path");

            var directory = new DirectoryInfo(relativePath);

            var path = FindRecursive(directory, solutionPath, ResolveIgnorePath);

            OutputWindowHelper.DebugLine($"using eslint ignore @ {path ?? "not found"}");

            return path;
        }

        private static string FindRecursive(DirectoryInfo directory, string end, Func<DirectoryInfo, string> fn)
        {
            do
            {
                var found = fn(directory);

                if (null != found)
                    return found;

                directory = directory.Parent;

                if (null == directory)
                    return null;

            } while (-1 != directory.FullName.IndexOf(end, StringComparison.OrdinalIgnoreCase));

            return null;
        }

        private static string GetGlobalEslintPath()
        {
            return EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.User) ??
                EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.Machine);
        }

        private static string GetLocalEslintPath(string relativePath)
        {
            var directory = new DirectoryInfo(relativePath);

            return FindRecursive(directory, directory.Root.FullName, ResolveEslintPath);
        }

        private static string GetOverrideEslintConfigPath()
        {
            var overridePath = Options.EslintConfigOverridePath;

            var path = string.IsNullOrEmpty(overridePath)
                ? null
                : overridePath;

            OutputWindowHelper.DebugLine($"EslintConfigOverridePath: {path ?? "null"}");

            return path;
        }

        private static string GetOverrideEslintIgnorePath()
        {
            var overridePath = Options.EslintIgnoreOverridePath;

            var path = string.IsNullOrEmpty(overridePath)
                ? null
                : overridePath;

            OutputWindowHelper.DebugLine($"EslintOverridePath: {path ?? "null"}");

            return path;
        }

        private static string GetOverrideEslintPath()
        {
            var overridePath = Options.EslintOverridePath;

            var path = string.IsNullOrEmpty(overridePath)
                ? null
                : overridePath;

            OutputWindowHelper.DebugLine($"EslintOverridePath: {path ?? "null"}");

            return path;
        }

        private static string GetPersonalConfigPath()
        {
            var directory = new DirectoryInfo(EnvironmentHelper.GetUserDirectoryPath());

            return ResolveConfigPath(directory);
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
