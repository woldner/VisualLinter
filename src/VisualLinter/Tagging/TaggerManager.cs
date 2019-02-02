using System;
using System.Collections.Generic;

namespace jwldnr.VisualLinter.Tagging
{
    internal class TaggerManager
    {
        private readonly IDictionary<string, LinterTagger> _taggers = new Dictionary<string, LinterTagger>(StringComparer.OrdinalIgnoreCase);

        internal IEnumerable<LinterTagger> Values => _taggers.Values;

        internal void Add(LinterTagger tagger)
        {
            _taggers.Add(tagger.FilePath, tagger);
        }

        internal bool Exists(string filePath)
        {
            return _taggers.ContainsKey(filePath);
        }

        internal void Remove(LinterTagger tagger)
        {
            _taggers.Remove(tagger.FilePath);
        }

        internal void Rename(string oldPath, string newPath)
        {
            if (false == _taggers.TryGetValue(oldPath, out var tagger))
                return;

            _taggers.Add(newPath, tagger);
            _taggers.Remove(oldPath);
        }

        internal bool TryGetValue(string filePath, out LinterTagger tagger)
        {
            return _taggers.TryGetValue(filePath, out tagger);
        }
    }
}
