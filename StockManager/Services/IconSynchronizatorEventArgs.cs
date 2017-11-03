using System;

namespace StockManager.Services
{
    class IconSynchronizatorEventArgs : EventArgs
    {
        private SyncState state;

        public SyncState State
        {
            get
            {
                return state;
            }
        }

        public IconSynchronizatorEventArgs(SyncState state)
        {
            this.state = state;
        }
    }
}
