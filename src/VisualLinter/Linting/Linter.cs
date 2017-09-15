using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal async Task<IEnumerable<EslintMessage>> LintAsync(string filePath)
        {
            try
            {
                var eslintPath = GetEslintPath(filePath);
                //OutputWindowHelper.WriteLine($"info: using linter @ '{eslintPath}'.");

                var configPath = GetConfigPath(filePath);
                //OutputWindowHelper.WriteLine($"info: using config @ '{configPath}'.");

                var results = await ExecuteProcessAsync(eslintPath, GetArguments(configPath));

                return ProcessResults(results);
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return Enumerable.Empty<EslintMessage>();
        }

        private static async Task<IEnumerable<EslintResult>> ExecuteProcessAsync(string filePath, string arguments)
        {
            var startInfo = new ProcessStartInfo(filePath, arguments)
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false
            };

            using (var process = Process.Start(startInfo))
            {
                if (null == process)
                    throw new Exception("fatal: unable to start eslint process.");

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();

                if (false == string.IsNullOrEmpty(error))
                    OutputWindowHelper.WriteLine(error);

                await process.WaitForExitAsync();

                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<EslintResult>>(output);
                }
                catch (Exception e)
                {
                    OutputWindowHelper.WriteLine(e.Message);
                    OutputWindowHelper.WriteLine(output);
                }

                throw new Exception($"fatal: could not lint file '{filePath}'.");
            }
        }

        private static string GetArguments(string configPath)
        {
            return $"--config \"{configPath}\" --format json --stdin";
        }

        private static IEnumerable<EslintMessage> ProcessResults(IEnumerable<EslintResult> results)
        {
            // this extension only support 1-1 linting
            // therefor results count will always be 1
            var result = results.FirstOrDefault();

            return null != result
                ? result.Messages
                : Enumerable.Empty<EslintMessage>();
        }

        private string GetConfigPath(string filePath)
        {
            if (_options.UsePersonalConfig)
                return EslintHelper.GetPersonalConfigPath()
                    ?? throw new Exception("fatal: no personal eslint config found.");

            return EslintHelper.GetLocalConfigPath(filePath)
                ?? throw new Exception("fatal: no local eslint config found.");
        }

        private string GetEslintPath(string filePath)
        {
            if (_options.UseGlobalEslint)
                return EslintHelper.GetGlobalEslintPath()
                    ?? throw new Exception(
                        "fatal: no global eslint executable found. is eslint installed globally?");

            return EslintHelper.GetLocalEslintPath(filePath)
                ?? throw new Exception("fatal: no local eslint executable found. is eslint installed locally?");
        }
    }
}