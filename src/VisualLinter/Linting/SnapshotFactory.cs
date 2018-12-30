using Microsoft.VisualStudio.Shell.TableManager;

namespace jwldnr.VisualLinter.Linting
{
    internal class SnapshotFactory : TableEntriesSnapshotFactoryBase
    {
        internal SnapshotFactory(LinterSnapshot snapshot)
        {
            CurrentSnapshot = snapshot;
        }

        public override int CurrentVersionNumber => CurrentSnapshot.VersionNumber;

        internal LinterSnapshot CurrentSnapshot { get; private set; }

        internal void UpdateSnapshot(LinterSnapshot snapshot)
        {
            CurrentSnapshot = snapshot;
        }

        public override ITableEntriesSnapshot GetCurrentSnapshot()
        {
            return CurrentSnapshot;
        }

        public override ITableEntriesSnapshot GetSnapshot(int versionNumber)
        {
            var snapshot = CurrentSnapshot;

            return versionNumber == snapshot.VersionNumber
                ? snapshot
                : null;
        }
    }
}
