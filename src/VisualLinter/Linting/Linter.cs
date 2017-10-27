using jwldnr.VisualLinter.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter.Linting
{
    internal class Linter
    {
        private readonly IVisualLinterOptions _options;

        internal Linter(IVisualLinterOptions options)
        {
            _options = options;
        }

        internal async Task<IEnumerable<EslintMessage>> LintAsync(string filePath, string source)
        {
            try
            {
                if (0 == source.Length)
                    return Enumerable.Empty<EslintMessage>();

                var eslintPath = GetEslintPath(filePath);
                Debug.WriteLine($"using eslint @ {eslintPath}");

                var arguments = GetArguments(filePath);
                var results = await ExecuteProcessAsync(eslintPath, arguments, source);

                return ProcessResults(results);
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return Enumerable.Empty<EslintMessage>();
        }

        private static async Task<IEnumerable<EslintResult>> ExecuteProcessAsync(string filePath, string arguments, string source)
        {
            var startInfo = new ProcessStartInfo(filePath, arguments)
            {
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false
            };

            using (var process = Process.Start(startInfo))
            {
                if (null == process)
                    throw new Exception("fatal: unable to start eslint process");

                using (var stream = new StreamWriter(process.StandardInput.BaseStream, new UTF8Encoding(false)))
                {
                    await stream.WriteAsync(source);
                }

                var output = await process.StandardOutput.ReadToEndAsync();
                if (null == output.NullIfEmpty())
                    throw new Exception($"fatal: eslint process returned empty. {Environment.NewLine}"
                        + $"this is most likely due to incorrect values set in options. {Environment.NewLine}"
                        + "please check your override paths in advanced options.");

                var error = await process.StandardError.ReadToEndAsync();
                if (null != error.NullIfEmpty())
                    throw new Exception(error);

                process.WaitForExit();

                return JsonConvert.DeserializeObject<IEnumerable<EslintResult>>(output);
            }
        }

        private static string GetGlobalEslintPath()
        {
            return EslintHelper.GetGlobalEslintPath()
                ?? throw new Exception("fatal: no global eslint found-- is eslint installed globally?");
        }

        private static string GetPersonalConfigPath()
        {
            return EslintHelper.GetPersonalConfigPath()
                ?? throw new Exception("fatal: no personal eslint config found");
        }

        private static IEnumerable<EslintMessage> ProcessMessages(IReadOnlyList<EslintMessage> messages)
        {
            // return empty messages when warning about ignored files
            if (1 == messages.Count && RegexHelper.IgnoreFileMatch(messages[0].Message))
                return Enumerable.Empty<EslintMessage>();

            return messages;
        }

        private static IEnumerable<EslintMessage> ProcessResults(IEnumerable<EslintResult> results)
        {
            // this extension only support 1-1 linting
            // therefor results count will always be 1
            var result = results.FirstOrDefault();

            return null != result
                ? ProcessMessages(result.Messages)
                : Enumerable.Empty<EslintMessage>();
        }

        private string GetArguments(string filePath)
        {
            var configPath = GetConfigPath(filePath);
            Debug.WriteLine($"using eslint configuration @ {configPath}");

            var arguments = $"--stdin --stdin-filename \"{filePath}\" --format json --config \"{configPath}\"";

            if (_options.DisableIgnorePath)
                return arguments;

            var ignorePath = GetIgnorePath(filePath);
            Debug.WriteLine($"using .eslintignore @ {ignorePath}");

            return $"{arguments} --ignore-path \"{ignorePath}\"";
        }

        private string GetConfigPath(string filePath)
        {
            if (_options.ShouldOverrideEslintConfig)
                return GetOverrideEslintConfigPath();

            if (_options.UsePersonalConfig)
                return GetPersonalConfigPath();

            return EslintHelper.GetLocalConfigPath(filePath)
                ?? throw new Exception("fatal: no local eslint config found");
        }

        private string GetEslintPath(string filePath)
        {
            if (_options.ShouldOverrideEslint)
                return GetOverrideEslintPath();

            if (_options.UseGlobalEslint)
                return GetGlobalEslintPath();

            return EslintHelper.GetLocalEslintPath(filePath)
                ?? throw new Exception("fatal: no local eslint found-- is eslint installed locally?");
        }

        private string GetIgnorePath(string filePath)
        {
            return _options.ShouldOverrideEslintIgnore
                ? GetOverrideEslintIgnorePath()
                : EslintHelper.GetIgnorePath(filePath);
        }

        private string GetOverrideEslintConfigPath()
        {
            return _options.EslintConfigOverridePath.NullIfEmpty()
                ?? throw new Exception("fatal: option 'Override .eslintignore path' is set to true-- but no path is set");
        }

        private string GetOverrideEslintIgnorePath()
        {
            return _options.EslintIgnoreOverridePath.NullIfEmpty()
                ?? throw new Exception("fatal: option 'Override ESLint config path' is set to true-- but no path is set");
        }

        private string GetOverrideEslintPath()
        {
            return _options.EslintOverridePath.NullIfEmpty()
                ?? throw new Exception("fatal: option 'Override ESLint path' set to true-- but no path is set");
        }
    }
}