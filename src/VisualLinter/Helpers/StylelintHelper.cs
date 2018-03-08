using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class StylelintHelper
    {
        private const string ExecutableName = "stylelint.cmd";
        private const string VariableName = "stylelint";
        private static readonly IVisualLinterOptions Options;

        private static readonly string[] SupportedConfigs =
        {
            ".stylelintrc.js",
            ".stylelintrc.yaml",
            ".stylelintrc.yml",
            ".stylelintrc.json",
            ".stylelintrc"
        };

        static StylelintHelper()
        {
            Options = ServiceProvider.GlobalProvider.GetMefService<IVisualLinterOptions>() ??
                throw new Exception("exception: helper unable to retrieve options");
        }

        internal static string GetArguments(string directoryPath)
        {
            var arguments = new Dictionary<string, string> { { "formatter", "json" } };

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
            OutputWindowHelper.DebugLine($"ShouldOverrideStylelintConfigPath: {Options.ShouldOverrideStylelintConfigPath}");

            // resolve override stylelint config path
            if (Options.ShouldOverrideStylelintConfigPath)
            {
                var overridePath = GetOverrideStylelintConfigPath() ??
                    throw new Exception("exception: option 'Override .stylelintignore path' is set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override stylelint config @ {overridePath}");

                return ValidateOverridePath(overridePath);
            }

            OutputWindowHelper.DebugLine($"UsePersonalStylelintConfig: {Options.UsePersonalStylelintConfig}");

            // resolve personal config path
            if (Options.UsePersonalStylelintConfig)
            {
                var globalPath = GetPersonalConfigPath() ??
                    throw new Exception("exception: no personal stylelint config found");

                OutputWindowHelper.DebugLine($"using personal stylelint config @ {globalPath}");

                return globalPath;
            }

            // resolve local stylelint config
            var localPath = GetLocalConfigPath(directoryPath) ??
                throw new Exception("exception: no local stylelint config found");

            OutputWindowHelper.DebugLine($"using local stylelint config @ {localPath}");

            return localPath;
        }

        internal static string GetStylelintPath(string directoryPath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideStylelintPath: {Options.ShouldOverrideStylelintPath}");

            // resolve override stylelint path
            if (Options.ShouldOverrideStylelintPath)
            {
                var overridePath = GetOverrideStylelintPath() ??
                    throw new Exception("exception: option 'Override Stylelint path' set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override stylelint @ {overridePath}");

                return ValidateOverridePath(overridePath);
            }

            OutputWindowHelper.DebugLine($"UseGlobalStylelint: {Options.UseGlobalStylelint}");

            // resolve global stylelint path
            if (Options.UseGlobalStylelint)
            {
                var globalPath = GetGlobalStylelintPath() ??
                    throw new Exception("exception: no global stylelint found-- is stylelint installed globally?");

                OutputWindowHelper.DebugLine($"using global stylelint @ {globalPath}");

                return globalPath;
            }

            // resolve local stylelint path
            var localPath = GetLocalStylelintPath(directoryPath) ??
                throw new Exception("exception: no local stylelint found-- is stylelint installed locally?");

            OutputWindowHelper.DebugLine($"using local stylelint @ {localPath}");

            return localPath;
        }

        internal static string GetIgnorePath(string directoryPath)
        {
            OutputWindowHelper.DebugLine($"DisableStylelintIgnore: {Options.DisableStylelintIgnore}");

            // disable stylelint ignore
            if (Options.DisableStylelintIgnore)
            {
                OutputWindowHelper.DebugLine("not using stylelint ignore");

                return null;
            }

            OutputWindowHelper.DebugLine($"ShouldOverrideStylelintIgnorePath: {Options.ShouldOverrideStylelintIgnorePath}");

            // resolve override stylelint ignore path
            if (Options.ShouldOverrideStylelintIgnorePath)
            {
                var overridePath = GetOverrideStylelintIgnore() ??
                    throw new Exception("exception: option 'Override Stylelint ignore path' is set to true-- but no path is set");

                OutputWindowHelper.DebugLine($"using override stylelint ignore @ {overridePath}");

                return ValidateOverridePath(overridePath);
            }

            // resolve stylelint ignore path
            var solutionPath = VsixHelper.GetSolutionPath() ??
                throw new Exception("exception: could not get solution path");

            var workingDirectory = new DirectoryInfo(directoryPath);

            var path = FindRecursive(workingDirectory, solutionPath, ResolveIgnorePath);

            OutputWindowHelper.DebugLine($"using stylelint ignore @ {path ?? "not found"}");

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

        private static string GetGlobalStylelintPath()
        {
            return EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.User) ??
                EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.Machine);
        }

        private static string GetLocalConfigPath(string directoryPath)
        {
            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, workingDirectory.Root.FullName, ResolveConfigPath);
        }

        private static string GetLocalStylelintPath(string directoryPath)
        {
            var workingDirectory = new DirectoryInfo(directoryPath);

            return FindRecursive(workingDirectory, workingDirectory.Root.FullName, ResolveStylelintPath);
        }

        private static string GetOverrideStylelintConfigPath()
        {
            var overridePath = Options.StylelintOverrideConfigPath;

            var path = string.IsNullOrEmpty(overridePath)
                ? null
                : overridePath;

            OutputWindowHelper.DebugLine($"StylelintOverrideConfigPath: {path ?? "null"}");

            return path;
        }

        private static string GetOverrideStylelintIgnore()
        {
            var overridePath = Options.StylelintOverrideIgnorePath;

            var path = string.IsNullOrEmpty(overridePath)
                ? null
                : overridePath;

            OutputWindowHelper.DebugLine($"StylelintOverridePath: {path ?? "null"}");

            return path;
        }

        private static string GetOverrideStylelintPath()
        {
            var overridePath = Options.StylelintOverridePath;

            var path = string.IsNullOrEmpty(overridePath)
                ? null
                : overridePath;

            OutputWindowHelper.DebugLine($"StylelintOverridePath: {path ?? "null"}");

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

        private static string ResolveStylelintPath(DirectoryInfo directory)
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
                .EnumerateFiles(".stylelintignore", SearchOption.TopDirectoryOnly)
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
