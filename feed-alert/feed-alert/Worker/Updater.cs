using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace feed_alert.Worker
{
    class Updater
    {
        public static void StartUpdateLoop()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    
                }
            });
        }
    }
}
