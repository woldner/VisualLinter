using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Tagging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter.Linting
{
    public interface ILinter
    {
        Task LintAsync(ILinterProvider provider, string filePath);
    }

    [Export(typeof(ILinter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class Linter : ILinter
    {
        private readonly IVisualLinterOptions _options;

        [ImportingConstructor]
        internal Linter([Import] IVisualLinterOptions options)
        {
            _options = options;
        }

        public async Task LintAsync(ILinterProvider provider, string filePath)
        {
            try
            {
                var eslintPath = GetEslintPath(filePath);
                OutputWindowHelper.DebugLine($"using eslint @ {eslintPath}");

                await ExecuteProcessAsync(provider, filePath, eslintPath)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                OutputWindowHelper.WriteLine(exception.Message);
            }
        }

        private static string GetGlobalEslintPath()
        {
            return EslintHelper.GetGlobalEslintPath()
                ?? throw new Exception("exception: no global eslint found-- is eslint installed globally?");
        }

        private static string GetPersonalConfigPath()
        {
            return EslintHelper.GetPersonalConfigPath()
                ?? throw new Exception("exception: no personal eslint config found");
        }

        private static void OnErrorDataReceived(DataReceivedEventArgs e, ILinterProvider provider, string filePath)
        {
            var result = e.Data;
            if (null == result.NullIfEmpty())
                return;

            try
            {
                OutputWindowHelper.WriteLine(result);

                provider.Accept(filePath, Enumerable.Empty<EslintMessage>());
            }
            catch (Exception exception)
            {
                OutputWindowHelper.WriteLine(exception.Message);
            }
        }

        private static void OnOutputDataReceived(DataReceivedEventArgs e, ILinterProvider provider, string filePath)
        {
            var result = e.Data;
            if (null == result.NullIfEmpty())
                return;

            try
            {
                var results = JsonConvert.DeserializeObject<IEnumerable<EslintResult>>(result);
                var messages = ProcessResults(results);

                provider.Accept(filePath, messages);
            }
            catch (Exception exception)
            {
                OutputWindowHelper.WriteLine(exception.Message);
            }
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

        private Task ExecuteProcessAsync(ILinterProvider provider, string filePath, string eslintPath)
        {
            var arguments = $"{GetArguments(filePath)} \"{filePath}\"";

            var startInfo = new ProcessStartInfo(eslintPath, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            var process = new Process { StartInfo = startInfo };

            process.ErrorDataReceived += (sender, e) => OnErrorDataReceived(e, provider, filePath);
            process.OutputDataReceived += (sender, e) => OnOutputDataReceived(e, provider, filePath);

            return Task.Run(() =>
            {
                try
                {
                    if (false == process.Start())
                        throw new Exception("exception: unable to start eslint process");

                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();

                    process.WaitForExit();
                }
                catch (Exception exception)
                {
                    OutputWindowHelper.WriteLine(exception.Message);
                }
                finally
                {
                    process.Close();
                }
            });
        }

        private string GetArguments(string filePath)
        {
            var configPath = GetConfigPath(filePath);
            OutputWindowHelper.DebugLine($"using eslint configuration @ {configPath}");

            var arguments = $"--format=\"json\" --config=\"{configPath}\"";

            OutputWindowHelper.DebugLine($"DisableIgnorePath: {_options.DisableIgnorePath}");
            if (_options.DisableIgnorePath)
                return arguments;

            var ignorePath = GetIgnorePath(filePath);
            if (null == ignorePath.NullIfEmpty())
                return arguments;

            OutputWindowHelper.DebugLine($"using .eslintignore @ {ignorePath}");
            return $"{arguments} --ignore-path=\"{ignorePath}\"";
        }

        private string GetConfigPath(string filePath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideEslintConfig: {_options.ShouldOverrideEslintConfig}");

            if (_options.ShouldOverrideEslintConfig)
                return GetOverrideEslintConfigPath();

            OutputWindowHelper.DebugLine($"UsePersonalConfig: {_options.UsePersonalConfig}");

            if (_options.UsePersonalConfig)
                return GetPersonalConfigPath();

            return EslintHelper.GetLocalConfigPath(filePath)
                ?? throw new Exception("exception: no local eslint config found");
        }

        private string GetEslintPath(string filePath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideEslint: {_options.ShouldOverrideEslint}");

            if (_options.ShouldOverrideEslint)
                return GetOverrideEslintPath();

            OutputWindowHelper.DebugLine($"UseGlobalEslint: {_options.UseGlobalEslint}");

            if (_options.UseGlobalEslint)
                return GetGlobalEslintPath();

            return EslintHelper.GetLocalEslintPath(filePath)
                ?? throw new Exception("exception: no local eslint found-- is eslint installed locally?");
        }

        private string GetIgnorePath(string filePath)
        {
            OutputWindowHelper.DebugLine($"ShouldOverrideEslintIgnore: {_options.ShouldOverrideEslintIgnore}");

            return _options.ShouldOverrideEslintIgnore
                ? GetOverrideEslintIgnorePath()
                : EslintHelper.GetIgnorePath(filePath);
        }

        private string GetOverrideEslintConfigPath()
        {
            OutputWindowHelper.DebugLine($"EslintConfigOverridePath: {_options.EslintConfigOverridePath.NullIfEmpty() ?? "null"}");

            return _options.EslintConfigOverridePath.NullIfEmpty()
                ?? throw new Exception("exception: option 'Override .eslintignore path' is set to true-- but no path is set");
        }

        private string GetOverrideEslintIgnorePath()
        {
            OutputWindowHelper.DebugLine($"EslintIgnoreOverridePath: {_options.EslintIgnoreOverridePath.NullIfEmpty() ?? "null"}");

            return _options.EslintIgnoreOverridePath.NullIfEmpty()
                ?? throw new Exception("exception: option 'Override ESLint config path' is set to true-- but no path is set");
        }

        private string GetOverrideEslintPath()
        {
            OutputWindowHelper.DebugLine($"EslintOverridePath: {_options.EslintOverridePath.NullIfEmpty() ?? "null"}");

            return _options.EslintOverridePath.NullIfEmpty()
                ?? throw new Exception("exception: option 'Override ESLint path' set to true-- but no path is set");
        }
    }
}