using jwldnr.VisualLinter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter.Services
{
    internal interface ILinterService
    {
        Task<IEnumerable<LinterMessage>> LintAsync(string filePath);
    }
}