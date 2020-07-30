using System;
using System.Collections.Generic;
using System.Text;

namespace MouseSimulator
{
    public struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point Zero = new Point(0, 0);
    }
}
