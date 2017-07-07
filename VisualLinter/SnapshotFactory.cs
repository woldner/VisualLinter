using Microsoft.VisualStudio.Shell.TableManager;

namespace jwldnr.VisualLinter
{
    internal class SnapshotFactory : TableEntriesSnapshotFactoryBase
    {
        public override int CurrentVersionNumber => CurrentSnapshot.VersionNumber;

        internal LinterSnapshot CurrentSnapshot { get; private set; }

        internal SnapshotFactory(LinterSnapshot snapshot)
        {
            CurrentSnapshot = snapshot;
        }

        public override ITableEntriesSnapshot GetCurrentSnapshot() => CurrentSnapshot;

        public override ITableEntriesSnapshot GetSnapshot(int versionNumber)
        {
            var snapshot = CurrentSnapshot;
            return versionNumber == snapshot.VersionNumber
                ? snapshot
                : null;
        }

        internal void UpdateResults(LinterSnapshot snapshot)
        {
            CurrentSnapshot.NextSnapshot = snapshot;
            CurrentSnapshot = snapshot;
        }
    }
}