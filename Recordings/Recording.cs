using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Xml;
using Data;
using InputHook;
using Monitors;
using OutputSimulator;

namespace Recordings
{
    public class Recording
    {
        public enum KFType
        {
            MOUSE, KEY
        }


        public abstract class Keyframe
        {
            private long t;
            public delegate void DoActionDelegate(Robot r);
            public DoActionDelegate DoAction;
            internal Recording Parent;

            public Action<long> updateTime;

            public readonly KFType Type;

            public Keyframe(long time, Recording parent, KFType type)
            {
                //Container = container;
                Parent = parent;
                t = time;

                Type = type;
            }


            public long GetTime() { return t; }

            //public void SlideTime(long delta)
            //{
            //    t += delta;
            //    updateTime(getTimeSec());
            //}

            public void SetTime(float time)
            {
                long p = t;
                if (time < 0) time = 0;
                t = (long)((time * 1000f) + 0.5f);
                Parent.checkTime(t, this);
                updateTime(t);

                Parent.shiftTimes(t - p, this);
            }

            internal void setTimeInternal(long time)
            {
                t = time;
                updateTime(t);
            }

            internal void shiftTimeInternal(long delta)
            {
                t += delta;
                updateTime(t);
            }

            public void OnUpdateTime(Action<long> callback) { updateTime = callback; }

            //public abstract string GetInfo();
            //public abstract string GetDescription();

            public abstract void Step(float x, Robot r);

            #region need fixing

            //public virtual bool HandlesKeys() { return false; }
            //public virtual KeyActions GetKeyAction() { return KeyActions.NONE; }
            //public virtual Key GetKey() { return Key.None; }

            //public virtual MouseAction GetMouseAction() { return MouseAction.NONE; }
            //public virtual int[] GetLocation() { return new int[] { -1, -1 }; }

            #endregion
        }

        public class KeyframeK : Keyframe
        {
            private KeyActions ka;
            private Key k;

            private Action<KeyActions> updateKA;
            private Action<Key> updateKey;

            internal KeyframeK(KeyActions ka, Key k, long timestamp, Recording parent) 
                : base(timestamp, parent, KFType.KEY)
            {
                this.k = k;
                this.ka = ka;
                DoAction = (r) => { r.DoAction(ka, k); };
                //Info = k.ToString();
                //Description = isDown ? "DOWN" : "UP";
            }

            public void OnUpdateKA(Action<KeyActions> cb) { updateKA = cb; }
            public void OnUpdateKey(Action<Key> cb) { updateKey = cb; }

            public void CycleKeyActions()
            {
                ka++;
                if ((int)ka >= Enum.GetNames(typeof(KeyActions)).Length) ka = (KeyActions)1;
                updateKA(ka);
            }

            public void SetKey(Key k)
            {
                this.k = k;
                updateKey(k);
            }

            public KeyActions GetKA()
            {
                return ka;
            }

            public Key GetKey()
            {
                return k;
            }


            public override string ToString()
            {
                return new StringBuilder()
                    .Append(k.ToString())
                    .Append(" ")
                    .Append("|")
                    .Append("KEY ")
                    .Append(ka.ToString())
                    .Append(GetTime())
                    .ToString();
            }

            public override void Step(float x, Robot r)
            {}
        }

        public class KeyframeM : Keyframe
        {
            private MouseAction ma;
            private int x, y;

            private Action<int, int> updateLoc;
            private Action<MouseAction> updateMA;

            private MouseAction[] actions;
            private int currentAction;

            internal KeyframeM(MouseAction a, int x, int y, long timestamp, Recording parent)
                : base(timestamp, parent, KFType.MOUSE)
            {
                this.x = x;
                this.y = y;
                this.ma = a;

                DoAction = (r) => r.DoAction(ma, x, y);

                actions = (MouseAction[])Enum.GetValues(typeof(MouseAction));
                for (int i = 0; i < actions.Length; i++)
                {
                    if (actions[i] == ma)
                    {
                        currentAction = i;
                        break;
                    }
                }
            }

