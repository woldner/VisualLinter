using Microsoft.VisualStudio.Shell.TableManager;

namespace jwldnr.VisualLinter.Linting
{
    internal class SnapshotFactory : TableEntriesSnapshotFactoryBase
    {
        public override int CurrentVersionNumber => CurrentSnapshot.VersionNumber;

        internal LintSnapshot CurrentSnapshot { get; private set; }

        internal SnapshotFactory(LintSnapshot snapshot)
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

        internal void UpdateResults(LintSnapshot snapshot)
        {
            CurrentSnapshot.NextSnapshot = snapshot;
            CurrentSnapshot = snapshot;
        }
    }
}