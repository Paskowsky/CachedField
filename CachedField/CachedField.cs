using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CachedField
{
    public class CachedField<T>
    {
        public static readonly TimeSpan DefaultUpdateSpan = new TimeSpan(0, 5, 0);

        private T value;
        private Func<T> getValueDelegate;
        private ReaderWriterLockSlim rwLock;
        private DateTime lastUpdate;

        private readonly TimeSpan updateSpan;


        private bool InvalidateRequired()
        {
            bool result = false;
            rwLock.EnterReadLock();
            if (DateTime.Now.Subtract(lastUpdate) > updateSpan)
            {
                result = true;
            }
            rwLock.ExitReadLock();
            return result;
        }

        public CachedField(Func<T> getValueDelegate) : this(getValueDelegate, DefaultUpdateSpan)
        {

        }

        public CachedField(Func<T> getValueDelegate, TimeSpan updateSpan)
        {
            if (getValueDelegate == null)
                throw new ArgumentNullException("getValueDelegate");

            this.rwLock = new ReaderWriterLockSlim();

            this.updateSpan = updateSpan;
            this.getValueDelegate = getValueDelegate;

            this.Invalidate();
        }

        public T Value
        {
            get
            {
                if (InvalidateRequired())
                {
                    Invalidate();
                }

                //Enter read lock to prevent value writing
                rwLock.EnterReadLock();
                T temp = value;
                rwLock.ExitReadLock();

                return temp;
            }
        }
        
        public void Invalidate()
        {
            if (!rwLock.TryEnterWriteLock(1))
            {
                //If it's already updating cache, don't update again.
                return;
            }

            value = getValueDelegate();
            lastUpdate = DateTime.Now;
            rwLock.ExitWriteLock();
        }
    }
}
