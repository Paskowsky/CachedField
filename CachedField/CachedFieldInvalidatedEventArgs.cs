using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CachedField
{
    public class CachedFieldInvalidatedEventArgs : EventArgs
    {
        public DateTime LastUpdate { get; private set; }

        public CachedFieldInvalidatedEventArgs(DateTime lastUpdate)
        {
            this.LastUpdate = lastUpdate;
        }
    }
}
