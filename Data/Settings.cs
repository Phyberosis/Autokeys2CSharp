using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Data
{
    public class Settings
    {
        private const float MAX_SPEED = 20;
        private const float MIN_SPEED = 0.25f;
        private const int MAX_REPEATS = 999;
        private const int MIN_REPEATS = 0;

        private int repeats;
        public int Repeats
        {
            get
            {
                return repeats;
            }
            set
            {
                var r = value;
                if (r > MAX_REPEATS) r = MAX_REPEATS;
                if (r < MIN_REPEATS) r = MIN_REPEATS;
                repeats = r;
            }
        }

        private float speed;
        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                var s = value;
                if (s < MIN_SPEED) s = MIN_SPEED;
                if (s > MAX_SPEED) s = MAX_SPEED;
                speed = s;
            }
        }

        public Settings()
        {
            Repeats = 0;
            Speed = 1;
        }

        public Settings(StreamReader sr)
        {
            string line = sr.ReadLine();
            speed = float.Parse(line);
            line = sr.ReadLine();
            repeats = int.Parse(line);
        }

        public override string ToString()
        {
            return speed + "\n" + repeats;
        }
    }
}
