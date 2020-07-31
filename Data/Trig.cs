using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Trig
    {
        // project raw onto target
        public static Vector3 Project(Vector3 raw, Vector3 target)
        {
            Vector3 dir = Vector3.Normalize(target);
            double l = Vector3.Dot(raw, dir);
            Vector3 result = Vector3.Multiply(dir, (float)l);

            return result;
        }

        // squash raw onto plain
        public static Vector3 Squash(Vector3 raw, Vector3 normal)
        {
            Vector3 difference = Project(raw, normal);
            Vector3 result = Vector3.Subtract(raw, difference);

            return result;
        }

        // get angle between a and b
        public static Angle AngleBetween(Vector3 a, Vector3 b)
        {
            double top = Vector3.Dot(a, b);
            double bot = a.Length() * b.Length();
            double raw = Math.Acos(top / bot);

            return (Angle)raw;
        }

        public static Angle AngleBetween(Quaternion a, Quaternion b)
        {
            double top = Quaternion.Dot(a, b);
            double bot = a.Length() * b.Length();
            double raw = Math.Acos(top / bot);

            return (Angle)raw;
        }

        // todo - point == vec?
        //public static Angle AngleBetween(Vector2 a, Vector2 b)
        //{
        //    Angle ans = Math.Atan((b.Y - a.Y) / (b.X - a.X));
        //    if (b.X < a.X) ans += Math.PI;
        //    return Math.Abs((double)ans);
        //}

        public static Angle AngleBetween(Point a, Point b)
        {
            if (b.X - a.X == 0)
                return b.Y > a.Y ? Angle.PI/2 : -Angle.PI/2;

            Angle ans = Math.Atan((b.Y - a.Y) / (b.X - a.X));
            if (b.X < a.X) ans += Angle.PI;
            return ans;
        }
    }
}
