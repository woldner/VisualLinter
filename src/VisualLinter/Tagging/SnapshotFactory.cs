using Microsoft.VisualStudio.Shell.TableManager;

namespace jwldnr.VisualLinter.Tagging
{
    internal class SnapshotFactory : TableEntriesSnapshotFactoryBase
    {
        internal SnapshotFactory(MessagesSnapshot snapshot)
        {
            CurrentSnapshot = snapshot;
        }

        public override int CurrentVersionNumber => CurrentSnapshot.VersionNumber;

        internal MessagesSnapshot CurrentSnapshot { get; private set; }

        internal void UpdateSnapshot(MessagesSnapshot snapshot)
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
