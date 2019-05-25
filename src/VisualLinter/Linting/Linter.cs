using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Tagging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter.Linting
{
    public interface ILinter
    {
        Task LintAsync(ILinterProvider provider, string filePath, CancellationToken token);
    }

    [Export(typeof(ILinter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class Linter : ILinter
    {
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public async Task LintAsync(ILinterProvider provider, string filePath, CancellationToken token)
        {
            try
            {
                await _mutex.WaitAsync(token).ConfigureAwait(false);

                try
                {
                    token.ThrowIfCancellationRequested();

                    var relativePath = Path.GetDirectoryName(filePath) ??
                        throw new Exception($"exception: could not get directory for file {filePath}");

                    var executable = EslintHelper.GetExecutableInfo(relativePath);
                    var config = EslintHelper.GetConfigInfo(relativePath);
                    var ignore = EslintHelper.GetIgnoreInfo(relativePath);

                    var arguments = GetArguments(filePath, config?.FullName, ignore?.FullName);

                    var output = await RunAsync(config?.DirectoryName, executable.FullName, arguments, token)
                        .ConfigureAwait(false);

                    token.ThrowIfCancellationRequested();

                    if (string.IsNullOrEmpty(output))
                        throw new Exception("linter returned empty result~ please read output for detailed information ^");

                    IEnumerable<EslintResult> results = new List<EslintResult>();

                    try
                    {
                        results = JsonConvert.DeserializeObject<IEnumerable<EslintResult>>(output);
                    }
                    catch (Exception e)
                    {
                        OutputWindowHelper.WriteLine(
                            "exception: error trying to deserialize output:" +
                            Environment.NewLine +
                            output);

                        OutputWindowHelper.WriteLine(e.Message);
                    }

                    var messages = ProcessResults(results);

                    token.ThrowIfCancellationRequested();

                    provider.Accept(filePath, messages);
                }
                catch (OperationCanceledException)
                { }
                catch (Exception e)
                {
                    OutputWindowHelper.WriteLine(e.Message);
                }
                finally
                {
                    _mutex.Release();
                }
            }
            catch (OperationCanceledException)
            { }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }
        }

        private static IEnumerable<EslintMessage> ProcessMessages(IReadOnlyList<EslintMessage> messages)
        {
            // return empty messages when warning about ignored files
            if (1 == messages.Count && RegexHelper.IgnoreFileMatch(messages[0].Message))
                return Enumerable.Empty<EslintMessage>();

            var fatal = messages.SingleOrDefault(message => message.IsFatal);
            if (null != fatal)
                throw new Exception(fatal.Message);

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

        private static string GetArguments(string filePath, string configPath, string ignorePath)
        {
            var arguments = new Dictionary<string, string> { { "format", "json" } };

            if (null != configPath)
                arguments.Add("config", configPath);

            if (null != ignorePath)
                arguments.Add("ignore-path", ignorePath);

            return string.Join(" ", QuoteArgument(filePath), FormatArguments(arguments));
        }

        private static string FormatArguments(IReadOnlyDictionary<string, string> arguments)
        {
            return string.Join(" ", arguments.Select(argument => $"--{argument.Key}=\"{argument.Value}\""));
        }

        private static string QuoteArgument(string argument)
        {
            return $"\"{argument}\"";
        }

        private static Task<string> RunAsync(string cwd, string fileName, string arguments, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var startInfo = new ProcessStartInfo(fileName, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8
                };

                if (false == string.IsNullOrEmpty(cwd))
                    startInfo.WorkingDirectory = cwd;

                var process = new Process { StartInfo = startInfo };

                string error = null;
                string output = null;

                process.ErrorDataReceived += ErrorHandler;
                process.OutputDataReceived += OutputHandler;

                void ErrorHandler(object sender, DataReceivedEventArgs e)
                {
                    if (null != e.Data)
                        error += e.Data + Environment.NewLine;
                }

                void OutputHandler(object sender, DataReceivedEventArgs e)
                {
                    if (null != e.Data)
                        output += e.Data + Environment.NewLine;
                }

                try
                {
                    //token.ThrowIfCancellationRequested();

                    if (false == process.Start())
                        throw new Exception("exception: unable to start eslint process");

                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();

                    process.WaitForExit();

                    if (false == string.IsNullOrEmpty(error))
                        throw new Exception(error);
                }
                catch (OperationCanceledException)
                { }
                catch (Exception e)
                {
                    OutputWindowHelper.WriteLine(e.Message);
                }
                finally
                {
                    process.Close();
                }

                return output;
            }, token);
        }
    }
}
