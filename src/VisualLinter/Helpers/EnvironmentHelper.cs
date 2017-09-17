using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class EnvironmentHelper
    {
        internal static string GetUserDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        internal static string GetVariable(string name, EnvironmentVariableTarget target)
        {
            var variables = GetVariableInfo("PATH", target);

            return variables
                .Select(value => new { value, files = GetFiles(value) })
                .Where(info => null != info.files)
                .Where(info => info.files.Any(file => -1 != file.IndexOf(name, StringComparison.OrdinalIgnoreCase)))
                .Select(info => GetExecutable(Path.Combine(Environment.ExpandEnvironmentVariables(info.value), name)))
                .FirstOrDefault();
        }

        private static string GetExecutable(string name)
        {
            var variables = GetVariableInfo("PATHEXT", EnvironmentVariableTarget.Machine);

            return variables
                .Where(value => null != value)
                .Select(value => new { value, file = name + value })
                .Where(info => File.Exists(info.file))
                .Select(info => info.file)
                .FirstOrDefault();
        }

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
                var values = Environment.GetEnvironmentVariable(variable, target);
                return values?.Split(Path.PathSeparator);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}