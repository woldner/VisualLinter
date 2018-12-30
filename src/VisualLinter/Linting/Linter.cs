using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Tagging;
using Newtonsoft.Json;

namespace jwldnr.VisualLinter.Linting
{
    public interface ILinter
    {
        Task LintAsync(string filePath, ILinterTracker tracker, CancellationToken token);
    }

    [Export(typeof(ILinter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class Linter : ILinter
    {
        private readonly IEslintHelper _eslintHelper;
        private readonly ILogger _logger;

        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        [ImportingConstructor]
        internal Linter(
            [Import] ILogger logger,
            [Import] IEslintHelper eslintHelper)
        {
            _logger = logger; // todo move to outer scope??
            _eslintHelper = eslintHelper;
        }

        public async Task LintAsync(string filePath, ILinterTracker tracker, CancellationToken token)
        {
            try
            {
                await _mutex.WaitAsync(token).ConfigureAwait(false);

                try
                {
                    token.ThrowIfCancellationRequested();

                    var directoryPath = Path.GetDirectoryName(filePath) ??
                        throw new Exception($"exception: could not get directory for file {filePath}");

                    var eslintPath = _eslintHelper.GetPath(directoryPath);
                    var arguments = string.Join(" ", QuoteArgument(filePath),
                        _eslintHelper.GetArguments(directoryPath));

                    var output = await RunAsync(eslintPath, arguments, token)
                        .ConfigureAwait(false);

                    token.ThrowIfCancellationRequested();

                    if (string.IsNullOrEmpty(output))
                        throw new Exception("exception: linter returned empty result");

                    IEnumerable<EslintResult> results = new List<EslintResult>();

                    try
                    {
                        results = JsonConvert.DeserializeObject<IEnumerable<EslintResult>>(output);
                    }
                    catch (Exception e)
                    {
                        _logger.WriteLine(
                            "exception: error trying to deserialize output:" +
                            Environment.NewLine +
                            output);

                        _logger.WriteLine(e.Message);
                    }

                    var messages = ProcessResults(results);

                    token.ThrowIfCancellationRequested();

                    tracker.Accept(filePath, messages);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    _logger.WriteLine(e.Message);
                }
                finally
                {
                    _mutex.Release();
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                _logger.WriteLine(e.Message);
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

        private static string QuoteArgument(string argument)
        {
            return $"\"{argument}\"";
        }

        private Task<string> RunAsync(string eslintPath, string arguments, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var startInfo = new ProcessStartInfo(eslintPath, arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    StandardErrorEncoding = Encoding.UTF8,
                    StandardOutputEncoding = Encoding.UTF8
                };

                var process = new Process {StartInfo = startInfo};

                string error = null;
                string output = null;

                process.ErrorDataReceived += ErrorHandler;
                process.OutputDataReceived += OutputHandler;

                void ErrorHandler(object sender, DataReceivedEventArgs e)
                {
                    if (null != e.Data)
                        error += e.Data;
                }

                void OutputHandler(object sender, DataReceivedEventArgs e)
                {
                    if (null != e.Data)
                        output += e.Data;
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
                {
                }
                catch (Exception e)
                {
                    _logger.WriteLine(e.Message);
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