            public void OnUpdateLoc(Action<int, int> cb) { updateLoc = cb; }
            public void OnUpdateMA(Action<MouseAction> cb) { updateMA = cb; }

            public void CycleMouseActions(bool forward)
            {
                currentAction = forward ? currentAction + 1 : currentAction - 1;
                int l = Enum.GetNames(typeof(MouseAction)).Length;
                if (forward && currentAction >= l)
                {
                    currentAction = 1;
                }
                else if (!forward && currentAction <= 0)
                {
                    currentAction = l - 1;
                }
                ma = actions[currentAction];

                updateMA(ma);
            }

            public void UpdateLocation(int xx, int yy)
            {
                x = xx;
                y = yy;

                updateLoc(x, y);
            }

            public int[] GetLoc()
            {
                return new int[] { x, y };
            }

            public MouseAction GetMA()
            {
                return ma;
            }

            public override void Step(float x, Robot r)
            {
                Point p = CursorMonitor.Position();
                float fx = -((x - 1) * (x - 1)) + 1;
                float dx = this.x - p.X;
                float dy = this.y - p.Y;

                dx *= fx; dy *= fx;
                dx += 0.5f; dy += 0.5f;
                r.DoAction(MouseAction.WM_MOUSEMOVE, (int)(p.X + dx), (int)(p.Y + dy));
            }

            public override string ToString()
            {
                return new StringBuilder()
                    .Append(ma.ToString())
                    .Append("|")
                    .Append("MOUSE: ")
                    .Append(x)
                    .Append(", ")
                    .Append(y)
                    .Append(" ")
                    .Append(GetTime())
                    .ToString();
            }
        }

        public HashedLinkedList<Keyframe> Keyframes { get; set; }

        private bool isPlaying = false;
        private LinkedList<Keyframe> buffer;

        public Recording()
        {
            Keyframes = new HashedLinkedList<Keyframe>();
        }

        public void Divert()
        {
            buffer = new LinkedList<Keyframe>();
        }

        public void Flush()
        {
            if (buffer == null) return;

            foreach (Keyframe k in buffer)
            {
                Keyframes.AddLast(k);
            }

            buffer = null;
        }

        private long checkTime(long time, Keyframe k)
        {
            var node = Keyframes.GetNode(k);
            if (node.Previous == null) return 0;

            long p = node.Previous.Value.GetTime();
            if (time < p) time = p;
            return time;
        }

        private void shiftTimes(long delta, Keyframe k)
        {
            var node = Keyframes.GetNode(k);
            node = node.Next;
            while(node != null)
            {
                node.Value.shiftTimeInternal(delta);
                node = node.Next;
            }
        }

        public void Distribute(float speed)
        {
            var per = 1f / speed;
            long period = ((long)(per * 1000 + 0.5f));
            var node = Keyframes.First;

            long t = 0;
            while(node != null)
            {
                node.Value.setTimeInternal(t);

                t += period;
                node = node.Next;
            }
        }

        public Keyframe AddDefaultM(Keyframe after)
        {
            return addDefaultDelegate(after, (time) =>
            {
               return new KeyframeM(MouseAction.WM_LBUTTONDOWN, 0, 0, time, this);
            });
        }

        public Keyframe AddDefaultK(Keyframe after)
        {
            return addDefaultDelegate(after, (time) =>
            {
               return new KeyframeK(KeyActions.PRESS, Key.X, time, this);
            });
        }

        private Keyframe addDefaultDelegate(Keyframe after, Func<long, Keyframe> makeNew)
        {
            Keyframe kf;
            if (after == null)
            {
                kf = makeNew(0);
                Keyframes.AddFirst(kf);
            }
            else
            {
                var n = Keyframes.GetNode(after);
                var timeB = n.Next != null ? n.Next.Value.GetTime() : n.Value.GetTime();
                long time = (long)((timeB + n.Value.GetTime() + 0.5f) / 2f);
                kf = makeNew(time);

                Keyframes.AddAfter(n, kf);
            }

            return kf;
        }

