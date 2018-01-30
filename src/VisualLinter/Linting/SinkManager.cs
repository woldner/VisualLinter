﻿using jwldnr.VisualLinter.Tagging;
using Microsoft.VisualStudio.Shell.TableManager;
using System;

namespace jwldnr.VisualLinter.Linting
{
    internal class SinkManager : IDisposable
    {
        private readonly TaggerProvider _provider;
        private readonly ITableDataSink _sink;

        internal SinkManager(TaggerProvider provider, ITableDataSink sink)
        {
            _provider = provider;
            _sink = sink;

            _provider.AddSinkManager(this);
        }

        public void Dispose()
        {
            _provider.RemoveSinkManager(this);
        }

        internal void AddFactory(ITableEntriesSnapshotFactory factory)
        {
            _sink.AddFactory(factory);
        }

        internal void RemoveFactory(ITableEntriesSnapshotFactory factory)
        {
            _sink.RemoveFactory(factory);
        }

        internal void UpdateSink(ITableEntriesSnapshotFactory factory)
        {
            _sink.FactorySnapshotChanged(factory);
        }
    }
}
