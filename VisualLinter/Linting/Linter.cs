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
        private const string ExecutableName = "eslint.cmd";
        private const string Name = "eslint";

        private readonly IVisualLinterOptions _options;

        internal Linter(IVisualLinterOptions options)
        {
            _options = options;
        }

        internal static string GetLocalLinterPath()
        {
            try
            {
                var solutionDirectory = new DirectoryInfo(VsixHelper.GetSolutionPath());
                var linterFile = solutionDirectory.GetFiles(ExecutableName, SearchOption.AllDirectories)
                    .FirstOrDefault();

                return linterFile?.FullName;
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
        }

        internal async Task<IEnumerable<LinterMessage>> LintAsync(string filePath)
        {
            try
            {
                var linterPath = GetLinterPath();
                var configPath = GetConfigPath(filePath);

                var results = await ExecuteProcessAsync(linterPath, GetArguments(configPath, filePath))
                    ?? throw new Exception("fatal: eslint returned null result.");

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
                    throw new Exception("fatal: unable to start eslint process.");

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

                return null;
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
            try
            {
                return EnvironmentHelper.GetVariable(Name, EnvironmentVariableTarget.User)
                    ?? EnvironmentHelper.GetVariable(Name, EnvironmentVariableTarget.Machine);
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
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

            return null != result
                ? result.Messages
                : Enumerable.Empty<LinterMessage>();
        }

        private string GetConfigPath(string filePath)
        {
            if (_options.UseGlobalConfig)
                return GetGlobalConfigPath()
                    ?? throw new Exception("fatal: no global eslint config found.");

            return GetLocalConfigPath(filePath)
                ?? throw new Exception("fatal: no local eslint config found.");
        }

        private string GetLinterPath()
        {
            if (_options.UseGlobalLinter)
                return GetGlobalLinterPath()
                    ?? throw new Exception("fatal: no global eslint executable found. is eslint installed globally?");

            return GetLocalLinterPath()
                ?? throw new Exception("fatal: no local eslint executable found. is eslint installed locally?");
        }
    }
}