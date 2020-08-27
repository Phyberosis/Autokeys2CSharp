using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
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
        public abstract class Keyframe
        {
            private long t;
            public delegate void DoActionDelegate(Robot r);
            public DoActionDelegate DoAction;
            internal LinkedListNode<Keyframe> Container;

            public Keyframe(long time, LinkedListNode<Keyframe> container)
            {
                Container = container;
                t = time;
            }

            public long GetTime() { return t; }

            public void SlideTime(long delta)
            {
                t += delta;
            }

            public void SetTime(long time)
            {
                t = time;
            }

            public abstract string GetInfo();
            public abstract string GetDescription();

            public virtual void Step(float x, Robot r)
            {

            }

            #region need fixing

            public virtual bool HandlesKeys() { return false; }
            public virtual KeyActions GetKeyAction() { return KeyActions.NONE; }
            public virtual Key GetKey() { return Key.None; }

            public virtual MouseAction GetMouseAction() { return MouseAction.NONE; }
            public virtual int[] GetLocation() { return new int[] { -1, -1 }; }
            #endregion
        }

        public class KeyFrameK : Keyframe
        {
            private KeyActions ka;
            private Key k;

            internal KeyFrameK(KeyActions ka, Key k, long timestamp, LinkedListNode<Keyframe> container) : base(timestamp, container)
            {
                this.k = k;
                this.ka = ka;
                DoAction = (r) => { r.DoAction(ka, k); };

                //Info = k.ToString();
                //Description = isDown ? "DOWN" : "UP";
            }

            public override bool HandlesKeys() { return true; }

            public override string GetInfo()
            {
                return k.ToString();
            }

            public override string GetDescription()
            {
                return ka.ToString();
            }

            public override Key GetKey()
            {
               return k;
            }
            public override KeyActions GetKeyAction()
            {
                return ka;
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
        }

        public class KeyFrameM : Keyframe
        {
            private MouseAction ma;
            private int x, y;

            internal KeyFrameM(MouseAction a, int x, int y, long timestamp, LinkedListNode<Keyframe> container) : base(timestamp, container)
            {
                this.x = x;
                this.y = y;
                this.ma = a;

                DoAction = (r) => r.DoAction(ma, x, y);

                //Info = MouseActionVerbalizer.Convert(a);
                //Description = "("+x+", " +y+")";
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

            public override string GetInfo()
            {
                return MouseActionVerbalizer.Convert(ma) + MouseActionVerbalizer.GetType(ma);
            }

            public override string GetDescription()
            {
                return "at (" + x + ", " + y + ")";
            }

            public override MouseAction GetMouseAction() 
            {
                return ma;
            }

            public override int[] GetLocation()
            {
                return new int[] { x, y };
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

        public LinkedList<Keyframe> Keyframes { get; set; }
        private bool isPlaying = false;
        private LinkedList<Keyframe> buffer;

        public Recording()
        {
            Keyframes = new LinkedList<Keyframe>();
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

        public void AddKeyFrame(Keyframe k)
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

        public void ShiftKeyframe(Keyframe k, Keyframe toAfter)
        {
            Keyframes.Remove(k);
            Keyframes.AddAfter(toAfter.Container, k);
        }

        public void InsertKeyframe(Keyframe k, Keyframe after)
        {

        }

        public void DeleteKeyframe(Keyframe k)
        {

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
