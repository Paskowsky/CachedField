using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CachedField.Tests
{
    class Program
    {
        static CachedField<string> x;

        static void Main(string[] args)
        {
            x = new CachedField<string>(() => { Thread.Sleep(5000); return DateTime.Now.ToString(); }, new TimeSpan(0, 0, 30));
            x.OnInvalidated += X_OnInvalidated;
            new Thread(() => { while (true) { Thread.Sleep(10000); x.Invalidate(); } }).Start();
            new Thread(() => { while (true) { Thread.Sleep(20000); x.Invalidate(); } }).Start();

            while (true)
            {
                Console.WriteLine(x.Value);

                Thread.Sleep(2 * 1000);

            }
        }

        private static void X_OnInvalidated(object sender, CachedFieldInvalidatedEventArgs e)
        {
            Console.WriteLine("Invalid" + e.LastUpdate.ToString());
        }
    }
}
