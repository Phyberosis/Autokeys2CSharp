using System;
using System.Windows.Forms;
using Data;
using InputHook;
using OutputSimulator;

namespace Recordings
{
    public class RecordingManager
    {
        private Hook hook = Hook.I();
        private delegate void RecKey(bool down, Keys k);
        private RecKey recKey = null;
        private Hook.OnMouseDelegate recMouse = null;

        private long startT;
        private Recording rec;
        private Robot robot = new Robot();

        private enum State { IDLE, RECORDING, PLAYING }
        private class SynchronizedEnum<T>
        {
            private T value;

            public void Set(T s)
            {
                lock (this)
                {
                    value = s;
                }
            }

            public T Get()
            {
                T img;
                lock (this)
                {
                    img = value;
                }
                return img;
            }
        }

        private SynchronizedEnum<State> currState = new SynchronizedEnum<State>();

        public RecordingManager()
        {
            hook.AddKeyHook(onKeyDown, onKeyUp);
            hook.AddMouseHook(onMouse);
            currState.Set(State.IDLE);
        }

        public void Play()
        {
            if (currState.Get() != State.IDLE) return;
            rec?.Play(robot, () => 
            {
                currState.Set(State.IDLE);
                Console.WriteLine("END");
            });
            currState.Set(State.PLAYING);

            Console.WriteLine("Play");
        }

        public void RecordBegin()
        {
            if (currState.Get() != State.IDLE) return;
            currState.Set(State.RECORDING);

            rec = new Recording();
            startT = Time.Millis();

            Func<int> GetTimestamp = () => { return (int)(Time.Millis() - startT); };

            bool diverted = false; // for not recording the stop rec gesture + esc

            recKey = (d, k) =>
            {
                k = k == Keys.None ? Keys.Escape : k;
                if (d)
                {
                    if (diverted) rec.Flush();
                    if (k == Keys.Escape)
                    {
                        rec.Divert();
                        diverted = true;
                    }
                }
                rec.AddKeyFrame(new Recording.KeyFrameK(d, k, GetTimestamp()));
            };

            recMouse = (a, x, y) =>
            {
                rec.AddKeyFrame(new Recording.KeyFrameM(a, x, y, GetTimestamp()));
            };
        }

        public void RecordEnd()
        {
            if (currState.Get() != State.RECORDING) return;
            currState.Set(State.IDLE);

            recKey = null;
            recMouse = null;

            Console.WriteLine(rec.ToString());
        }

        private void onKeyDown(Keys k)
        {
            recKey?.Invoke(true, k);
        }

        private void onKeyUp(Keys k)
        {
            recKey?.Invoke(false, k);
        }

        private void onMouse(MouseAction a, int x, int y)
        {
            recMouse?.Invoke(a, x, y);
        }
    }
}
