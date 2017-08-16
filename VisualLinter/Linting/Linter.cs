using jwldnr.VisualLinter.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Process = System.Diagnostics.Process;

namespace jwldnr.VisualLinter.Linting
{
    internal class Linter
    {
        private const string Name = "eslint";

        private readonly IVisualLinterOptions _options;

        internal Linter(IVisualLinterOptions options)
        {
            _options = options;
        }

        internal async Task<IEnumerable<LinterMessage>> LintAsync(string filePath)
        {
            try
            {
                var linterPath = GetGlobalLinterPath();
                if (null == linterPath)
                    throw new Exception("linter path cannot be null");

                var configPath = GetConfigPath(filePath);
                if (null == configPath)
                    throw new Exception("config path cannot be null");

                var results = await ExecuteProcessAsync(linterPath, GetArguments(configPath, filePath));
                if (null == results)
                    throw new Exception("linter returned null result");

                return ProcessResults(results);
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return Enumerable.Empty<LinterMessage>();
        }

        private static async Task<IEnumerable<LinterResult>> ExecuteProcessAsync(string fileName, string arguments)
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
                    return null;

                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(error))
                    OutputWindowHelper.WriteLine(error);

                process.WaitForExit();

                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<LinterResult>>(output);
                }
                catch (Exception e)
                {
                    OutputWindowHelper.WriteLine(e.Message);
                }

                return Enumerable.Empty<LinterResult>();
            }
        }

        private static string GetArguments(string configPath, string filePath)
        {
            return $"--config \"{configPath}\" --format json \"{filePath}\"";
        }

        private static string GetGlobalConfigPath()
        {
            try
            {
                return VsixHelper.GetGlobalConfigPath();
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
        }

        private static string GetGlobalLinterPath()
        {
            var path = EnvironmentHelper.GetVariable(Name, EnvironmentVariableTarget.User);
            return path ?? EnvironmentHelper.GetVariable(Name, EnvironmentVariableTarget.Machine);
        }

        private static string GetLocalConfigPath(string filePath)
        {
            try
            {
                return VsixHelper.GetLocalConfigPath(filePath);
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
        }

        private static IEnumerable<LinterMessage> ProcessResults(IEnumerable<LinterResult> results)
        {
            // this extension only support 1-1 linting
            // therefor results count will always be 1
            var result = results.FirstOrDefault();

            return null == result
                ? Enumerable.Empty<LinterMessage>()
                : result.Messages;
        }

        private string GetConfigPath(string filePath)
        {
            return _options.UseGlobalConfig
                ? GetGlobalConfigPath()
                : GetLocalConfigPath(filePath);
        }
    }
}
