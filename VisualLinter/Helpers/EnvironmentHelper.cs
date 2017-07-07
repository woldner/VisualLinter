using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class EnvironmentHelper
    {
        internal static string GetVariable(string name) => GetVariableInfo("PATH", EnvironmentVariableTarget.User)
            .Select(value => new { value, files = GetFiles(value) })
            .Where(info => null != info.files)
            .Where(info => info.files.Any(file => file.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1))
            .Select(info => GetExecutable(Path.Combine(Environment.ExpandEnvironmentVariables(info.value), name)))
            .FirstOrDefault();

        private static string GetExecutable(string name) => GetVariableInfo("PATHEXT", EnvironmentVariableTarget.Machine)
            .Where(value => null != value)
            .Select(value => new { value, file = name + value })
            .Where(info => File.Exists(info.file))
            .Select(info => info.file)
            .FirstOrDefault();

        private static string[] GetFiles(string value)
        {
            try
            {
                return Directory.GetFiles(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static IEnumerable<string> GetVariableInfo(string variable, EnvironmentVariableTarget target)
        {
            try
            {
                return Environment.GetEnvironmentVariable(variable, target)?
                    .Split(Path.PathSeparator);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}