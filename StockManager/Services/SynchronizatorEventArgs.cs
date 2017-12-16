using System;

namespace StockManager.Services
{
    class SynchronizatorEventArgs : EventArgs
    {
        private SyncState state;

        public SyncState State
        {
            get
            {
                return state;
            }
        }

        public SynchronizatorEventArgs(SyncState state)
        {
            this.state = state;
        }
    }
}
