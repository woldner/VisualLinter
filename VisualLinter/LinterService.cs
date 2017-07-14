using jwldnr.VisualLinter.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter
{
    internal interface ILinterService
    {
        Task<IEnumerable<LinterMessage>> LintAsync(string filePath);
    }

    [Export(typeof(ILinterService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class LinterService : ILinterService
    {
        private readonly string _eslintPath;
        private readonly IVisualLinterOptions _options;

        [ImportingConstructor]
        internal LinterService([Import] IVisualLinterOptions options)
        {
            _options = options;
            _eslintPath = GetESLintPath();
        }

        public async Task<IEnumerable<LinterMessage>> LintAsync(string filePath)
        {
            try
            {
                var results = await ExecuteProcessAsync(_eslintPath, GetLinterArguments(filePath));
                if (null == results)
                    throw new ArgumentNullException(nameof(results));

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
                catch (Exception)
                {
                    OutputWindowHelper.WriteLine(output);
                }

                return Enumerable.Empty<LinterResult>();
            }
        }

        private static string GetESLintPath()
        {
            const string name = "eslint";

            var path = EnvironmentHelper.GetVariable(name, EnvironmentVariableTarget.User);
            return path ?? EnvironmentHelper.GetVariable(name, EnvironmentVariableTarget.Machine);
        }

        private static string GetLinterArguments(string filePath)
        {
            return $"--format json \"{filePath}\"";
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
    }
}