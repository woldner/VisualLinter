using System.Collections.Generic;

namespace jwldnr.VisualLinter.Tagging
{
    internal class TaggerManager
    {
        internal IEnumerable<LinterTagger> Values => _taggers.Values;

        private readonly IDictionary<string, LinterTagger> _taggers = new Dictionary<string, LinterTagger>();

        internal void Add(LinterTagger tagger)
        {
            _taggers.Add(Key(tagger.FilePath), tagger);
        }

        internal bool Exists(string filePath)
        {
            return _taggers.ContainsKey(Key(filePath));
        }

        internal void Remove(LinterTagger tagger)
        {
            _taggers.Remove(Key(tagger.FilePath));
        }

        internal void Rename(string oldPath, string newPath)
        {
            var oldKey = Key(oldPath);
            if (!_taggers.TryGetValue(oldKey, out LinterTagger tagger))
                return;

            _taggers.Add(Key(newPath), tagger);
            _taggers.Remove(oldKey);
        }

        internal bool TryGetValue(string filePath, out LinterTagger tagger)
        {
            return _taggers.TryGetValue(Key(filePath), out tagger);
        }

        private static string Key(string filePath)
        {
            return filePath.ToLowerInvariant();
        }
    }
}