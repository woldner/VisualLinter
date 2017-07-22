using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Process = System.Diagnostics.Process;

namespace jwldnr.VisualLinter.Services
{
    [Export(typeof(ILinterService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class LinterService : ILinterService
    {
        private const string Name = "eslint";

        private readonly IVisualLinterOptions _options;

        [ImportingConstructor]
        internal LinterService([Import] IVisualLinterOptions options)
        {
            _options = options;
        }

        public async Task<IEnumerable<LinterMessage>> LintAsync(string filePath)
        {
            var eslintPath = GetGlobalPath();

            try
            {
                if (null == eslintPath)
                    throw new ArgumentNullException(nameof(eslintPath));

                var results = await ExecuteProcessAsync(eslintPath, GetArguments(filePath));
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
                catch (Exception e)
                {
                    OutputWindowHelper.WriteLine(e.Message);
                }

                return Enumerable.Empty<LinterResult>();
            }
        }

        private static string GetGlobalPath()
        {
            var path = EnvironmentHelper.GetVariable(Name, EnvironmentVariableTarget.User);
            return path ?? EnvironmentHelper.GetVariable(Name, EnvironmentVariableTarget.Machine);
        }

        private static string GetLocalConfigPath()
        {
            var path = VsixHelper.GetSolutionPath();
            if (null == path)
                return null;

            try
            {
                return Directory.GetFiles(path, ".eslintrc.*", SearchOption.AllDirectories)
                    .SingleOrDefault();
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

        private string GetAdditionalArguments()
        {
            if (false == _options.UseLocalConfig)
                return string.Empty;

            var localConfigPath = GetLocalConfigPath();
            if (null != localConfigPath)
                return $" --config \"{localConfigPath}\"";

            OutputWindowHelper.WriteLine("Could not find local ESLint config.");

            return string.Empty;
        }

        private string GetArguments(string filePath)
        {
            var arguments = $"--format json \"{filePath}\"";

            arguments += GetAdditionalArguments();

            return arguments;
        }
    }
}