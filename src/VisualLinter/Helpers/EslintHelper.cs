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

        internal static string GetArguments(string directoryPath)
        {
            var arguments = new Dictionary<string, string> { { "format", "json" } };

            var configPath = GetConfigPath(directoryPath);
            arguments.Add("config", configPath);

            var ignorePath = GetIgnorePath(directoryPath);
            if (string.IsNullOrEmpty(ignorePath))
                return FormatArguments(arguments);

            arguments.Add("ignore-path", ignorePath);
            return FormatArguments(arguments);
        }

        internal static string GetConfigPath(string directoryPath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideEslintConfig: {Options.ShouldOverrideEslintConfig}");

            // resolve override eslint config path
            if (Options.ShouldOverrideEslintConfig)
            {
                var overridePath = GetOverrideEslintConfigPath() ??
                    throw new Exception("exception: option 'Override .eslintignore path' is set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override eslint config @ {overridePath}");

                return ValidateOverridePath(overridePath);
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

            // resolve local eslint config
            var localPath = GetLocalConfigPath(directoryPath) ??
                throw new Exception("exception: no local eslint config found");

            OutputWindowHelper.DebugLine($"using local eslint config @ {localPath}");

            return localPath;
        }

        internal static string GetEslintPath(string directoryPath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideEslint: {Options.ShouldOverrideEslint}");

            // resolve override eslint path
            if (Options.ShouldOverrideEslint)
            {
                var overridePath = GetOverrideEslintPath() ??
                    throw new Exception("exception: option 'Override ESLint path' set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override eslint @ {overridePath}");

                return ValidateOverridePath(overridePath);
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
            var localPath = GetLocalEslintPath(directoryPath) ??
                throw new Exception("exception: no local eslint found-- is eslint installed locally?");

            OutputWindowHelper.DebugLine($"using local eslint @ {localPath}");

            return localPath;
        }

        internal static string GetIgnorePath(string directoryPath)
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

                return ValidateOverridePath(overridePath);
            }

            // resolve eslint ignore path
            var solutionPath = VsixHelper.GetSolutionPath() ??
                throw new Exception("exception: could not get solution path");

            var workingDirectory = new DirectoryInfo(directoryPath);

            var path = FindRecursive(workingDirectory, solutionPath, ResolveIgnorePath);

            OutputWindowHelper.DebugLine($"using eslint ignore @ {path ?? "not found"}");

            return path;
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

        private static string FormatArguments(IReadOnlyDictionary<string, string> arguments)
        {
            return string.Join(" ", arguments.Select(arg => $"--{arg.Key}=\"{arg.Value}\""));
        }

        private static string GetGlobalEslintPath()
        {
            return EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.User) ??
                EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.Machine);
        }

        private static string GetLocalConfigPath(string directoryPath)
        {
            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, workingDirectory.Root.FullName, ResolveConfigPath);
        }

        private static string GetLocalEslintPath(string directoryPath)
        {
            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, workingDirectory.Root.FullName, ResolveEslintPath);
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

        private static string ValidateOverridePath(string filePath)
        {
            if (false == File.Exists(filePath))
                throw new FileNotFoundException($"exception: could not find file '{filePath}'");

            return filePath;
        }
    }
}
