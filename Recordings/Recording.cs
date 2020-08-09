using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Data;
using InputHook;
using OutputSimulator;

namespace Recordings
{
    internal class Recording
    {
        public abstract class KeyFrame
        {
            private int t;
            public KeyFrame(int time) { t = time; }

            public int GetTime() { return t; }

            public abstract void DoAction(Robot r);
        }

        public class KeyFrameK : KeyFrame
        {
            private bool d;
            private Keys k;

            public KeyFrameK(bool isDown, Keys k, int timestamp) : base(timestamp)
            {
                d = isDown;
                this.k = k;
            }

            public override void DoAction(Robot r)
            {
                if (d)
                    r.KeyDown(k);
                else
                    r.KeyUp(k);

                Console.WriteLine(k);
            }

            public override string ToString()
            {
                return new StringBuilder()
                    .Append("KEY: ")
                    .Append(d ? "DOWN" : " UP ")
                    .Append("|")
                    .Append(k.ToString())
                    .Append(" ")
                    .Append(GetTime())
                    .ToString();
            }
        }

        public class KeyFrameM : KeyFrame
        {
            private MouseAction ma;
            private int x, y;

            public KeyFrameM(MouseAction a, int x, int y, int timestamp) : base(timestamp)
            {
                this.x = x;
                this.y = y;
                this.ma = a;
            }

            public override void DoAction(Robot r)
            {
                switch (ma)
                {
                    case MouseAction.WM_LBUTTONDOWN:
                        r.LDown(x, y);
                        break;
                    case MouseAction.WM_LBUTTONUP:
                        r.LUp(x, y);
                        break;
                    case MouseAction.WM_MOUSEMOVE:
                        r.Move(x, y);
                        break;
                    case MouseAction.WM_MOUSEWHEEL:
                        Console.WriteLine("SCROLL not implemented!!");
                        break;
                    case MouseAction.WM_RBUTTONDOWN:
                        r.RDown(x, y);
                        break;
                    case MouseAction.WM_RBUTTONUP:
                        r.RUp(x, y);
                        break;
                }
            }

            public override string ToString()
            {
                return new StringBuilder()
                    .Append("MOUSE: ")
                    .Append(x)
                    .Append(", ")
                    .Append(y)
                    .Append("|")
                    .Append(ma.ToString())
                    .Append(" ")
                    .Append(GetTime())
                    .ToString();
            }
        }

        private LinkedList<KeyFrame> kfs;
        private bool isPlaying = false;
        private LinkedList<KeyFrame> buffer;

        public Recording()
        {
            kfs = new LinkedList<KeyFrame>();
        }

        public void Divert()
        {
            buffer = new LinkedList<KeyFrame>();
        }

        public void Flush()
        {
            if (buffer == null) return;

            foreach (KeyFrame k in buffer)
            {
                kfs.AddLast(k);
            }

            buffer = null;
        }

        public void AddKeyFrame(KeyFrame k)
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
                kfs.AddLast(k);
            }
        }

        public void Play(Robot r, Action onComplete)
        {
            //Console.WriteLine("here");
            Thread t = new Thread(() =>
            {
                long startT = Time.Millis();
                //Console.WriteLine(kfs.Count());
                LinkedListNode<KeyFrame> node = kfs.First;
                while(node != null)
                {
                    KeyFrame k = node.Value;

                    lock (this)
                    {
                        if (!isPlaying) break;
                    }

                    long currT = Time.Millis() - startT;
                    if (currT < k.GetTime())
                    {
                        Thread.Sleep(16);
                        continue;
                    }

                    k.DoAction(r);
                    node = node.Next;
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
            if (kfs.Count == 0) return "<<EMPTY REC>>";

            StringBuilder s = new StringBuilder();
            foreach (KeyFrame k in kfs)
            {
                s.Append(k.ToString()).Append("\n");
            }
            
            s.Remove(s.Length - 1, 1);
            return s.ToString();
        }
    }
}