        public void AddKeyframe(MouseAction ma, int x, int y, long time)
        {
            var kf = new KeyframeM(ma, x, y, time, this);
            addKeyframe(kf);
            //return kf;
        }

        public void AddKeyframe(KeyActions ka, Key k, long time)
        {
            var kf = new KeyframeK(ka, k, time, this);
            addKeyframe(kf);
            //return kf;
        }

        private void addKeyframe(Keyframe k)
        {
            lock (this)
            {
                if (isPlaying)
                    throw new Exception("cannot modify while playing");
            }

            if (buffer != null)
            {
                buffer.AddLast(k);
            }
            else
            {
                Keyframes.AddLast(k);
            }
        }

        public Keyframe GetLinkedWithOffset(Keyframe k, int os)
        {
            var node = Keyframes.GetNode(k);

            Action move;
            if (os > 0)
                move = () => node = node.Next;
            else
                move = () => node = node.Previous;

            int delta = os > 0 ? -1 : 1;
            while (node != null && os != 0)
            {
                move();
                os += delta;
            }

            return node?.Value;
        }

        public bool KeyframeOffsetExists(Keyframe k, int os)
        {
            var node = Keyframes.GetNode(k);

            Action move;
            if (os > 0)
                move = () => node = node.Next;
            else
                move = () => node = node.Previous;

            int delta = os > 0 ? -1 : 1;
            while (node != null && os != 0)
            {
                move();
                os += delta;
            }

            return os == 0 && node!= null;
        }

        public void ShiftKeyframe(Keyframe k, Keyframe toAfter)
        {
            LinkedList<long> times = new LinkedList<long>();
            foreach(var kf in Keyframes)
            {
                times.AddLast(kf.GetTime());
            }

            Keyframes.Remove(k);
            InsertKeyframe(k, toAfter);

            var nt = times.First;
            var nk = Keyframes.First;

            while(nk != null)
            {
                nk.Value.setTimeInternal(nt.Value);

                nt = nt.Next;
                nk = nk.Next;
            }
        }

        public void InsertKeyframe(Keyframe k, Keyframe after)
        {
            if (after == null) Keyframes.AddFirst(k);
            else Keyframes.AddAfter(after, k);
        }

        public void DeleteKeyframe(Keyframe k)
        {
            Keyframes.Remove(k);
        }

        public void Play(Robot r, Action onComplete, int repeats, float speed)
        {
            //Console.WriteLine("here");
            Thread t = new Thread(() =>
            {
                repeats++;
                while(repeats > 0)
                {
                    long startT = Time.Millis();
                    long playT = startT;
                    //Console.WriteLine(Keyframes.Count());
                    LinkedListNode<Keyframe> node = Keyframes.First;
                    while (node != null)
                    {
                        Keyframe k = node.Value;

                        lock (this)
                        {
                            if (!isPlaying) break;
                        }

                        long currT = (long)((Time.Millis() - startT) * speed);

                        long nextT = k.GetTime();
                        if (currT < nextT)
                        {
                            long total = nextT - playT;
                            long running = currT - playT;
                            k.Step((float)running / (float)total, r);
                            Thread.Sleep(16);
                            continue;
                        }
                        else
                        {
                            playT = currT;
                        }

                        k.DoAction(r);
                        node = node.Next;
                    }

                    repeats--;
                }

                onComplete();
            });

            lock(this)
            {
                isPlaying = true;
            }
            t.Start();
        }

        public void Stop()
        {
            lock (this)
            {
                isPlaying = false;
            }
        }

        public override string ToString()
        {
            if (Keyframes.Count == 0) return "<<EMPTY RECORDING>>";

            StringBuilder s = new StringBuilder();
            foreach (Keyframe k in Keyframes)
            {
                s.Append(k.ToString()).Append("\n");
            }
            
            s.Remove(s.Length - 1, 1);
            return s.ToString();
        }
    }
}
