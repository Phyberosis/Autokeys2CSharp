using System;
using System.Windows.Forms;
using Data;
using InputHook;
using OutputSimulator;
using Events;
using System.Xml;

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
        private EventHandle<Recording> recoringStopHandle;
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

            recoringStopHandle = EventsBuiltin.RegisterEvent<Recording>(EventID.REC);
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

            Func<long> GetTimestamp = () => { return Time.Millis() - startT; };

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

                KeyActions ka;
                switch (k)
                {
                    case Keys.LMenu:
                    case Keys.RMenu:
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                    case Keys.LControlKey:
                    case Keys.RControlKey:
                        ka = d ? KeyActions.DOWN : KeyActions.UP;
                        break;
                    default:
                        if (!d) return;
                        ka = KeyActions.PRESS;
                        break;
                }

                rec.AddKeyFrame(new Recording.KeyFrameK(ka, k, GetTimestamp()));
            };

            bool ld = false, rd = false, md = false;
            int px = -1, py = -1;
            //Recording.KeyFrameM prevK;
            bool isLastMove = false;
            recMouse = (a, x, y) =>
            {
                //if (a == MouseAction.WM_LBUTTONDOWN) ld = true;
                //if (a == MouseAction.WM_RBUTTONDOWN) rd = true;
                //if (a == MouseAction.WM_MBUTTONDOWN) md = true;

                //if (a == MouseAction.WM_LBUTTONUP) ld = false;
                //if (a == MouseAction.WM_RBUTTONUP) rd = false;
                //if (a == MouseAction.WM_MBUTTONUP) md = false;

                if (a == MouseAction.WM_MOUSEMOVE /*&& !ld && !rd && !md*/)
                {
                    px = x; py = y;
                    isLastMove = true;
                    return;
                }
                else
                {
                    if (isLastMove) rec.AddKeyFrame(new Recording.KeyFrameM(MouseAction.WM_MOUSEMOVE, px, py, GetTimestamp() - 1));
                    isLastMove = false;
                }

                rec.AddKeyFrame(new Recording.KeyFrameM(a, x, y, GetTimestamp()));
            };
        }

        public void RecordEnd()
        {
            if (currState.Get() != State.RECORDING) return;
            currState.Set(State.IDLE);

            recKey = null;
            recMouse = null;

            recoringStopHandle.Notify(rec);
            //Console.WriteLine(rec.ToString());
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
