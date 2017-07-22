using System.Collections.Generic;

namespace jwldnr.VisualLinter.Tagging
{
    internal class TaggerManager
    {
        internal IEnumerable<Tagger> Values => _taggers.Values;

        private readonly IDictionary<string, Tagger> _taggers = new Dictionary<string, Tagger>();

        internal void Add(Tagger tagger)
        {
            _taggers.Add(Key(tagger.FilePath), tagger);
        }

        internal bool Exists(string filePath)
        {
            return _taggers.ContainsKey(Key(filePath));
        }

        internal void Remove(Tagger tagger)
        {
            _taggers.Remove(Key(tagger.FilePath));
        }

        internal void Rename(string oldPath, string newPath)
        {
            var oldKey = Key(oldPath);
            if (!_taggers.TryGetValue(oldKey, out Tagger tagger))
                return;

            _taggers.Add(Key(newPath), tagger);
            _taggers.Remove(oldKey);
        }

        internal bool TryGetValue(string filePath, out Tagger tagger)
        {
            return _taggers.TryGetValue(Key(filePath), out tagger);
        }

        private static string Key(string filePath)
        {
            return filePath.ToLowerInvariant();
        }
    }
}