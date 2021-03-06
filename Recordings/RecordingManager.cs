﻿using System;
using Data;
using InputHook;
using OutputSimulator;
using Events;
using System.Xml;
using System.Windows.Input;
using Monitors;

namespace Recordings
{
    public class RecordingManager
    {
        private Hook hook = Hook.I();
        private delegate void RecKey(bool down, Key k);
        private RecKey recKey = null;
        private Hook.OnMouseDelegate recMouse = null;

        private long startT;
        private Recording rec;
        private EventHandle<Recording> newRecordingHandle;
        private Robot robot = new Robot();

        private EventHandle<MyColors> showOverlayHandle;
        private EventHandle<object> hideOverlayHandle;

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

            rec = new Recording();

            newRecordingHandle = EventsBuiltin.RegisterEvent<Recording>(EventID.REC);
            newRecordingHandle.Notify(rec);

            showOverlayHandle = EventsBuiltin.RegisterEvent<MyColors>(EventID.BORDER_SHOW);
            hideOverlayHandle = EventsBuiltin.RegisterEvent(EventID.BORDER_HIDE);
            ExternalKeyMonitor.Monitor(Key.Escape, () => { rec?.Stop(); });

            EventsBuiltin.RegisterListener<Recording>(EventID.REC_LOADED, (r) =>
            {
                rec = r;
                //Console.WriteLine("here");
            });
        }

        public void Play(float speed, int repeats)
        {
            if (currState.Get() != State.IDLE) return;
            rec?.Play(robot, () => 
            {
                currState.Set(State.IDLE);
                hideOverlayHandle.Notify();

                Console.WriteLine("END");
            }, repeats, speed);
            currState.Set(State.PLAYING);
            showOverlayHandle.Notify(MyColors.GREEN);

            Console.WriteLine("Play");
            //Console.WriteLine(rec.Keyframes.Count);
            //Console.WriteLine(rec.Keyframes.First.Value.ToString());
        }

        public void RecordBegin()
        {
            if (currState.Get() != State.IDLE) return;
            currState.Set(State.RECORDING);
            showOverlayHandle.Notify(MyColors.RED);

            rec = new Recording();
            startT = Time.Millis();

            Func<long> GetTimestamp = () => { return Time.Millis() - startT; };

            recKey = (d, k) =>
            {

                KeyActions ka;
                switch (k)
                {
                    case Key.LeftAlt:
                    case Key.RightAlt:
                    case Key.LeftShift:
                    case Key.RightShift:
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                    case Key.LWin:
                    case Key.RWin:
                        ka = d ? KeyActions.DOWN : KeyActions.UP;
                        break;
                    default:
                        if (!d) return;
                        ka = KeyActions.PRESS;
                        break;
                }

                rec.AddKeyframe(ka, k, GetTimestamp());
            };

            //bool ld = false, rd = false, md = false;
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
                    if (isLastMove) rec.AddKeyframe(MouseAction.WM_MOUSEMOVE, px, py, GetTimestamp() - 1);
                    isLastMove = false;
                }

                rec.AddKeyframe(a, x, y, GetTimestamp());
            };
        }

        public void RecordEnd()
        {
            if (currState.Get() != State.RECORDING) return;
            currState.Set(State.IDLE);
            hideOverlayHandle.Notify();

            recKey = null;
            recMouse = null;

            if(rec.Keyframes.Count > 0)
            {
                var last = rec.Keyframes.Last;
                var sLast = last.Previous;

                Console.WriteLine(last.Value.ToString());
                Console.WriteLine(sLast.Value.ToString());
                if (sLast.Value.ToString().Contains("Shift") &&
                    last.Value.ToString().Contains("Escape"))
                {
                    rec.Keyframes.Remove(last);
                    rec.Keyframes.Remove(sLast);
                };
            }

            newRecordingHandle.Notify(rec);
            //Console.WriteLine(rec.ToString());
        }

        private void onKeyDown(Key k)
        {
            recKey?.Invoke(true, k);
        }

        private void onKeyUp(Key k)
        {
            recKey?.Invoke(false, k);
        }

        private void onMouse(MouseAction a, int x, int y)
        {
            recMouse?.Invoke(a, x, y);
        }
    }
}
