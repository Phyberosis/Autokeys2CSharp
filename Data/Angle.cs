using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Angle
    {
        public static Angle PI = new Angle(_PI);

        private float val;
        private const float _PI = (float)Math.PI;

        private void init(float j)
        {
            while (j < 0)
            {
                j += 2 * _PI;
            }

            while (j >= 2 * _PI)
            {
                j -= 2 * _PI;
            }

            val = j;
        }

        public static Angle Force(float j)
        {
            Angle a = new Angle();
            a.val = j;
            return a;
        }

        public Angle()
        {
            val = 0f;
        }

        public Angle(float j)
        {
            init(j);
            rectify();
        }

        public bool Intercepts(Angle i, Angle dest)
        {
            if (dest == i || dest == this || i == this) return false;

            Angle diff = dest - this;
            Angle j = i - this;

            //Console.WriteLine("ANG_INT: " + diff + " " + j);
            float toHit = (float)diff - (float)j;
            bool hits = diff > 0 ? toHit > 0 : toHit < 0;
            //Console.WriteLine("TH>> " + toHit);
            return !(diff > 0 ^ j > 0) && hits;
        }

        private void rectify()
        {
            if (val > _PI) val = -((2 * _PI) - val);
            else if (val < -_PI) val = ((2 * _PI) + val);
        }

        public float toDegrees()
        {
            return ((val * 180f) / (float)Math.PI);
        }

        public override string ToString()
        {
            return val.ToString();
        }

        public static bool TryParse(string v, out Angle val)
        {
            float f;
            if (float.TryParse(v, out f))
            {
                val = new Angle(f);
                return true;
            }
            else
            {
                val = null;
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(float)))
                return val == (float)obj;

            if (!obj.GetType().Equals(GetType()))
                return false;

            return val == ((Angle)obj).val;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static explicit operator float(Angle j) => j.val;
        public static implicit operator Angle(float j)
        {
            return new Angle(j);
        }
        public static implicit operator Angle(double j)
        {
            return new Angle((float)j);
        }
        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static bool operator ==(Angle i, Angle j)
        {
            return i.val == j.val;
        }

        public static bool operator !=(Angle i, Angle j)
        {
            return i.val != j.val;
        }

        public static bool operator >(Angle i, Angle j)
        {
            return i.val > j.val;
        }

        public static bool operator <(Angle i, Angle j)
        {
            return i.val < j.val;
        }

        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static Angle operator +(Angle i, Angle j)
        {
            return new Angle(i.val + j.val);
        }

        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static Angle operator -(Angle i, Angle j)
        {
            return new Angle(i.val - j.val);
        }

        public static Angle operator -(Angle j)
        {
            return new Angle(-j.val);
        }

        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static Angle operator *(Angle i, float j)
        {
            //Console.WriteLine("{0}, {1}", i, j);
            return new Angle(i.val * j);
        }

        /// <param name = "j"> should be the same type as <paramref name="i"/>
        public static Angle operator /(Angle i, float j)
        {
            return new Angle(i.val / j);
        }
    }
}
