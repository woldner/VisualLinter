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

        internal async Task<IEnumerable<LinterMessage>> LintAsync(string filePath, string source)
        {
            try
            {
                if (0 == source.Length)
                    return Enumerable.Empty<LinterMessage>();

                var linterPath = GetLinterPath(filePath);
                //OutputWindowHelper.WriteLine($"info: using linter @ '{linterPath}'.");

                var configPath = GetConfigPath(filePath);
                //OutputWindowHelper.WriteLine($"info: using config @ '{configPath}'.");

                var results = await ExecuteProcessAsync(linterPath, GetArguments(configPath), source);

                return ProcessResults(results);
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return Enumerable.Empty<LinterMessage>();
        }

        private static async Task<IEnumerable<LinterResult>> ExecuteProcessAsync(string fileName, string arguments, string source)
        {
            var startInfo = new ProcessStartInfo(fileName, arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using (var process = Process.Start(startInfo))
            {
                if (null == process)
                    throw new Exception("fatal: unable to start eslint process.");

                using (var stream = new StreamWriter(process.StandardInput.BaseStream, new UTF8Encoding(false)))
                {
                    await stream.WriteAsync(source);
                }

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();

                if (false == string.IsNullOrEmpty(error))
                    OutputWindowHelper.WriteLine(error);

                process.WaitForExit();

                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<LinterResult>>(output);
                }
                catch (Exception e)
                {
                    OutputWindowHelper.WriteLine(e.Message);
                    OutputWindowHelper.WriteLine(output);
                }

                throw new Exception($"fatal: eslint could not lint file '{fileName}'.");
            }
        }

        private static string GetArguments(string configPath)
        {
            return $"--config \"{configPath}\" --format json --stdin";
        }

        private static IEnumerable<LinterMessage> ProcessResults(IEnumerable<LinterResult> results)
        {
            // this extension only support 1-1 linting
            // therefor results count will always be 1
            var result = results.FirstOrDefault();

            return null != result
                ? result.Messages
                : Enumerable.Empty<LinterMessage>();
        }

        private string GetConfigPath(string filePath)
        {
            if (_options.UsePersonalConfig)
                return LinterHelper.GetPersonalConfigPath()
                    ?? throw new Exception("fatal: no personal eslint config found.");

            return LinterHelper.GetLocalConfigPath(filePath)
                ?? throw new Exception("fatal: no local eslint config found.");
        }

        private string GetLinterPath(string filePath)
        {
            if (_options.UseGlobalLinter)
                return LinterHelper.GetGlobalLinterPath()
                    ?? throw new Exception("fatal: no global eslint executable found. is eslint installed globally?");

            return LinterHelper.GetLocalLinterPath(filePath)
                ?? throw new Exception("fatal: no local eslint executable found. is eslint installed locally?");
        }
    }
}