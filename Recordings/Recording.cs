using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Data;
using InputHook;
using OutputSimulator;

namespace Recordings
{
    public class Recording
    {
        public abstract class KeyFrame
        {
            private long t;
            public delegate void DoActionDelegate(Robot r);
            public DoActionDelegate DoAction;

            public KeyFrame(long time) 
            { 
                t = time;
            }

            public long GetTime() { return t; }

            public void SlideTime(long delta) 
            {
                t += delta;
            }

            public abstract string GetInfo();
            public abstract string GetDescription();


            #region need fixing

            public virtual bool HandlesKeys() { return false; }
            public virtual KeyActions GetKeyAction() { return KeyActions.NONE; }
            public virtual Keys GetKey() { return Keys.None; }

            #endregion
        }

        public class KeyFrameK : KeyFrame
        {
            private KeyActions ka;
            private Keys k;

            public KeyFrameK(KeyActions ka, Keys k, long timestamp) : base(timestamp)
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

            public override Keys GetKey()
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

        public class KeyFrameM : KeyFrame
        {
            private MouseAction ma;
            private int x, y;

            public KeyFrameM(MouseAction a, int x, int y, long timestamp) : base(timestamp)
            {
                this.x = x;
                this.y = y;
                this.ma = a;

                DoAction = (r) => r.DoAction(ma, x, y);

                //Info = MouseActionVerbalizer.Convert(a);
                //Description = "("+x+", " +y+")";
            }

            public override string GetInfo()
            {
                return MouseActionVerbalizer.Convert(ma);
            }

            public override string GetDescription()
            {
                return MouseActionVerbalizer.GetTypePrefix(ma)+ " (" + x + ", " + y + ")";
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

        public OpenLinkedList<KeyFrame> Keyframes { get; set; }
        private bool isPlaying = false;
        private LinkedList<KeyFrame> buffer;

        public Recording()
        {
            Keyframes = new OpenLinkedList<KeyFrame>();
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
                Keyframes.AddLast(k);
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
                Keyframes.AddLast(k);
            }
        }

        public void Play(Robot r, Action onComplete)
        {
            //Console.WriteLine("here");
            Thread t = new Thread(() =>
            {
                long startT = Time.Millis();
                //Console.WriteLine(kfs.Count());
                OpenLinkedListNode<KeyFrame> node = Keyframes.First();
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

                    int todo_animate_mouse_move;
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
            if (Keyframes.Count() == 0) return "<<EMPTY RECORDING>>";

            StringBuilder s = new StringBuilder();
            foreach (KeyFrame k in Keyframes)
            {
                s.Append(k.ToString()).Append("\n");
            }
            
            s.Remove(s.Length - 1, 1);
            return s.ToString();
        }
    }
}
