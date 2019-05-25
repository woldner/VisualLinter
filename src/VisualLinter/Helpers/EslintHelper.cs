using Microsoft.VisualStudio.Shell;
using System;
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


        internal static FileInfo GetConfigInfo(string relativePath)
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

                return new FileInfo(overridePath);
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

            // resolve eslint config path
            var cwd = new DirectoryInfo(relativePath);

            var end = VsixHelper.GetSolutionPath() ??
                throw new Exception("exception: could not get solution path");

            var info = FindRecursive(cwd, end, ResolveConfigPath);
            if (null == info)
                OutputWindowHelper.DebugLine($"could not resolve eslint config relative to '{relativePath}', handing over config resolving to eslint"
                    + Environment.NewLine
                    + "WARNING! this could result in unexpected behavior");

            return info;
        }

        internal static FileInfo GetExecutableInfo(string relativePath)
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

                return new FileInfo(overridePath);
            }

            OutputWindowHelper.DebugLine($"UseGlobalEslint: {Options.UseGlobalEslint}");

            // resolve global eslint path
            if (Options.UseGlobalEslint)
            {
                var globalPath = GetGlobalEslintPath() ??
                    throw new Exception("exception: no global eslint found-- is eslint installed globally?");

                OutputWindowHelper.DebugLine($"using global eslint @ {globalPath}");

                return new FileInfo(globalPath);
            }

            // resolve local eslint path
            var info = GetLocalEslintPath(relativePath) ??
                throw new Exception("exception: no local eslint found-- is eslint installed locally?");

            OutputWindowHelper.DebugLine($"using local eslint @ {info.FullName}");

            return info;
        }

        internal static FileInfo GetIgnoreInfo(string relativePath)
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

                return new FileInfo(overridePath);
            }

            // resolve eslint ignore path
            var cwd = new DirectoryInfo(relativePath);

            var end = VsixHelper.GetSolutionPath() ??
                throw new Exception("exception: could not get solution path");

            var info = FindRecursive(cwd, end, ResolveIgnorePath);

            OutputWindowHelper.DebugLine($"using eslint ignore @ {info?.FullName ?? "not found"}");

            return info;
        }

        private static FileInfo FindRecursive(DirectoryInfo cwd, string end, Func<DirectoryInfo, FileInfo> fn)
        {
            do
            {
                var info = fn(cwd);

                if (null != info)
                    return info;

                cwd = cwd.Parent;

                if (null == cwd)
                    return null;

            } while (-1 != cwd.FullName.IndexOf(end, StringComparison.OrdinalIgnoreCase));

            return null;
        }

        private static string GetGlobalEslintPath()
        {
            return EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.User) ??
                EnvironmentHelper.GetVariable(VariableName, EnvironmentVariableTarget.Machine);
        }

        private static FileInfo GetLocalEslintPath(string relativePath)
        {
            var cwd = new DirectoryInfo(relativePath);
            var end = cwd.Root.FullName;

            return FindRecursive(cwd, end, ResolveEslintPath);
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

        private static FileInfo GetPersonalConfigPath()
        {
            var directory = new DirectoryInfo(EnvironmentHelper.GetUserDirectoryPath());

            return ResolveConfigPath(directory);
        }

        private static FileInfo ResolveConfigPath(FileSystemInfo directory)
        {
            var fileName = SupportedConfigs
                .Select(config =>
                    Path.Combine(directory.FullName, config))
                .FirstOrDefault(File.Exists);

            // let eslint handle config
            if (null == fileName)
                return null;

            return new FileInfo(fileName);
        }

        private static FileInfo ResolveEslintPath(DirectoryInfo directory)
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
                    file.FirstOrDefault())
                .SingleOrDefault();

            return executable;
        }

        private static FileInfo ResolveIgnorePath(DirectoryInfo directory)
        {
            return directory
                .EnumerateFiles(".eslintignore", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();
        }
    }
}
