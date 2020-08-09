using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Time
    {
        private static readonly long FIRST = DateTime.Now.Ticks;

        public static float Now()
        {
            return (DateTime.Now.Ticks - FIRST) / 10000000f;
        }

        public static long Micro()
        {
            return (DateTime.Now.Ticks - FIRST) / 10L;
        }
        public static long Millis()
        {
            return (DateTime.Now.Ticks - FIRST) / 10000L;
        }
    }
}
